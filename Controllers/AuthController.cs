using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers {
    [Route ("api/[controller]")]
    public class AuthController : Controller {
        private readonly IAuthRepository _repo;
        public AuthController (IAuthRepository repo) {
         _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Resister(string username,string password)
        {
            // validate request
            username=username.ToLower();
            
            if (await _repo.UserExists(username))
              return BadRequest("Username is already taken");

            var userToCreate =  new User
            {
                Username=username
            } ;

            var createUser = await _repo.Resister(userToCreate,password);

            return StatusCode(201);
               
        }
    }
}