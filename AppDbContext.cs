using Microsoft.EntityFrameworkCore; //importa o DbContext que faz a conexão com o BD
using MinhaApiEfPostgres.Models;//importa os modelos

namespace MinhaApiEfPostgres    //define o espaço principal do projeto(Main), é onde vai ficar a classe que representa a conexão com o DB
{
    public class AppDbContext : DbContext//diz que a classe AppDbContext é d otipo Dbcontext(do import)
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { //construtor da classe recebe o options e envia para a Dbcontext por meio do base(options)
        }
        public DbSet<User> User { get; set; }
        
    }
}