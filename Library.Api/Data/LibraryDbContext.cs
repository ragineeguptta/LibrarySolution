using Library.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Author { get; set; } = null!;
        public DbSet<BookAuthor> BookAuthor { get; set; } = null!;
        public DbSet<Borrower> Borrowers { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookAuthor>().HasKey(ba => new { ba.BookId, ba.AuthorId });
            modelBuilder.Entity<Borrower>().HasIndex(b => b.Email).IsUnique();

            modelBuilder.Entity<Borrower>().HasData(
                new Borrower { Id = 1, FullName="Admin User", Email = "admin@library.local", PasswordHash = "REPLACE_WITH_HASH", Role="Admin"}
                );
        }
    }
}
