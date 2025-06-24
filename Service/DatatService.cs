using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Yprotect.Models;

namespace Yprotect.Services
{
    public class DataService
    {
        private readonly string _dataPath;

        public DataService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appData, "Yprotect");
            Directory.CreateDirectory(appFolder);
            _dataPath = Path.Combine(appFolder, "passwords.json");
        }

        public List<PasswordEntry> LoadPasswords()
        {
            try
            {
                if (!File.Exists(_dataPath))
                    return GetDefaultPasswords();

                var json = File.ReadAllText(_dataPath);
                var passwords = JsonSerializer.Deserialize<List<PasswordEntry>>(json);
                return passwords ?? GetDefaultPasswords();
            }
            catch
            {
                return GetDefaultPasswords();
            }
        }

        public void SavePasswords(IEnumerable<PasswordEntry> passwords)
        {
            try
            {
                var json = JsonSerializer.Serialize(passwords, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(_dataPath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }

        private List<PasswordEntry> GetDefaultPasswords()
        {
            return new List<PasswordEntry>
            {
                new PasswordEntry 
                { 
                    Site = "Gmail", 
                    Username = "user@gmail.com", 
                    Password = "motdepasse123" 
                },
                new PasswordEntry 
                { 
                    Site = "Facebook", 
                    Username = "moncompte", 
                    Password = "supersecret456" 
                }
            };
        }
    }
}