using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yprotect.Model
{
    public class BDUtilisateur
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Nom { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string Prenom { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string MotDePasse { get; set; } = string.Empty;
        
        public string Token { get; set; } = string.Empty;
        
        public DateTime DateCreation { get; set; }
        
        public string Role { get; set; } = "User";
    }
}