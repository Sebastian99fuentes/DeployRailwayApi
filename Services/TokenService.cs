using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiDeployReservas.Data.Models;
using ApiDeployReservas.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ApiDeployReservas.Services
{
  
    public class TokenService :ItokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly string Issuer_;
        private readonly string Audience_;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
            Issuer_ = _config["Jwt:Issuer"] ;
            Audience_ = _config["Jwt:Audience"];
        }

       public string CreateToken(AppUser user)
       {

         var claims = new List<Claim>
          {
          //  new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "defaultEmail@example.com"),
           new Claim(JwtRegisteredClaimNames.GivenName,  user.UserName ?? ""),
           new Claim("userId", user.Id.ToString()), // Aquí se agrega el UserId como un claim adicional
            // new Claim(ClaimTypes.Role, GetUserRolesAsync(user).Result) // Usa GetUserRolesAsync
          };

          // Usar HmacSha512Signature para la firma
           var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

           var tokenDescriptor = new SecurityTokenDescriptor
          {
               Subject = new ClaimsIdentity(claims),
               Expires = DateTime.Now.AddDays(7),
               Issuer = Issuer_,
               Audience = Audience_,
               SigningCredentials = creds 
          };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
          return tokenHandler.WriteToken(token); // Esto debería generar un token firmado
        }

    }
}