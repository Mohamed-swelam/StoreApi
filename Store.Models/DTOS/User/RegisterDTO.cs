using System.ComponentModel.DataAnnotations;

namespace Store.Models.DTOS.User
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
