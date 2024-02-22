using Microsoft.EntityFrameworkCore;
using Project1.Models;

namespace Project1.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer("Server=SOFRIMENTO;Database=GerenciaTodo;User Id=Kali;Password=Souza04;TrustServerCertificate=True;");
        }
    }
}