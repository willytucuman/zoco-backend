using BackendZocoUsers.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendZocoUsers.Controllers
{
    [ApiController]
    [Route("api/session-logs")]
    [Authorize(Roles = "Admin")]
    public class SessionLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SessionLogController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _context.SessionLogs
                .Include(s => s.User)
                .OrderByDescending(s => s.StartTime)
                .Select(s => new
                {
                    s.Id,
                    s.UserId,
                    UserName = s.User!.Name,
                    s.StartTime,
                    s.EndTime
                })
                .ToListAsync();

            return Ok(logs);
        }
    }
}
