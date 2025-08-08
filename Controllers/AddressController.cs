using BackendZocoUsers.Data;
using BackendZocoUsers.DTOs.Address;
using BackendZocoUsers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendZocoUsers.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        // 👤 Obtener todas mis direcciones (o todas si soy admin)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var query = _context.Addresses.AsQueryable();

            if (role != "Admin")
            {
                query = query.Where(a => a.UserId.ToString() == userId);
            }

            var addresses = await query.ToListAsync();
            return Ok(addresses);
        }

        // ✅ Crear una nueva dirección
        [HttpPost]
        public async Task<IActionResult> Create(Address address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Admin")
            {
                address.UserId = Guid.Parse(userId!);
            }
            else
            {
                if (address.UserId == Guid.Empty) return BadRequest(new { error = "UserId es requerido para Admin" });
                var exists = await _context.Users.AnyAsync(u => u.Id == address.UserId);
                if (!exists) return BadRequest(new { error = "UserId no existe" });
            }

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return Ok(address);
        }


        // ✏️ Editar dirección (actualización parcial)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressRequest updated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();

            if (role != "Admin" && address.UserId.ToString() != userId)
                return Forbid();

            if (!string.IsNullOrWhiteSpace(updated.Street))
                address.Street = updated.Street;

            if (!string.IsNullOrWhiteSpace(updated.City))
                address.City = updated.City;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ❌ Eliminar dirección
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();

            if (role != "Admin" && address.UserId.ToString() != userId)
                return Forbid();

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
