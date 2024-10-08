using System.ComponentModel.DataAnnotations;

namespace Store.Models.DTOS.User
{
    public class LoginDTO
    {
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
