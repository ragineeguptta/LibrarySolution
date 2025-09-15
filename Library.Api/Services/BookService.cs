using AutoMapper;
using Library.Api.Data;
using Library.Api.DTOs;
using Library.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Services
{
    public interface IBookService
    {
        Task<BookDto> CreateBookAsync(CreateBookDto dto);
        Task<List<BookDto>> GetAllAsync();
        Task<BookDto?> GetByIdAsync(int id);
    }

    public class BookService : IBookService
    {
        private readonly LibraryDbContext _ctx;
        private readonly IMapper _mapper;

        public BookService(LibraryDbContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                ISBN = dto.ISBN,
                PublishedAt = dto.PublishedAt,
                TotalCopies = dto.TotalCopies,
                AvailableCopies = dto.TotalCopies
            };

            foreach (var aid in dto.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = aid });
            }

            _ctx.Books.Add(book);
            await _ctx.SaveChangesAsync();

            return _mapper.Map<BookDto>(book);
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var books = await _ctx.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .ToListAsync();
            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var book = await _ctx.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);
            return book == null ? null : _mapper.Map<BookDto>(book);
        }
    }
}
