using Library.Api.Data;
using Library.Api.DTOs;
using Library.Api.Entities;
using Library.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext _ctx;

        public LoansController(LibraryDbContext ctx) => _ctx = ctx;

        [HttpPost("issue")]
        [Authorize]
        public async Task<IActionResult> Issue([FromBody] IssueRequestDto dto)
        {
            var borrowerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var book = await _ctx.Books.FindAsync(dto.BookId);
            if (book == null) return NotFound("Book not found");
            if (book.AvailableCopies <= 0) return BadRequest("No copies available");

            book.AvailableCopies -= 1;
            var loan = new Loan
            {
                BookId = book.Id,
                BorrowerId = borrowerId,
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(dto.Days ?? 14)
            };

            _ctx.Loans.Add(loan);
            await _ctx.SaveChangesAsync();
            return Ok(new { loan.Id, loan.DueDate });
        }

        [HttpPost("{loanId}/return")]
        [Authorize]
        public async Task<IActionResult> Return(int loanId)
        {
            var loan = await _ctx.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == loanId);
            if (loan == null) return NotFound();
            if (loan.IsReturned) return BadRequest("Already returned");

            loan.ReturnDate = DateTime.UtcNow;
            loan.Book.AvailableCopies += 1;

            // optional fine
            int finePerDay = 10; // e.g. Rs 10
            int daysLate = (loan.ReturnDate.Value.Date - loan.DueDate.Date).Days;
            var fine = daysLate > 0 ? daysLate * finePerDay : 0;

            await _ctx.SaveChangesAsync();
            return Ok(new { loan.Id, returnedAt = loan.ReturnDate, fine });
        }
    }
}
