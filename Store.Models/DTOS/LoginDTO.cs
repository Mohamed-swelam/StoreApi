using System.ComponentModel.DataAnnotations;

namespace Store.Models.DTOS
{
    public class LoginDTO
    {
        public string UserName {  get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
