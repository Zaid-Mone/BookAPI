using BookAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using BooAPI.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;

namespace BookAPI.Utility
{
    public class TokenGenerator:ITokenGenerator
    {
            private readonly IConfiguration _config;
        public TokenGenerator(IConfiguration config)
        {
            _config = config;

        }
        public string CreateToken(AppUser user)
            {
                var claims = new List<Claim>
            {
                 new Claim("Id",user.Id.ToString()),
                 new Claim("FirstName",user.FirstName),
                 new Claim("LastName",user.LastName),
                 new Claim(JwtRegisteredClaimNames.Email,user.Email)
            };
                var key = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(_config.GetSection("secret_Key").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Audience = "localhost",
                    Issuer = "localhost",
                    Expires = DateTime.Now.AddSeconds(10),
                    SigningCredentials = creds
                };
                var tokenhandler = new JwtSecurityTokenHandler();
                var token = tokenhandler.CreateToken(tokenDescriptor);
                return tokenhandler.WriteToken(token);
            }
        public bool ValidateToken(string token="")
        {
            try
            {
                token = token.Replace("Bearer ", "");
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Check if principal.Identity is not null and authenticated
                return principal?.Identity?.IsAuthenticated == true;
            }
            catch (SecurityTokenException ex)
            {
                // Handle token-related errors, such as invalid signature or expired token
                // Log the error or handle it appropriately
                return false;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                // Log the error or handle it appropriately
                return false;
            }
        }

    
        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b856debcf54d42a8bec1757c9c91b622"))
            };
        }
    }
    }
