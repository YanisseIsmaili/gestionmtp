using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Yprotect.Models;
using Yprotect.Services;
using Yprotect.Views;

namespace Yprotect.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private ObservableCollection<PasswordEntry> _passwords;
        private ObservableCollection<PasswordEntry> _filteredPasswords;
        private PasswordEntry? _selectedPassword;
        private string _searchText = "";

        public MainWindowViewModel()
        {
            _dataService = new DataService();
            _passwords = new ObservableCollection<PasswordEntry>();
            _filteredPasswords = new ObservableCollection<PasswordEntry>();
            
            // Charger les mots de passe sauvegardés
            LoadPasswords();
            
            // Commandes
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, () => SelectedPassword != null);
            DeleteCommand = new RelayCommand(Delete, () => SelectedPassword != null);
            
            // Initialiser la liste filtrée
            UpdateFilteredPasswords();
            
            // S'abonner aux changements pour sauvegarder automatiquement
            _passwords.CollectionChanged += (s, e) => SavePasswords();
        }

        public ObservableCollection<PasswordEntry> Passwords
        {
            get => _filteredPasswords;
        }

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

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private void LoadPasswords()
        {
            var loadedPasswords = _dataService.LoadPasswords();
            _passwords.Clear();
            
            foreach (var password in loadedPasswords)
            {
                // S'abonner aux changements de propriétés pour sauvegarder automatiquement
                password.PropertyChanged += (s, e) => SavePasswords();
                _passwords.Add(password);
            }
        }

        private void SavePasswords()
        {
            _dataService.SavePasswords(_passwords);
        }

        private void UpdateFilteredPasswords()
        {
            var filtered = string.IsNullOrWhiteSpace(_searchText)
                ? _passwords.ToList()
                : _passwords.Where(p => 
                    p.Site.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Username.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToList();

            // Ajouter les nouveaux éléments
            for (int i = 0; i < filtered.Count; i++)
            {
                if (i >= _filteredPasswords.Count)
                {
                    _filteredPasswords.Add(filtered[i]);
                }
                else if (_filteredPasswords[i] != filtered[i])
                {
                    _filteredPasswords[i] = filtered[i];
                }
            }

            // Supprimer les éléments en trop
            while (_filteredPasswords.Count > filtered.Count)
            {
                _filteredPasswords.RemoveAt(_filteredPasswords.Count - 1);
            }
        }

        private async void Add()
        {
            var dialog = new EditPasswordDialog();
            if (await dialog.ShowDialog<bool>(GetMainWindow()))
            {
                // S'abonner aux changements de propriétés pour le nouveau mot de passe
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
                    // La sauvegarde se fera automatiquement via PropertyChanged
                    UpdateFilteredPasswords();
                }
            }
        }

        private Avalonia.Controls.Window GetMainWindow()
        {
            return Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow!
                : throw new InvalidOperationException("Could not find main window.");
        }

        private void Delete()
        {
            if (SelectedPassword != null)
            {
                _passwords.Remove(SelectedPassword);
                SelectedPassword = null;
                UpdateFilteredPasswords();
                // La sauvegarde se fera automatiquement via CollectionChanged
            }
        }
    }
}