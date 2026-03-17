namespace TetPee.Service.Category;

public class Response
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid? ParentId { get; set; }
    }
}