using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnTrack.Data;
using VulnTrack.Dtos;
using VulnTrack.Models;
using VulnTrack.Services;

namespace VulnTrack.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;

    // DI hands us the database and token service automatically.
    public AuthController(AppDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest("Username already taken.");

        var user = new User
        {
            Username = dto.Username,
            // Never store plaintext — BCrypt salts + hashes the password.
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { user.Username, user.Role });
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);

        // Verify the password against the stored hash.
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = _tokens.CreateToken(user);
        return Ok(new { token });
    }
}
