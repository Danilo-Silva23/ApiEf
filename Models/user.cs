namespace MinhaApiEfPostgres.Models//declarando que todo o codigo que est� dentro das chaves � do models
{
    public class User//classe publica(pode ser usado em outros arquivos.)
    {
        public int Id { get; set; }//get e set declara que pode ser lida e alterada
        public  string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}