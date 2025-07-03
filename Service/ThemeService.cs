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
        public static event Action? ThemeChanged;

        static ThemeService()
        {
            // Détecter le thème OS au démarrage
            DetectSystemTheme();
        }

        private static void DetectSystemTheme()
        {
            if (Application.Current != null)
            {
                var actualTheme = Application.Current.ActualThemeVariant;
                
                if (actualTheme == ThemeVariant.Dark)
                    _currentTheme = AppTheme.Dark;
                else if (actualTheme == ThemeVariant.Light)
                    _currentTheme = AppTheme.Light;
                else
                    _currentTheme = AppTheme.System;
                
                SetTheme(_currentTheme);
            }
        }

        public static void SetTheme(AppTheme theme)
        {
            _currentTheme = theme;
            
            if (Application.Current != null)
            {
                switch (theme)
                {
                    case AppTheme.System:
                        Application.Current.RequestedThemeVariant = ThemeVariant.Default;
                        RemoveNeonTheme();
                        break;
                    case AppTheme.Dark:
                        Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                        RemoveNeonTheme();
                        break;
                    case AppTheme.Light:
                        Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                        RemoveNeonTheme();
                        break;
                    case AppTheme.Neon:
                        Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                        ApplyNeonTheme();
                        break;
                }
                
                ThemeChanged?.Invoke();
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

        public static bool IsNeonTheme() => _currentTheme == AppTheme.Neon;

        private static void ApplyNeonTheme()
        {
            if (Application.Current?.Resources != null)
            {
                var resources = Application.Current.Resources;
                
                // Background colors
                resources["SystemControlBackgroundChromeMediumLowBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1a1a2e"));
                resources["SystemControlBackgroundBaseLowBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#0f0f23"));
                resources["SystemControlBackgroundAltHighBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1a1a2e"));
                
                // Text colors
                resources["SystemControlForegroundBaseHighBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ffff"));
                resources["SystemControlForegroundBaseMediumBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ffff"));
                resources["SystemControlForegroundBaseLowBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ffff"));
                
                // Accent colors
                resources["SystemControlHighlightAccentBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#ff00ff"));
                resources["SystemControlForegroundAccentBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#ff00ff"));
                
                // Input controls
                resources["TextControlBackground"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#0f0f23"));
                resources["TextControlForeground"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ffff"));
                resources["TextControlBorderBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ffff"));
                
                // Button colors
                resources["ButtonBackground"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ff00"));
                resources["ButtonForeground"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#000000"));
                resources["ButtonBorderBrush"] = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#00ff00"));
            }
        }

        private static void RemoveNeonTheme()
        {
            if (Application.Current?.Resources != null)
            {
                var resources = Application.Current.Resources;
                var keysToRemove = new[]
                {
                    "SystemControlBackgroundChromeMediumLowBrush",
                    "SystemControlBackgroundBaseLowBrush",
                    "SystemControlBackgroundAltHighBrush",
                    "SystemControlForegroundBaseHighBrush",
                    "SystemControlForegroundBaseMediumBrush",
                    "SystemControlForegroundBaseLowBrush",
                    "SystemControlHighlightAccentBrush",
                    "SystemControlForegroundAccentBrush",
                    "TextControlBackground",
                    "TextControlForeground",
                    "TextControlBorderBrush",
                    "ButtonBackground",
                    "ButtonForeground",
                    "ButtonBorderBrush"
                };

                foreach (var key in keysToRemove)
                {
                    if (resources.ContainsKey(key))
                        resources.Remove(key);
                }
            }
        }
    }
}