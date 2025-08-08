using BackendZocoUsers.Data;
using BackendZocoUsers.DTOs.Users;
using BackendZocoUsers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendZocoUsers.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // 🔒 Solo admins pueden ver todos los usuarios
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Name, u.Email, u.Role })
                .ToListAsync();

            return Ok(users);
        }

        // 👤 Obtener datos del usuario actual
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Where(u => u.Id.ToString() == userId)
                .Select(u => new { u.Id, u.Name, u.Email, u.Role })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateMeRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(Guid.Parse(userId!));
            if (user == null) return NotFound();

            // Si envían email, opcional: validar que no esté usado por otro
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var exists = await _context.Users
                    .AnyAsync(u => u.Email == request.Email && u.Id != user.Id);
                if (exists) return BadRequest(new { error = "El email ya está en uso." });
                user.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                user.Name = request.Name;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ❌ Borrar un usuario (solo admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // crear un usuario (solo admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            if (await _context.Users.AnyAsync(u => u.Email == req.Email))
                return BadRequest(new { error = "El email ya está registrado." });

            Role roleParsed = Role.User;
            if (!Enum.TryParse<Role>(req.Role, true, out roleParsed))
                return BadRequest(new { error = "Rol inválido (use Admin o User)." });

            var user = new User
            {
                Name = req.Name,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = roleParsed
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Name, user.Email, Role = user.Role.ToString() });
        }
    }
}
