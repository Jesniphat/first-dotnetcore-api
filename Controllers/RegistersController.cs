using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FirstDotNetCore.Models;

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
            body.Password = "abcdef";
            
            await this.db.Connection.OpenAsync();
            body.Db = this.db;
            await body.InsertAsync();
            return new OkObjectResult(body);
            
        }



    }
}
