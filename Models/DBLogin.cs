using System;
using System.ComponentModel.DataAnnotations;

namespace Yprotect.Model
{
    public class BDLogin
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string NomUtilisateur { get; set; } = string.Empty;
        
        [Required]
        public string MotDePasseHash { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        public DateTime DateCreation { get; set; }
    }
}