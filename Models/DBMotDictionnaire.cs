using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yprotect.Utils;

namespace Yprotect.Model
{
    public class BDMotDictionnaire
    {
        [Key]
        [Required(ErrorMessage = Messages.General_Validation_IDMotDictionnaire_Required)]

        public Guid Id { get; set; }

        [Required(ErrorMessage = Messages.General_Validation_MotDictionnaire_Required)]
        [StringLength(50, ErrorMessage = Messages.General_Validation_MotDictionnaire_StringLength)]
        public string Mot { get; set; } = string.Empty;
        
        public string toto { get; set; } = string.Empty;
    }
}