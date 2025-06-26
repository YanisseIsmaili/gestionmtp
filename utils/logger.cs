using System;
using System.IO;

namespace Yprotect.Utils
{
    public static class Logger
    {
        private static readonly string LogFilePath;
        private static readonly object LockObject = new object();

        static Logger()
        {
            // Créer le fichier de log dans le répertoire de l'application
            var fileName = $"yprotect_{DateTime.Now:yyyy-MM-dd}.log";
            LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        /// <summary>
        /// Log d'information
        /// </summary>
        public static void Info(string message)
        {
            WriteLog("INFO", message);
        }

        /// <summary>
        /// Log d'erreur
        /// </summary>
        public static void Error(string message)
        {
            WriteLog("ERROR", message);
        }

        /// <summary>
        /// Log d'erreur avec exception
        /// </summary>
        public static void Error(string message, Exception ex)
        {
            WriteLog("ERROR", $"{message} | {ex.Message}");
        }

        /// <summary>
        /// Log de succès
        /// </summary>
        public static void Success(string message)
        {
            WriteLog("SUCCESS", message);
        }

        /// <summary>
        /// Log pour la base de données
        /// </summary>
        public static void Database(string message)
        {
            WriteLog("DB", message);
        }

        /// <summary>
        /// Écriture dans console + fichier
        /// </summary>
        private static void WriteLog(string level, string message)
        {
            var logEntry = $"[{level}] {DateTime.Now:HH:mm:ss} - {message}";
            
            // Afficher dans la console
            Console.WriteLine(logEntry);
            
            // Écrire dans le fichier
            try
            {
                lock (LockObject)
                {
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
                }
            }
            catch
            {
                // Ignorer les erreurs de fichier pour ne pas planter l'app
            }
        }
    }
}
