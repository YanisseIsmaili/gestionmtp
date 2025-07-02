using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yprotect.Model;
using Yprotect.Modeles;
using Yprotect.Utils;

namespace Yprotect.Utils
{
    public static class DictionaryImporter
    {
        public static int ImportFromFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            
            return extension switch
            {
                ".txt" => ImportFromTextFile(filePath),
                ".csv" => ImportFromCsvFile(filePath),
                _ => throw new ArgumentException("Format non supporté. Utilisez .txt ou .csv")
            };
        }

        private static int ImportFromTextFile(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                return ProcessWords(lines);
            }
            catch (Exception ex)
            {
                Logger.Error("Erreur lors de l'import du fichier texte", ex);
                return 0;
            }
        }

        private static int ImportFromCsvFile(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                var words = new List<string>();

                foreach (var line in lines.Skip(1)) // Skip header if exists
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    
                    // Parse CSV line (simple splitting, can be enhanced)
                    var parts = line.Split(',');
                    foreach (var part in parts)
                    {
                        var cleanWord = part.Trim().Trim('"');
                        if (!string.IsNullOrWhiteSpace(cleanWord))
                            words.Add(cleanWord);
                    }
                }

                return ProcessWords(words.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error("Erreur lors de l'import du fichier CSV", ex);
                return 0;
            }
        }

        private static int ProcessWords(string[] lines)
        {
            var words = lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .Where(word => word.Length <= 50)
                .Distinct()
                .ToList();

            using var context = new YprotectContext();
            var existingWords = context.MotsDictionnaire.Select(m => m.Mot).ToHashSet();
            
            int addedCount = 0;
            foreach (var word in words)
            {
                if (!existingWords.Contains(word))
                {
                    context.MotsDictionnaire.Add(new BDMotDictionnaire
                    {
                        Id = Guid.NewGuid(),
                        Mot = word,
                        toto = ""
                    });
                    addedCount++;
                }
            }

            context.SaveChanges();
            Logger.Success($"Dictionnaire importé: {addedCount} mots ajoutés sur {words.Count}");
            
            return addedCount;
        }

        public static void ClearDictionary()
        {
            try
            {
                using var context = new YprotectContext();
                context.MotsDictionnaire.RemoveRange(context.MotsDictionnaire);
                context.SaveChanges();
                
                Logger.Success("Dictionnaire vidé");
            }
            catch (Exception ex)
            {
                Logger.Error("Erreur lors de la suppression du dictionnaire", ex);
            }
        }

        public static int GetWordCount()
        {
            try
            {
                using var context = new YprotectContext();
                return context.MotsDictionnaire.Count();
            }
            catch (Exception ex)
            {
                Logger.Error("Erreur lors du comptage des mots", ex);
                return 0;
            }
        }
    }
}