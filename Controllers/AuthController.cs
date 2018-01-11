using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(string userName, string password){ 
            userName=userName.ToLower();
            if(await _repo.UserExists(userName))
               return BadRequest("User is already taken");
            var userToCreate = new User(){
                UserName = userName 
            };
            var createUser = _repo.Register(userToCreate, password);
            return StatusCode(201);

        }
    }
}