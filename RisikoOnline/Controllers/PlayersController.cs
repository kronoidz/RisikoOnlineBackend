using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RisikoOnline.Data;
using RisikoOnline.Services;

namespace RisikoOnline.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        private static readonly Regex NameRegex = new Regex(@"^\w\w\w+$", RegexOptions.Compiled);
        private static readonly Regex PasswordRegex = new Regex(@"^....+$", RegexOptions.Compiled);

        private AppDbContext _dbContext;
        private ConfigurationService _configuration;

        public PlayersController(AppDbContext dbContext, ConfigurationService configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public class PlayerPostRequest
        {
            [Required] public string Name { get; set; }
            [Required] public string Password { get; set; }
        }

        public class PlayerAuthResponse
        {
            public string Token { get; set; }
        }

        private string GetPasswordHash(string password, string salt)
        {
            var sha = new SHA256Managed();
            var hashInput = password + _configuration.PasswordSecret + salt;
            var hashRaw = sha.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
            return Convert.ToHexString(hashRaw).ToLower();
        }

        [HttpPost]
        public async Task<ActionResult> PostPlayer([FromBody] PlayerPostRequest request)
        {
            if (await _dbContext.Players.FindAsync(request.Name) != null)
                return Conflict(new ApiError(ApiErrorType.PlayerNameConflict));
            
            if (!NameRegex.IsMatch(request.Name) || !PasswordRegex.IsMatch(request.Password))
                return BadRequest(new ApiError(ApiErrorType.InvalidCredentials));

            string salt = Guid.NewGuid().ToString();
            var newPlayer = new Player
            {
                Name = request.Name,
                PasswordHash = GetPasswordHash(request.Password, salt),
                PasswordSalt = salt
            };

            _dbContext.Players.Add(newPlayer);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("auth")]
        public async Task<ActionResult<PlayerAuthResponse>> Authenticate([FromBody] PlayerPostRequest request)
        {
            Player player = await _dbContext.Players.FindAsync(request.Name);
            if (player == null)
                return Unauthorized(new ApiError(ApiErrorType.InvalidCredentials));

            string hash = GetPasswordHash(request.Password, player.PasswordSalt);
            if (player.PasswordHash != hash)
                return Unauthorized(new ApiError(ApiErrorType.InvalidCredentials));
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.TokenSigningSecret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.Name, player.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new PlayerAuthResponse {Token = tokenHandler.WriteToken(token)};
        }
    }
}