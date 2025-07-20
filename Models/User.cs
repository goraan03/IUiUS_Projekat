namespace IUiUS_Projekat.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public User() { }

        public User(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }
}
