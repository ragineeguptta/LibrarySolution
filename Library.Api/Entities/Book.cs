namespace Library.Api.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public DateTime? PublishedAt { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    }
}
