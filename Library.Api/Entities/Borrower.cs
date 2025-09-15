namespace Library.Api.Entities
{
    public class Borrower
    {
        public int Id { get; set; } 
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!; // unique
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!; // Borrower, Librarian, Admin

    }
}
