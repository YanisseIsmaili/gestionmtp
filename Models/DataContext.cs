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
    class YprotectContext : DbContext
    {   /// <summary>
        /// Initializes a new instance of the <see cref="YprotectContext"/> class.
        /// </summary>
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configure the context to use SQLite with a specific database file
                optionsBuilder.UseSqlite("Data Source=yprotect.db");
            }
        }

        public DbSet<BDUtilisateur> Utilisateurs { get; set; }
    }
    
}