using BookAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using BooAPI.Models;

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
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };
                var tokenhandler = new JwtSecurityTokenHandler();
                var token = tokenhandler.CreateToken(tokenDescriptor);
                return tokenhandler.WriteToken(token);
            }

        }
    }
