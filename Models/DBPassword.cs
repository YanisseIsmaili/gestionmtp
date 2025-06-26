using System;
using System.ComponentModel.DataAnnotations;

namespace Yprotect.Model
{
    public class BDPassword
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string Site { get; set; } = string.Empty;
        
        [Required]
        public string NomUtilisateur { get; set; } = string.Empty;
        
        [Required]
        public string MotDePasseChiffre { get; set; } = string.Empty;
        
        public DateTime DateCreation { get; set; }
    }
}