using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreditGuardAPI.Data;
using CreditGuardAPI.Models;
using CreditGuardAPI.Services;

namespace CreditGuardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenService _tokenService;

        public AuthController(ApplicationDbContext context, JwtTokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
                SecretQuestion = registerDto.SecretQuestion,
                SecretAnswer = registerDto.SecretAnswer
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !PasswordHasher.VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = _tokenService.GenerateJwtToken(user);

            return Ok(new { Token = token, Role = user.Role });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == forgotPasswordDto.Username);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Verify secret answer
            if (user.SecretAnswer.ToLower() != forgotPasswordDto.SecretAnswer.ToLower())
            {
                return BadRequest("Incorrect secret answer");
            }

            // Update password
            user.PasswordHash = PasswordHasher.HashPassword(forgotPasswordDto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("Password reset successfully");
        }
    }
}
