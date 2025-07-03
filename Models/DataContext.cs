using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yprotect.Model;
using System.IO;
using Yprotect.Modeles;

namespace Yprotect.Modeles
{
    public class YprotectContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "yprotect.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        public DbSet<BDUtilisateur> Utilisateurs { get; set; }
        public DbSet<BDMotDictionnaire> MotsDictionnaire { get; set; }
        public DbSet<BDPassword> Passwords { get; set; }
    }
}