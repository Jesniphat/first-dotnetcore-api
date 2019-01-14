using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FirstDotNetCore.Models;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace FirstDotNetCore
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsController : ControllerBase
    { 
        private readonly IConfiguration _configuration;
        public AppDb db;
        public LoginsController(AppDb DB, IConfiguration configuration)
        {
            // Use AppDb instance service
            this.db = DB;
            this._configuration = configuration;
        }


        // GET api/querys
        [HttpPost]
        public async Task<IActionResult> GetUser(Users body)
        {
            
            await this.db.Connection.OpenAsync();
            var query = new UserQueries(this.db);

            body.Password = CalculateMD5Hash(body.Password);

            var result = await query.FindOneByIdAsync(body.Email, body.Password);
            if (result == null) {
                return BadRequest("Invalid username or password.");
            }
            var token = this.GetToken(result);
            return new OkObjectResult(token);
        }

      private string GetToken(Users data)
      {
        var Id = data.Id.ToString();
        var claims = new List<Claim>
              {
                  new Claim(JwtRegisteredClaimNames.Sub, Id),
                  new Claim(JwtRegisteredClaimNames.Email, data.Email),
                  new Claim("FullName", String.Concat(data.Name, " ", data.Lastname)),
                  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              };

        if (data.Isadmin == "1") {
            claims.Add(new Claim("IsAdmin", "true"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtIssuer"],
            audience: _configuration["JwtAudience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
      }

      private string CalculateMD5Hash(string password)
      {
          // step 1, calculate MD5 hash from input
          MD5 md5 = System.Security.Cryptography.MD5.Create();
          byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
          byte[] hash = md5.ComputeHash(inputBytes);
          // step 2, convert byte array to hex string
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < hash.Length; i++)
          {
              sb.Append(hash[i].ToString("X2"));
          }
          return sb.ToString();
      }
  }
}
