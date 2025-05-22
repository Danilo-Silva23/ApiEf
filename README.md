# 📆 MinhaApiEfPostgres - Passo a Passo para Execução

Este é um projeto de API RESTful criado com ASP.NET Core, utilizando Entity Framework Core para acesso ao banco de dados PostgreSQL.

---

## ✅ Requisitos

* .NET SDK instalado (recomenda-se a versão 6 ou superior)
* PostgreSQL instalado e rodando
* DBeaver ou outro cliente para banco de dados (opcional)
* Postman ou Insomnia para testar a API (opcional)

---

## ⚙️ Passos para Executar

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/MinhaApiEfPostgres.git
cd MinhaApiEfPostgres
```

### 2. Configure a string de conexão

Abra o arquivo `appsettings.json` e adicione:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=minhaapi;Username=postgres;Password=1234"
  }
}
```

> Obs: Altere os dados conforme sua instalação local.

### 3. Instale os pacotes necessários

```bash
dotnet add package BCrypt.Net-Next

dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer


dotnet add package System.IdentityModel.Tokens.Jwt

```bash
dotnet add package Microsoft.EntityFrameworkCore
```

```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
```

```bash
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### 4. Gerar as Migrations

```bash
dotnet ef migrations add CriarTabelaUsuario
```

### 5. Aplicar a Migration ao banco

```bash
dotnet ef database update
```

### 6. Executar a aplicação

```bash
dotnet run
```

A aplicação estará rodando em `http://localhost:5047` (verifique no console a porta correta).

---

## 📊 Testes

Use Postman ou outro cliente HTTP para testar os seguintes endpoints:

* `GET /api/users` – Lista todos os usuários
* `GET /api/users/{id}` – Retorna um usuário específico
* `POST /api/users` – Cria um novo usuário
* `PUT /api/users/{id}` – Atualiza um usuário
* `DELETE /api/users/{id}` – Remove um usuário

---

## 💭 Dicas

* Se tiver erro ao aplicar migrations, verifique a string de conexão.
* Para ver logs de erros, observe a saída do console.
* O DBeaver pode ser usado para visualizar e manipular os dados no banco PostgreSQL.

---

Pronto! Agora é só codar ✌️
