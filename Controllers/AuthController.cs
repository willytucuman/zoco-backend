using BackendZocoUsers.Data;
using BackendZocoUsers.DTOs.Auth;
using BackendZocoUsers.Models;
using BackendZocoUsers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendZocoUsers.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public AuthController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
        // Endpoint para registrar un administrador
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterRequest request)
        {
            try
            {
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = Role.Admin
                };  

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var response = await _authService.LoginAsync(new LoginRequest
                {
                    Email = request.Email,
                    Password = request.Password
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userGuid = Guid.Parse(userId!);

            var lastSession = await _context.SessionLogs
                .Where(s => s.UserId == userGuid && s.EndTime == null)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();

            if (lastSession == null)
                return BadRequest(new { message = "No hay sesión activa para cerrar." });

            lastSession.EndTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sesión cerrada correctamente." });
        }


    }
}
