using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FirstDotNetCore.Models;
using System.Security.Cryptography;
using System.Text;

namespace FirstDotNetCore
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistersController : ControllerBase
    { 
        public AppDb db;
        public RegistersController(AppDb DB)
        {
            // Use AppDb instance service
            this.db = DB;
        }

        // GET api/querys
        [HttpGet]
        public async Task<IActionResult> GetLatest()
        {
            
            await this.db.Connection.OpenAsync();
            var query = new UserQueries(this.db);
            var result = await query.LatestPostsAsync();
            return new OkObjectResult(result);
        }


        // GET api/querys/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            using (this.db)
            {
                await this.db.Connection.OpenAsync();
                var query = new UserQueries(this.db);
                var result = await query.FindOneAsync(id);
                if (result == null)
                    return new NotFoundResult();
                return new OkObjectResult(result);
            }
        }

        // POST api/async
        [HttpPost]
        public async Task<IActionResult> Post(Users body)
        {
            body.Password = this.CalculateMD5Hash(body.Password);
            
            await this.db.Connection.OpenAsync();
            body.Db = this.db;
            await body.InsertAsync();
            return new OkObjectResult(body);
            
        }

        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
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
