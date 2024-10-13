using System.ComponentModel.DataAnnotations;

namespace Store.Models.DTOS.User
{
    public class LoginDTO
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
