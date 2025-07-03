using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Yprotect.Model;
using Yprotect.Modeles;
using Yprotect.Utils;
using Yprotect.Views;
using Yprotect.ViewModels;

namespace Yprotect.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object? sender, RoutedEventArgs e)
        {
            var nom = NomTextBox.Text?.Trim();
            var prenom = PrenomTextBox.Text?.Trim();
            var email = EmailTextBox.Text?.Trim();
            var password = PasswordTextBox.Text;
            var confirmPassword = ConfirmPasswordTextBox.Text;

            if (!ValidateInputs(nom, prenom, email, password, confirmPassword))
                return;

            try
            {
                using var context = new YprotectContext();
                
                // Vérifier si email existe déjà
                if (context.Utilisateurs.Any(u => u.Email == email))
                {
                    ShowError("Cet email est déjà utilisé");
                    return;
                }

                // Créer nouvel utilisateur
                var newUser = new BDUtilisateur
                {
                    Id = Guid.NewGuid(),
                    Nom = nom!,
                    Prenom = prenom!,
                    Email = email!,
                    MotDePasse = HashPassword(password!),
                    Token = "",
                    DateCreation = DateTime.Now
                };

                context.Utilisateurs.Add(newUser);
                context.SaveChanges();

                Logger.Success($"Nouveau compte créé pour {email}");
                
                // Ouvrir la fenêtre principale
                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };
                
                if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = mainWindow;
                }
                
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError("Erreur lors de la création du compte");
                Logger.Error("Erreur création compte", ex);
            }
        }

        private bool ValidateInputs(string? nom, string? prenom, string? email, string? password, string? confirmPassword)
        {
            if (string.IsNullOrEmpty(nom))
            {
                ShowError("Le nom est requis");
                return false;
            }

            if (string.IsNullOrEmpty(prenom))
            {
                ShowError("Le prénom est requis");
                return false;
            }

            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                ShowError("Email invalide");
                return false;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                ShowError("Le mot de passe doit contenir au moins 8 caractères");
                return false;
            }

            if (password != confirmPassword)
            {
                ShowError("Les mots de passe ne correspondent pas");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.IsVisible = true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private void BackToLoginButton_Click(object? sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            
            if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = loginWindow;
            }
            
            loginWindow.Show();
            this.Close();
        }
    }
}