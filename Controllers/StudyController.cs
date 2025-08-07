using BackendZocoUsers.Data;
using BackendZocoUsers.DTOs.Study;
using BackendZocoUsers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendZocoUsers.Controllers
{
    [ApiController]
    [Route("api/studies")]
    [Authorize]
    public class StudyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudyController(AppDbContext context)
        {
            _context = context;
        }

        // 👤 Ver mis estudios (o todos si soy Admin)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var query = _context.Studies.AsQueryable();

            if (role != "Admin")
            {
                query = query.Where(s => s.UserId.ToString() == userId);
            }

            var studies = await query.ToListAsync();
            return Ok(studies);
        }

        // ✅ Crear un nuevo estudio
        [HttpPost]
        public async Task<IActionResult> Create(Study study)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Admin")
            {
                study.UserId = Guid.Parse(userId!);
            }

            _context.Studies.Add(study);
            await _context.SaveChangesAsync();
            return Ok(study);
        }

        // ✏️ Editar un estudio
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudyRequest updated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var study = await _context.Studies.FindAsync(id);
            if (study == null) return NotFound();

            if (role != "Admin" && study.UserId.ToString() != userId)
                return Forbid();

            // ✅ Solo actualiza si viene algo
            if (!string.IsNullOrWhiteSpace(updated.Title))
                study.Title = updated.Title;

            if (!string.IsNullOrWhiteSpace(updated.Institution))
                study.Institution = updated.Institution;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // ❌ Borrar un estudio
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var study = await _context.Studies.FindAsync(id);
            if (study == null) return NotFound();

            if (role != "Admin" && study.UserId.ToString() != userId)
                return Forbid();

            _context.Studies.Remove(study);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
