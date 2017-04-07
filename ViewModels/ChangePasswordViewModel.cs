using System.ComponentModel.DataAnnotations;

namespace LetMeKnowApi.ViewModels 
{
    public class ChangePasswordViewModel
    {  
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres de longitud.", MinimumLength = 6)]
        [Display(Name = "Antigua Contraseña")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres de longitud.", MinimumLength = 6)]
        [Display(Name = "Nueva Contraseña")]   
        public string NewPassword { get; set; }
        
        [Compare("NewPassword", ErrorMessage = "La contraseña y la confirmación de la contraseña no coinciden.")]
        public string ConfirmPassword { get; set; }        
    }
}
