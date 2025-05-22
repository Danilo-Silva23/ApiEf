using Microsoft.AspNetCore.Mvc; //importa recursos para criar controllers e endpoints
using MinhaApiEfPostgres.Models; //importa os modelos (User)
using System.Collections.Generic; //importa para usar IEnumerable
using System.Linq; //importa para usar o ToList()
using BCrypt.Net; // importa para usar BCrypt

namespace MinhaApiEfPostgres.Controllers //define que essa classe pertence ao grupo controllers
{   
    [ApiController] //diz ao AspNetCore que essa classe vai controlar requisi��es de API
    [Route("api/[controller]")] //define a rota da API como api/(nome do controller, nesse caso ser� 'users')
    public class UsersController : ControllerBase //define que a classe usercontroller � do tipo ControllerBase (tipo que � importado no come�o do c�digo), esse tipo tem funcionalidades como Ok(), NotFound() etc.
    {
        private readonly AppDbContext _context; //Cria uma vari�vel para acessar o BD. Ser privada garante que s� ser� usada nessa classe (s� o controller deve acessar diretamente o BD). readonly significa que n�o ser� alterada.

        public UsersController(AppDbContext context) //construtor (� chamado automaticamente quando o controller � usado), inicia as vari�veis da classe
        {
            _context = context;
        }

        //Mostrar todos os usu�rios
        [HttpGet] //� um GET em toda a tabela que for usada nas pr�ximas linhas
        public ActionResult<IEnumerable<User>> GetUsers() //ActionResult<IEnumerable> � uma forma de armazenar e retornar os dados da requisi��o. GetUsers � o nome da fun��o 
        {
            var users = _context.User
                .Select(u => new User
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Password = u.Password
                }).ToList();

            return Ok(users); //Retorna status 200 + a lista de usu�rios com email e senha
        }

        //Mostrar um �nico usu�rio pelo ID
        [HttpGet("{id}")] //define que o m�todo aceita um par�metro na URL (ex: /api/users/1)
        public ActionResult<User> GetUser(int id)
        {
            var user = _context.User
                .Where(u => u.Id == id)
                .Select(u => new User
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Password = u.Password
                })
                .FirstOrDefault();

            if (user == null) //se n�o encontrar, retorna 404
            {
                return NotFound();
            }

            return Ok(user); //se encontrar, retorna 200 com os dados
        }

       
        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
            if (_context.User.Any(u => u.Email == user.Email))
                return BadRequest("Email j� est� em uso.");
            // Criptografa a senha antes de salvar no banco

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.User.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        //Atualizar um usu�rio existente
        [HttpPut("{id}")] //requisi��o PUT com par�metro ID
        public IActionResult UpdateUser(int id, User updatedUser) //updatedUser � uma vari�vel do tipo User recebida pelo corpo da requisi��o
        {
            var user = _context.User.Find(id); //procura o usu�rio pelo ID

            if (user == null)
            {
                return NotFound(); //retorna 404 se n�o encontrar
            }

            user.Name = updatedUser.Name; //atualiza o nome
            user.Email = updatedUser.Email; //atualiza o email

            // Criptografa a senha nova antes de salvar
            user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

            _context.SaveChanges(); //salva no banco

            return NoContent(); //retorna 204 - sucesso sem conte�do
        }

        //Deletar um usu�rio
        [HttpDelete("{id}")] //requisi��o DELETE com ID
        public IActionResult DeleteUser(int id)
        {
            var user = _context.User.Find(id); //procura o usu�rio

            if (user == null)
            {
                return NotFound(); //404 se n�o encontrar
            }

            _context.User.Remove(user); //remove do contexto
            _context.SaveChanges(); //salva no banco
            return NoContent(); //204 - sucesso sem conte�do
        }
    }
}
