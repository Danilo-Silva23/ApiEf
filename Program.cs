using Microsoft.EntityFrameworkCore; //Importa o EF Core 
using MinhaApiEfPostgres; //Importa tudo do projeto (inclusive AppDbContext)
using Microsoft.AspNetCore.Authentication.JwtBearer; //Para usar autenticação baseada em JWT
using Microsoft.IdentityModel.Tokens; //Para configurar as regras de validação do Token
using System.Text; //Para usar Encoding.UTF8.GetBytes(...).

var builder = WebApplication.CreateBuilder(args); //Inicializa a aplicação ASP.NET e lê configurações(args) como o appsettings.json

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //Lê a string de conexão do appsettings.json


builder.Services.AddDbContext<AppDbContext>(options =>// Adiciona o DbContext e informa que vai usar PostgreSQL com a string de conexão
    options.UseNpgsql(connectionString)
);


builder.Services.AddControllers();// Adiciona o suporte a Controllers (arquivos que respondem as requisições da API)

// Pega a chave do token no appsettings.json em JwtSettings:SecretKey
var key = builder.Configuration["JwtSettings:SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //Define JWT como padrão de autenticação
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, //não validar o emissor
        ValidateAudience = false, //não validar o público
        ValidateLifetime = true, //valida se não está expirado
        ValidateIssuerSigningKey = true, //garante que a assinatura é válida
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) //Define a chave usada para validar o token
    };
});

var app = builder.Build(); //Constroi a aplicação com as configurações anteriores

app.UseHttpsRedirection(); //Redireciona HTTP para HTTPS

app.UseAuthentication(); // Ativa o middleware de autenticação (obrigatório ANTES do Authorization)
app.UseAuthorization(); //Ativa o middleware de autorização (verifica se o usuário pode acessar a rota)

app.MapControllers(); //Indica que vai usar os controllers para lidar com as rotas

app.Run(); //Roda a aplicação
