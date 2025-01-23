using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDeployReservas.Controllers.Dtos.Account;
using ApiDeployReservas.Data.Models;
using ApiDeployReservas.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiDeployReservas.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _UserMananger;

        private readonly ItokenService _tokenService;

        private readonly SignInManager<AppUser> _signInManager;

        public AccountController (UserManager<AppUser> userManager, ItokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _UserMananger = userManager;

            _tokenService = tokenService;

            _signInManager =signInManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login (LoginDto loginDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
         var user = await _UserMananger.Users.FirstOrDefaultAsync( U => U.UserName == loginDto.Username.ToLower());
         if(user == null) return Unauthorized("Usuario Invalido!");

         var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

         if(!result.Succeeded) return Unauthorized("Usuario o contrase�a incorrecta");

         return Ok(
            new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token =  _tokenService.CreateToken(user)
            }
         );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register ([FromBody] RegisterDto register)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _UserMananger.Users.FirstOrDefaultAsync(U => U.UserName == register.Username.ToLower());
            if (user != null) return BadRequest("El nombre de usuario ya est� en uso!");

            var email = await _UserMananger.Users.FirstOrDefaultAsync(U => U.Email.ToLower() == register.Email.ToLower());
            if (email != null) return BadRequest("Este correo ya est� en uso!");

            try
            {

                var appUser = new AppUser
                {
                    UserName = register.Username,
                    Email = register.Email
                };

                var createdUser = await _UserMananger.CreateAsync(appUser,register.Password);

                if(createdUser.Succeeded)
                {
                    var roleResult = await _UserMananger.AddToRoleAsync(appUser,"User");
                    if(roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                    UserName = appUser.UserName,
                                    Email = appUser.Email,    
                                Token =  _tokenService.CreateToken(appUser)

                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch(Exception e)
            {
                 return StatusCode(500, e);
            }
        }
        
    }
}