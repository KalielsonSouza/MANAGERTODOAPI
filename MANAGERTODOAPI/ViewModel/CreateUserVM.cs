using Project1.Utils;
using System.ComponentModel.DataAnnotations;
namespace Project1.ViewModel
{
    public class CreateUserVM
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
       
    }
}
