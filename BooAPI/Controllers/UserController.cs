using BooAPI.Models;
using BookAPI.Data;
using BookAPI.DTOs;
using BookAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DataDbContext _context;
        private readonly UserManager<AppUser> _appUserManager;
        private readonly ITokenGenerator _tokenGenerator;
        public UserController(DataDbContext context, UserManager<AppUser> appUserManager, 
            IConfiguration config, ITokenGenerator tokenGenerator)
        {
            _context = context;
            _appUserManager = appUserManager;
            _config = config;
            _tokenGenerator = tokenGenerator;
        }


        [HttpPost("test")]
        public async Task<ActionResult> CreateUser()
        {
            var appUser = new AppUser()
            {
                Email = "Admin@email.com",
                FirstName = "Admin",
                UserName = "Admin",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var check = await _appUserManager.FindByEmailAsync(appUser.Email);
            if(check != null)
            {
                return BadRequest(new { message = "The User is already exist" });
            }
            var result = await _appUserManager.CreateAsync(appUser, "Admin1234*");
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Something Went Wrong" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Post(LoginDTO loginDTO)
        {
            try
            {
                var user = await _appUserManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                {
                    return BadRequest(new { message = "User is not exist please register" });
                }
                var result = await _appUserManager.CheckPasswordAsync(user, loginDTO.Password);

                if (result)
                {
                    return Ok(new
                    {
                        Token = _tokenGenerator.CreateToken(user)
                    });
                }
                else
                {
                    return BadRequest(new { message="The Password doesn't match" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong" });
            }

        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegisterDTO>> Post(RegisterDTO registerDTO)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var user = new AppUser
                    {
                        UserName = registerDTO.Email,
                        FirstName = registerDTO.FirstName,
                        Email = registerDTO.Email,
                        PhoneNumber = registerDTO.PhoneNumber,
                        LastName = registerDTO.LastName,
                    };
                    var check = await _appUserManager.FindByNameAsync(registerDTO.Email);
                    if (check != null)
                    {
                        return BadRequest(new { message = "sorry the email is already used" });
                    }
                    var result = await _appUserManager.CreateAsync(user, registerDTO.Password);
                }
                return Ok(new { message = "User addedd successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }
    }
}
