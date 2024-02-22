
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }      
        public string FullName { get; set; }        
        public string Email { get; set; }
        public string Password { get; set; }
        public string PermissionType { get; set; }
    }
}
