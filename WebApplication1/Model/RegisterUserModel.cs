using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class RegisterUserModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
