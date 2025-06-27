using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Yprotect.Models;
using Yprotect.Services;
using Yprotect.Views;
using Yprotect.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

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
            
            LoadPasswords();
            
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, () => SelectedPassword != null);
            DeleteCommand = new RelayCommand(Delete, () => SelectedPassword != null);
            ImportCsvCommand = new RelayCommand(ImportCsv);
            
            UpdateFilteredPasswords();
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
        public ICommand ImportCsvCommand { get; }

        private void LoadPasswords()
        {
            var loadedPasswords = _dataService.LoadPasswords();
            _passwords.Clear();
            
            foreach (var password in loadedPasswords)
            {
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

            while (_filteredPasswords.Count > filtered.Count)
            {
                _filteredPasswords.RemoveAt(_filteredPasswords.Count - 1);
            }
        }

        private async void Add()
        {
            var dialog = new EditPasswordDialog();
            var mainWindow = GetMainWindow();
            
            if (await dialog.ShowDialog<bool>(mainWindow))
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
                var mainWindow = GetMainWindow();
                
                if (await dialog.ShowDialog<bool>(mainWindow))
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
            if (result != null && result.Length > 0)
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

        private Window GetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow ?? throw new InvalidOperationException("MainWindow not found");
            }
            throw new InvalidOperationException("Application lifetime not found");
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
    }
}