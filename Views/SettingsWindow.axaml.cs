using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yprotect.Model;
using Yprotect.Modeles;
using Yprotect.Utils;

namespace Yprotect.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadWordCount();
        }

        private void LoadWordCount()
        {
            try
            {
                using var context = new YprotectContext();
                var count = context.MotsDictionnaire.Count();
                WordCountText.Text = count.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Erreur lors du comptage des mots", ex);
                WordCountText.Text = "Erreur";
            }
        }

        private async void ImportDictionaryButton_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Importer dictionnaire",
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new() { Name = "Fichiers texte et CSV", Extensions = { "txt", "csv" } },
                    new() { Name = "Fichiers texte", Extensions = { "txt" } },
                    new() { Name = "Fichiers CSV", Extensions = { "csv" } },
                    new() { Name = "Tous les fichiers", Extensions = { "*" } }
                }
            };

            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                ImportDictionary(result[0]);
            }
        }

        private void ImportDictionary(string filePath)
        {
            try
            {
                var addedCount = DictionaryImporter.ImportFromFile(filePath);
                ShowStatus($"✓ {addedCount} mots ajoutés", "#00ff00");
                LoadWordCount();
            }
            catch (Exception ex)
            {
                ShowStatus($"✗ {ex.Message}", "#ff0000");
                Logger.Error("Erreur lors de l'import du dictionnaire", ex);
            }
        }

        private async void ClearDictionaryButton_Click(object? sender, RoutedEventArgs e)
        {
            var result = await ShowConfirmDialog("Êtes-vous sûr de vouloir vider le dictionnaire ?");
            if (result)
            {
                try
                {
                    using var context = new YprotectContext();
                    context.MotsDictionnaire.RemoveRange(context.MotsDictionnaire);
                    context.SaveChanges();
                    
                    ShowStatus("✓ Dictionnaire vidé", "#00ff00");
                    LoadWordCount();
                    Logger.Success("Dictionnaire vidé");
                }
                catch (Exception ex)
                {
                    ShowStatus("✗ Erreur lors de la suppression", "#ff0000");
                    Logger.Error("Erreur lors de la suppression du dictionnaire", ex);
                }
            }
        }

        private async Task<bool> ShowConfirmDialog(string message)
        {
            var dialog = new Window
            {
                Title = "Confirmation",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var result = false;
            var panel = new StackPanel { Margin = new Avalonia.Thickness(20) };
            
            panel.Children.Add(new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap });
            
            var buttonPanel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Margin = new Avalonia.Thickness(0, 20, 0, 0) };
            
            var yesButton = new Button { Content = "Oui", Margin = new Avalonia.Thickness(0, 0, 10, 0) };
            yesButton.Click += (s, e) => { result = true; dialog.Close(); };
            
            var noButton = new Button { Content = "Non" };
            noButton.Click += (s, e) => { result = false; dialog.Close(); };
            
            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);
            panel.Children.Add(buttonPanel);
            
            dialog.Content = panel;
            
            await dialog.ShowDialog(this);
            return result;
        }
        // showStatus methode pour afficher le message de statut
        private void ShowStatus(string message, string color)
        {
            DictionaryStatusText.Text = message;
            DictionaryStatusText.Foreground = Avalonia.Media.Brush.Parse(color);
            DictionaryStatusText.IsVisible = true;
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}