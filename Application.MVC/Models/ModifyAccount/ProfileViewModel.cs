using System.ComponentModel.DataAnnotations;

namespace Application.MVC.Models.ModifyAccount
{
    public class ProfileViewModel
    {
        [Display(Name = "Email actual")]
        
        public string CurrentEmail { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } 

        [Required]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        // Solo lectura
        public string Email { get; set; }


        
    }
}
