
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1.Models
{
    public class Tarefa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }
        public TaskStatus Status { get; set; }

        public DateTime DataCriacao { get; set; }        
        public Guid Dono { get; set; }
    }
}
