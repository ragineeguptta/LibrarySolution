using Library.Api.Data;
using Library.Api.Entities;
using Library.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Library.Api.Services.JwtService;

namespace Library.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(LibraryDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // DTOs (you may already have them in DTOs folder)
        public record RegisterDto(string FullName, string Email, string Password);
        public record LoginDto(string Email, string Password);

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // basic validation
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password are required.");

            var exists = await _context.Borrowers.AnyAsync(b => b.Email == dto.Email);
            if (exists) return Conflict("Email already registered.");

            // Hash password with BCrypt
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var borrower = new Borrower
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = hash,
                Role = "Borrower"
            };

            _context.Borrowers.Add(borrower);
            await _context.SaveChangesAsync();

            return CreatedAtAction(null, new { id = borrower.Id, borrower.Email, borrower.FullName });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Borrowers.FirstOrDefaultAsync(b => b.Email == dto.Email);
            if (user == null) return Unauthorized("Invalid credentials.");

            // Verify password
            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return Unauthorized("Invalid credentials.");

            var token = _jwtService.GenerateToken(user.Id, user.Email, user.FullName, user.Role);

            return Ok(new
            {
                token,
                expiresAt = System.DateTime.UtcNow.AddMinutes(int.Parse(_jwtService is null ? "60" : (int.Parse(_jwtService is null ? "60" : "60").ToString()))) // placeholder, not used
            });
        }
    }
}