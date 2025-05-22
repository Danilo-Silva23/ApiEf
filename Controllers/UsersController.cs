using Microsoft.AspNetCore.Mvc; //importa recursos para criar controllers e endpoints
using MinhaApiEfPostgres.Models; //importa os modelos (User)
using System.Collections.Generic; //importa para usar IEnumerable
using System.Linq; //importa para usar o ToList()
using BCrypt.Net; // importa para usar BCrypt

namespace MinhaApiEfPostgres.Controllers //define que essa classe pertence ao grupo controllers
{   
    [ApiController] //diz ao AspNetCore que essa classe vai controlar requisições de API
    [Route("api/[controller]")] //define a rota da API como api/(nome do controller, nesse caso será 'users')
    public class UsersController : ControllerBase //define que a classe usercontroller é do tipo ControllerBase (tipo que é importado no começo do código), esse tipo tem funcionalidades como Ok(), NotFound() etc.
    {
        private readonly AppDbContext _context; //Cria uma variável para acessar o BD. Ser privada garante que só será usada nessa classe (só o controller deve acessar diretamente o BD). readonly significa que não será alterada.

        public UsersController(AppDbContext context) //construtor (é chamado automaticamente quando o controller é usado), inicia as variáveis da classe
        {
            _context = context;
        }

        //Mostrar todos os usuários
        [HttpGet] //é um GET em toda a tabela que for usada nas próximas linhas
        public ActionResult<IEnumerable<User>> GetUsers() //ActionResult<IEnumerable> é uma forma de armazenar e retornar os dados da requisição. GetUsers é o nome da função 
        {
            var users = _context.User
                .Select(u => new User
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Password = u.Password
                }).ToList();

            return Ok(users); //Retorna status 200 + a lista de usuários com email e senha
        }

        //Mostrar um único usuário pelo ID
        [HttpGet("{id}")] //define que o método aceita um parâmetro na URL (ex: /api/users/1)
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

            if (user == null) //se não encontrar, retorna 404
            {
                return NotFound();
            }

            return Ok(user); //se encontrar, retorna 200 com os dados
        }

       
        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
            if (_context.User.Any(u => u.Email == user.Email))
                return BadRequest("Email já está em uso.");
            // Criptografa a senha antes de salvar no banco

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.User.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        //Atualizar um usuário existente
        [HttpPut("{id}")] //requisição PUT com parâmetro ID
        public IActionResult UpdateUser(int id, User updatedUser) //updatedUser é uma variável do tipo User recebida pelo corpo da requisição
        {
            var user = _context.User.Find(id); //procura o usuário pelo ID

            if (user == null)
            {
                return NotFound(); //retorna 404 se não encontrar
            }

            user.Name = updatedUser.Name; //atualiza o nome
            user.Email = updatedUser.Email; //atualiza o email

            // Criptografa a senha nova antes de salvar
            user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

            _context.SaveChanges(); //salva no banco

            return NoContent(); //retorna 204 - sucesso sem conteúdo
        }

        //Deletar um usuário
        [HttpDelete("{id}")] //requisição DELETE com ID
        public IActionResult DeleteUser(int id)
        {
            var user = _context.User.Find(id); //procura o usuário

            if (user == null)
            {
                return NotFound(); //404 se não encontrar
            }

            _context.User.Remove(user); //remove do contexto
            _context.SaveChanges(); //salva no banco
            return NoContent(); //204 - sucesso sem conteúdo
        }
    }
}
