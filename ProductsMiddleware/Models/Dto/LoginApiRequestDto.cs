using System.ComponentModel.DataAnnotations;

namespace ProductsMiddleware.Models.Dto
{
    public class LoginApiRequestDto
    {
        [Required]
        public string username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required]
        public int expiresInMins { get; set; }
    }
}
