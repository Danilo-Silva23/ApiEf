using Microsoft.AspNetCore.Mvc;//Importa os recursos necessarios para construir controller e rotas da API com o AspNetCore. EX: ControllerBase, IActionResult.
using MinhaApiEfPostgres.Models;//Importa o User e o AppDbContenxt 
using Microsoft.IdentityModel.Tokens;//Importa classes para confirar o jwt
using System.IdentityModel.Tokens.Jwt;//Importa classes para manipular e criar JWTs
using System.Security.Claims;//trata o email dentro do token
using System.Text;//importa as funcionalidades de codificação de strings

namespace MinhaApiEfPostgres.Controllers
{
    [ApiController]//diz ao aspnetcore q essa classe é um ApiController(ela tem respostas automaticas de validação, etc).
    [Route("api/[controller]")]//define a rota base
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;//define o acesso ao BD via EF
        private readonly string _jwtKey; //define o segredo do JWT

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;                                                              //Instânciando as variáveis de conexão com o BD e Chave do JWT
            _jwtKey = configuration["JwtSettings:SecretKey"];
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] User newUser)
        {
            if (_context.User.Any(u => u.Email == newUser.Email))//se qualquer valor na tabela for igual ao email da requisição retorna erro
                return BadRequest("Email já está em uso.");

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password); // Criptografa a senha

            _context.User.Add(newUser);
            _context.SaveChanges();

            return Ok("Usuário registrado com sucesso");
        }



        [HttpPost("login")]//Define o EndPoint /api/auth/login
        public IActionResult Login([FromBody] LoginRequest loginUser) //IAction é o metodo de retornno, significa q ele retorna qualquer tipo de resposta HTTP:200(ok), 401(unauthorized
        //O MÉTODO retorna o tipo IActionResult, tem o nome de Login e recebe um parametro () que fuinciona em duas partes, FromBody q informa ao AspNet que o conteudo da req vira no corpo do HTTP(body) em JSON. User é o modelo que a variavel loginUser receberá.
        {
            var user = _context.User.FirstOrDefault(u => u.Email == loginUser.Email);
            if (user == null)                                  //verifica se existe, caso não retorna 401
                return Unauthorized("Usuario ou senha inválidos");

            bool senhaValida = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password); // essa função do bcrypt retorna um bool que diz se o user do bd é igual ao da requisição .
            if (!senhaValida)//se for diferente de true(padrão) ele retorna 401
                return Unauthorized("Usuário ou senha inválidos");

            var tokenHandler = new JwtSecurityTokenHandler();//cria um token
            var key = Encoding.ASCII.GetBytes(_jwtKey);//converte a chave secreta para bytes 

            var tokenDescriptor = new SecurityTokenDescriptor// criando uma variavel do tipo tokenDescriptor 
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email)
                }),                                                                         //A variável é instânciada com informações tipo nome e email
                Expires = DateTime.UtcNow.AddHours(1),              //tempo para expirar

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),                      //coloca a secre key no token
                    SecurityAlgorithms.HmacSha256Signature                        //define que a assinatura deve ser sha256
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);  // cria o token e passa a descrição(Formato, tempo para expirar e secret key 
            var tokenString = tokenHandler.WriteToken(token);    //transorma o token em string

            return Ok(new { token = tokenString });//mostra o token no console 
        }
    }
}
