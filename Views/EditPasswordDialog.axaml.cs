using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Yprotect.Models;

namespace Yprotect.Views
{
    public partial class EditPasswordDialog : Window
    {
        public PasswordEntry PasswordEntry { get; private set; }
        public bool Result { get; private set; }

        public EditPasswordDialog(PasswordEntry? entry = null)
        {
            InitializeComponent();

            PasswordEntry = entry ?? new PasswordEntry();

            // Remplir les champs
            SiteTextBox.Text = PasswordEntry.Site;
            UsernameTextBox.Text = PasswordEntry.Username;
            EmailTextBox.Text = PasswordEntry.Email;
            PasswordTextBox.Text = PasswordEntry.Password;
            NotesTextBox.Text = PasswordEntry.Notes;
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            // Sauvegarder les modifications
            PasswordEntry.Site = SiteTextBox.Text ?? "";
            PasswordEntry.Username = UsernameTextBox.Text ?? "";
            PasswordEntry.Email = EmailTextBox.Text ?? "";
            PasswordEntry.Password = PasswordTextBox.Text ?? "";
            PasswordEntry.Notes = NotesTextBox.Text ?? "";

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