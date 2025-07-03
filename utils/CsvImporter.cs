using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yprotect.Models;
using Yprotect.Utils;

namespace Yprotect.Services
{
    public static class CsvImporter
    {
        public static List<PasswordEntry> ImportFromCsv(string filePath)
        {
            var passwords = new List<PasswordEntry>();
            
            try
            {
                var lines = File.ReadAllLines(filePath);
                var startIndex = HasHeader(lines) ? 1 : 0;
                
                for (int i = startIndex; i < lines.Length; i++)
                {
                    var entry = ParseCsvLine(lines[i]);
                    if (entry != null)
                        passwords.Add(entry);
                }
                
                Logger.Success($"Imported {passwords.Count} passwords from CSV");
            }
            catch (Exception ex)
            {
                Logger.Error("Error importing CSV", ex);
            }
            
            return passwords;
        }
        
        private static bool HasHeader(string[] lines)
        {
            if (lines.Length == 0) return false;
            
            var firstLine = lines[0].ToLower();
            return firstLine.Contains("name") || firstLine.Contains("url") || 
                   firstLine.Contains("site") || firstLine.Contains("username") || 
                   firstLine.Contains("password");
        }
        
        private static PasswordEntry? ParseCsvLine(string line)
        {
            try
            {
                var parts = SplitCsvLine(line);
                
                // Format: name,site,url,username,password,note
                if (parts.Length >= 6)
                {
                    return new PasswordEntry
                    {
                        Site = parts[1].Trim(),      // site
                        Username = parts[0].Trim(),  // name → Username
                        Email = parts[3].Trim(),     // username → Email
                        Password = parts[4].Trim(),  // password
                        Notes = parts[5].Trim()      // note
                    };
                }
                // Fallback format: site,username,password
                else if (parts.Length >= 3)
                {
                    return new PasswordEntry
                    {
                        Site = parts[0].Trim(),
                        Username = parts[1].Trim(),
                        Password = parts[2].Trim()
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error parsing CSV line: {line}", ex);
            }
            
            return null;
        }
        
        private static string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;
            
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            
            result.Add(current);
            return result.ToArray();
        }
    }
}