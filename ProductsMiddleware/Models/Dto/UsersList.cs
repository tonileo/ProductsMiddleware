using ProductsMiddleware.Models.Domain;

namespace ProductsMiddleware.Models.Dto
{
    public class UsersList
    {
        public List<LoginRequestDto> Users { get; set; }
    }
}
