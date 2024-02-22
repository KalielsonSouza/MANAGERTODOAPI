using Project1.Utils;
using System.ComponentModel.DataAnnotations;
namespace Project1.ViewModel
{
    public class UpdateTarefaVM
    {
        
        [Required]
        [MinLength(16)]
        public string NomeTarefa { get; set; }            
       
    }
}
