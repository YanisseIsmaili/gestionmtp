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
        public string Nom { get; set; }
        
        [StringLength(10)]
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Token { get; set; }
        public DateTime DateCreation { get; set; }

    }
}