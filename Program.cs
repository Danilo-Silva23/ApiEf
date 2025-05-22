using Microsoft.EntityFrameworkCore; //Importa o EF Core 
using MinhaApiEfPostgres; //Importa tudo do projeto (inclusive AppDbContext)
using Microsoft.AspNetCore.Authentication.JwtBearer; //Para usar autentica��o baseada em JWT
using Microsoft.IdentityModel.Tokens; //Para configurar as regras de valida��o do Token
using System.Text; //Para usar Encoding.UTF8.GetBytes(...).

var builder = WebApplication.CreateBuilder(args); //Inicializa a aplica��o ASP.NET e l� configura��es(args) como o appsettings.json

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //L� a string de conex�o do appsettings.json


builder.Services.AddDbContext<AppDbContext>(options =>// Adiciona o DbContext e informa que vai usar PostgreSQL com a string de conex�o
    options.UseNpgsql(connectionString)
);


builder.Services.AddControllers();// Adiciona o suporte a Controllers (arquivos que respondem as requisi��es da API)

// Pega a chave do token no appsettings.json em JwtSettings:SecretKey
var key = builder.Configuration["JwtSettings:SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //Define JWT como padr�o de autentica��o
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, //n�o validar o emissor
        ValidateAudience = false, //n�o validar o p�blico
        ValidateLifetime = true, //valida se n�o est� expirado
        ValidateIssuerSigningKey = true, //garante que a assinatura � v�lida
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) //Define a chave usada para validar o token
    };
});

var app = builder.Build(); //Constroi a aplica��o com as configura��es anteriores

app.UseHttpsRedirection(); //Redireciona HTTP para HTTPS

app.UseAuthentication(); // Ativa o middleware de autentica��o (obrigat�rio ANTES do Authorization)
app.UseAuthorization(); //Ativa o middleware de autoriza��o (verifica se o usu�rio pode acessar a rota)

app.MapControllers(); //Indica que vai usar os controllers para lidar com as rotas

app.Run(); //Roda a aplica��o
