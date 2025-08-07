using BackendZocoUsers.Data;
using BackendZocoUsers.DTOs.Auth;
using BackendZocoUsers.Helpers;
using BackendZocoUsers.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendZocoUsers.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("El email ya está registrado.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = Role.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Credenciales inválidas.");

            var token = _jwtHelper.GenerateToken(user);
            var session = new SessionLog
            {
                UserId = user.Id,
                StartTime = DateTime.UtcNow
            };

            _context.SessionLogs.Add(session);
            await _context.SaveChangesAsync();
            return new AuthResponse
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
