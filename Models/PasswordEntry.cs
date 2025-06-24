using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Yprotect.Models
{
    public class PasswordEntry : INotifyPropertyChanged
    {
        private string _site = "";
        private string _username = "";
        private string _password = "";

        public string Site 
        { 
            get => _site;
            set => SetProperty(ref _site, value);
        }

        public string Username 
        { 
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password 
        { 
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string MaskedPassword => new string('●', Math.Min(Password.Length, 12));

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(Password))
                OnPropertyChanged(nameof(MaskedPassword));
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}