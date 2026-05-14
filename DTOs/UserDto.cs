namespace SimpleEcoms.DTOs
{
    public class RegisterDto
    {        
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Identifier { get; set; } = string.Empty; // email OR phone
        public string Password { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}