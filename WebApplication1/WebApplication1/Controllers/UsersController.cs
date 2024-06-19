using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.DTOs.User;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;

        public UsersController(ITokenService tokenService, AppDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }


        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto dto)
        {
            var isEmailTaken = await _context.Users.AnyAsync(u => u.Email.Equals(dto.Email, StringComparison.CurrentCultureIgnoreCase));
            if (isEmailTaken) return Conflict("Email is already taken");

            var user = new User
            {
                Email = dto.Email
            };
            user.HashedPassword = new PasswordHasher<User>().HashPassword(user, dto.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(dto.Email, StringComparison.CurrentCultureIgnoreCase));
            if (user == null) return Unauthorized();

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, dto.Password);
            if (result == PasswordVerificationResult.Failed) return Unauthorized();

            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken.token;
            user.RefreshTokenExpiration = refreshToken.expiration;

            await _context.SaveChangesAsync();

            var response = new TokenResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken.token,
            };

            return Ok(response);
        }


        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto dto)
        {
            var principal = _tokenService.ValidateAndGetPrincipalFromJwt(dto.AccessToken, false);
            if (principal == null) return Unauthorized();

            var claimIdUser = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimIdUser == null || !int.TryParse(claimIdUser, out var userId))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiration < DateTime.UtcNow)
                return Unauthorized();

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken.token;
            user.RefreshTokenExpiration = newRefreshToken.expiration;

            await _context.SaveChangesAsync();

            var response = new TokenResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = newRefreshToken.token,
            };

            return Ok(response);
        }
    }
}