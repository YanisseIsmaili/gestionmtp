using Avalonia;
using Avalonia.Styling;
using System;

namespace Yprotect.Services
{
    public enum AppTheme
    {
        System,
        Dark,
        Light,
        Neon
    }

    public static class ThemeService
    {
        private static AppTheme _currentTheme = AppTheme.System;

        public static void SetTheme(AppTheme theme)
        {
            _currentTheme = theme;
            
            if (Application.Current != null)
            {
                switch (theme)
                {
                    case AppTheme.System:
                        Application.Current.RequestedThemeVariant = ThemeVariant.Default;
                        break;
                    case AppTheme.Dark:
                        Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                        break;
                    case AppTheme.Light:
                        Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                        break;
                    case AppTheme.Neon:
                        Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                        ApplyNeonTheme();
                        break;
                }
            }
        }

        public static void CycleTheme()
        {
            _currentTheme = _currentTheme switch
            {
                AppTheme.System => AppTheme.Dark,
                AppTheme.Dark => AppTheme.Light,
                AppTheme.Light => AppTheme.Neon,
                AppTheme.Neon => AppTheme.System,
                _ => AppTheme.System
            };
            
            SetTheme(_currentTheme);
        }

        public static string GetCurrentThemeName()
        {
            return _currentTheme switch
            {
                AppTheme.System => "🖥️ System",
                AppTheme.Dark => "🌙 Dark",
                AppTheme.Light => "☀️ Light",
                AppTheme.Neon => "⚡ Neon",
                _ => "🖥️ System"
            };
        }

        private static void ApplyNeonTheme()
        {
            // Application du thème néon via les ressources
            if (Application.Current?.Resources != null)
            {
                var resources = Application.Current.Resources;
                
                // Couleurs néon
                resources["SystemControlBackgroundChromeMediumLowBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1a1a2e"));
                resources["SystemControlForegroundBaseHighBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ffff"));
                resources["SystemControlForegroundBaseMediumBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ffff"));
                resources["SystemControlHighlightAccentBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#ff00ff"));
                resources["SystemControlBackgroundBaseLowBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#0f0f23"));
            }
        }
    }
}