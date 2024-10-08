using System.ComponentModel.DataAnnotations;

namespace Store.Models.DTOS.User
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
