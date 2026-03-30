using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QueueManagement.Domain.Entities.DTOs;
using QueueManagement.Domain.Entities;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using QueueManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Options;


public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtTokenGenerator> _logger;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings, ILogger<JwtTokenGenerator> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }


    public async Task<AuthResponseDto> GenerateTokenAsync(Guid userId, string email, IList<string> roles)
    {
        try
        {
            // Validate
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty");

            var claims = CreateClaims(userId, email, roles);
            var credentials = CreateSigningCredentials();
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Token generated successfully for user: {Email}", email);

            return new AuthResponseDto
            {
                AccessToken = tokenString,
                RefreshToken = GenerateRefreshToken(),
                ExpiresAt = expires,
                ExpiresIn = (int)TimeSpan.FromMinutes(_jwtSettings.ExpiryMinutes).TotalSeconds,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user: {Email}", email);
            throw;
        }
    }

    

    private List<Claim> CreateClaims(Guid userId, string email, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}