using System.ComponentModel.DataAnnotations;

namespace ProductsMiddleware.Models.Dto
{
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}
