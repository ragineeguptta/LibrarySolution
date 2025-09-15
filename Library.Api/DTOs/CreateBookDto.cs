namespace Library.Api.DTOs
{
    public class CreateBookDto
    {
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public DateTime? PublishedAt { get; set; }
        public int TotalCopies { get; set; }
        public List<int> AuthorIds { get; set; } = new();
    }
}
