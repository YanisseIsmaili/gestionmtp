using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Yprotect.Model;
using Yprotect.Modeles;

namespace Yprotect.Utils
{
    public static class AdminSeeder
    {
        public static void SeedSuperAdmin(YprotectContext context)
        {
            // Vérifier si un admin existe déjà
            var existingAdmin = context.Utilisateurs.FirstOrDefault(u => u.Email == "admin@yprotect.com");
            
            if (existingAdmin != null)
            {
                // Mettre à jour le rôle si nécessaire
                if (string.IsNullOrEmpty(existingAdmin.Role) || existingAdmin.Role != "Admin")
                {
                    existingAdmin.Role = "Admin";
                    context.SaveChanges();
                }
                return;
            }

            string defaultPassword = Messages.Default_Admin_Password;
            
            var superAdmin = new BDUtilisateur
            {
                Id = Guid.NewGuid(),
                Nom = "Admin",
                Prenom = "Super",
                Email = "admin@yprotect.com",
                MotDePasse = HashPassword(defaultPassword),
                Token = "",
                DateCreation = DateTime.Now,
                Role = "Admin"
            };

            context.Utilisateurs.Add(superAdmin);
            context.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}