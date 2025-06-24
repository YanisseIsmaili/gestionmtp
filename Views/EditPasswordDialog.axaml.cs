using Avalonia.Controls;
using Avalonia.Interactivity;
using Yprotect.Models;

namespace Yprotect.Views
{
    public partial class EditPasswordDialog : Window
    {
        public PasswordEntry PasswordEntry { get; private set; }
        public bool Result { get; private set; }
        private readonly bool _isNewEntry;

        public EditPasswordDialog(PasswordEntry? entry = null)
        {
            InitializeComponent();
            
            _isNewEntry = entry == null;
            PasswordEntry = entry ?? new PasswordEntry();
            
            // Remplir les champs
            SiteTextBox.Text = PasswordEntry.Site;
            UsernameTextBox.Text = PasswordEntry.Username;
            PasswordTextBox.Text = PasswordEntry.Password;
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            // Sauvegarder les modifications dans l'objet existant
            PasswordEntry.Site = SiteTextBox.Text ?? "";
            PasswordEntry.Username = UsernameTextBox.Text ?? "";
            PasswordEntry.Password = PasswordTextBox.Text ?? "";
            
            Result = true;
            Close(true);
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Result = false;
            Close(false);
        }
    }
}