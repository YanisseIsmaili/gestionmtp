using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Yprotect.Models;
using Yprotect.Services;
using Yprotect.Views;
using Yprotect.Utils;
using Yprotect.Model;
using Yprotect.Modeles;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Yprotect.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<PasswordEntry> _passwords;
        private ObservableCollection<PasswordEntry> _filteredPasswords;
        private PasswordEntry? _selectedPassword;
        private string _searchText = "";
        private string _themeButtonText = "🖥️ System";

        public MainWindowViewModel()
        {
            _passwords = new ObservableCollection<PasswordEntry>();
            _filteredPasswords = new ObservableCollection<PasswordEntry>();

            LoadPasswords();

            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, () => SelectedPassword != null);
            DeleteCommand = new RelayCommand(Delete, () => SelectedPassword != null);
            ImportCsvCommand = new RelayCommand(ImportCsv);
            CycleThemeCommand = new RelayCommand(CycleTheme);
            SettingsCommand = new RelayCommand(OpenSettings);
            LogoutCommand = new RelayCommand(Logout);
            GeneratePasswordCommand = new RelayCommand(ShowGeneratedPassword);
            CopyPasswordCommand = new RelayCommand<PasswordEntry>(CopyPassword);

            UpdateFilteredPasswords();
            _passwords.CollectionChanged += (s, e) => SavePasswords();
        }

        public ObservableCollection<PasswordEntry> Passwords => _filteredPasswords;

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                UpdateFilteredPasswords();
            }
        }

        public PasswordEntry? SelectedPassword
        {
            get => _selectedPassword;
            set
            {
                SetProperty(ref _selectedPassword, value);
                ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public string ThemeButtonText
        {
            get => _themeButtonText;
            set => SetProperty(ref _themeButtonText, value);
        }

        public bool IsAdmin => UserSession.IsAdmin;
        public string UserRoleText => UserSession.IsAdmin ? "👑 Administrateur" : "👤 Utilisateur";
        public string UserDisplayName => $"{UserSession.CurrentUser?.Prenom} {UserSession.CurrentUser?.Nom}";

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ImportCsvCommand { get; }
        public ICommand CycleThemeCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand GeneratePasswordCommand { get; }
        public ICommand CopyPasswordCommand { get; }

        private void LoadPasswords()
        {
            try
            {
                using var context = new YprotectContext();

                // Filtrer par utilisateur connecté
                var userId = UserSession.CurrentUser?.Id;
                if (userId == null)
                    return;

                var dbPasswords = context.Passwords
                    .Where(p => p.UtilisateurId == userId)
                    .ToList();

                _passwords.Clear();

                foreach (var dbPassword in dbPasswords)
                {
                    var entry = new PasswordEntry
                    {
                        Site = dbPassword.Site,
                        Username = dbPassword.NomUtilisateur,
                        Password = dbPassword.MotDePasseChiffre
                    };

                    entry.PropertyChanged += (s, e) => SavePasswords();
                    _passwords.Add(entry);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Erreur lors du chargement des mots de passe", ex);
            }
        }

        private void SavePasswords()
        {
            try
            {
                using var context = new YprotectContext();

                context.Passwords.RemoveRange(context.Passwords);

                foreach (var password in _passwords)
                {
                    context.Passwords.Add(new BDPassword
                    {
                        Id = Guid.NewGuid(),
                        Site = password.Site,
                        NomUtilisateur = password.Username,
                        MotDePasseChiffre = password.Password,
                        DateCreation = DateTime.Now
                    });
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("Erreur lors de la sauvegarde des mots de passe", ex);
            }
        }

        private void UpdateFilteredPasswords()
        {
            var filtered = string.IsNullOrWhiteSpace(_searchText)
                ? _passwords.ToList()
                : _passwords.Where(p =>
                    p.Site.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Username.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToList();

            for (int i = 0; i < filtered.Count; i++)
            {
                if (i >= _filteredPasswords.Count)
                    _filteredPasswords.Add(filtered[i]);
                else if (_filteredPasswords[i] != filtered[i])
                    _filteredPasswords[i] = filtered[i];
            }

            while (_filteredPasswords.Count > filtered.Count)
                _filteredPasswords.RemoveAt(_filteredPasswords.Count - 1);
        }

        private async void Add()
        {
            var dialog = new EditPasswordDialog();
            if (await dialog.ShowDialog<bool>(GetMainWindow()))
            {
                dialog.PasswordEntry.PropertyChanged += (s, e) => SavePasswords();
                _passwords.Add(dialog.PasswordEntry);
                SelectedPassword = dialog.PasswordEntry;
                UpdateFilteredPasswords();
            }
        }

        private async void Edit()
        {
            if (SelectedPassword != null)
            {
                var dialog = new EditPasswordDialog(SelectedPassword);
                if (await dialog.ShowDialog<bool>(GetMainWindow()))
                {
                    UpdateFilteredPasswords();
                }
            }
        }

        private async void ImportCsv()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Import CSV",
                Filters = new List<FileDialogFilter>
                {
                    new() { Name = "CSV files", Extensions = { "csv" } },
                    new() { Name = "All files", Extensions = { "*" } }
                }
            };

            var result = await dialog.ShowAsync(GetMainWindow());
            if (result?.Length > 0)
            {
                var imported = CsvImporter.ImportFromCsv(result[0]);
                foreach (var entry in imported)
                {
                    entry.PropertyChanged += (s, e) => SavePasswords();
                    _passwords.Add(entry);
                }
                UpdateFilteredPasswords();
                Logger.Success($"Imported {imported.Count} passwords from CSV");
            }
        }

        private void CycleTheme()
        {
            ThemeService.CycleTheme();
            ThemeButtonText = ThemeService.GetCurrentThemeName();
        }

        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog(GetMainWindow());
        }

        private void Delete()
        {
            if (SelectedPassword != null)
            {
                _passwords.Remove(SelectedPassword);
                SelectedPassword = null;
                UpdateFilteredPasswords();
            }
        }

        private void Logout()
        {
            UserSession.Logout();

            var loginWindow = new LoginWindow();

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Ferme l'ancienne fenêtre après avoir affiché la nouvelle
                var oldMainWindow = desktop.MainWindow;
                desktop.MainWindow = loginWindow;
                loginWindow.Show();
                oldMainWindow?.Close();
            }
            else
            {
                loginWindow.Show();
            }
        }

        private Window GetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.MainWindow ?? throw new InvalidOperationException("MainWindow not found");
            throw new InvalidOperationException("Application lifetime not found");
        }

        // Générateur de mot de passe basé sur le dictionnaire
        public string GeneratePassword(int wordCount = 3, int minLength = 12)
        {
            try
            {
                using var context = new YprotectContext();
                var words = context.MotsDictionnaire.Select(m => m.Mot).ToList();

                if (words.Count == 0)
                    return "Aucun mot en base";

                var rnd = new Random();
                string password = "";

                while (password.Length < minLength)
                {
                    for (int i = 0; i < wordCount; i++)
                    {
                        var word = words[rnd.Next(words.Count)];
                        password += word;
                    }
                    password += rnd.Next(10).ToString();
                    password += "!@#$%&*"[rnd.Next(7)];
                }

                if (password.Length > minLength)
                    password = password.Substring(0, minLength);

                return password;
            }
            catch (Exception ex)
            {
                Logger.Error("Erreur génération mot de passe", ex);
                return "Erreur génération";
            }
        }

        // Commande pour afficher le mot de passe généré
        private async void ShowGeneratedPassword()
        {
            var password = GeneratePassword();
            var dialog = new Window
            {
                Title = "Mot de passe généré",
                Width = 400,
                Height = 200,
                Content = new TextBlock
                {
                    Text = $"Mot de passe généré :\n{password}\n\n(Copié dans le presse-papier)",
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var mainWindow = GetMainWindow();
            if (mainWindow.Clipboard != null)
                await mainWindow.Clipboard.SetTextAsync(password);

            await dialog.ShowDialog(mainWindow);
        }

        // Commande pour copier le mot de passe via le menu contextuel
        private async void CopyPassword(PasswordEntry? entry)
        {
            if (entry == null)
            {
                Logger.Error("Aucun mot de passe sélectionné pour la copie.");
                return;
            }

            if (string.IsNullOrEmpty(entry.Password))
            {
                Logger.Error("Le champ mot de passe est vide.");
                return;
            }

            // Utilise le presse-papier de la fenêtre principale
            var mainWindow = GetMainWindow();
            if (mainWindow.Clipboard != null)
                await mainWindow.Clipboard.SetTextAsync(entry.Password);

            var dialog = new Window
            {
                Title = "Copie",
                Width = 300,
                Height = 120,
                Content = new TextBlock
                {
                    Text = "Mot de passe copié dans le presse-papier.",
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            await dialog.ShowDialog(mainWindow);
        }
    }
}