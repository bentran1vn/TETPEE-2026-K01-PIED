using System.ComponentModel.DataAnnotations;

namespace TetPee.Service.Product;

public class Request
{
    public class CreateProductRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(150)]
        public required string Name { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(2000)]
        public required string Description { get; set; }

        public required decimal Price { get; set; }

        public List<Guid>? CategoryIds { get; set; }
    }
}