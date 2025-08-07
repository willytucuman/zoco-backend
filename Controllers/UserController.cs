using BackendZocoUsers.Data;
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

        // ✏️ Editar datos propios
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe(User updateRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(Guid.Parse(userId!));
            if (user == null) return NotFound();

            user.Name = updateRequest.Name;
            user.Email = updateRequest.Email;

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
    }
}
