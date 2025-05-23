﻿using ApplicationLayer.Common.Interfaces.JwtToken;
using ApplicationLayer.Common.Services;
using DomainLayer.UserAggregate;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InfrastructureLayer.Implementations.JwtToken
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly JwtSettings _jwtSettings;
        public JwtTokenGenerator(IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtSettingsOptions)
        {
            _dateTimeProvider = dateTimeProvider;
            _jwtSettings = jwtSettingsOptions.Value;
        }
        public string GenerateToken(User user)
        {
            var siginingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.Secret)), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: _dateTimeProvider.Now.AddMinutes(_jwtSettings.ExpiresIn),
                claims: claims,
                signingCredentials: siginingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
