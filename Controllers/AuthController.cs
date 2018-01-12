using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
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
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto ){ 
            userForRegisterDto.UserName=userForRegisterDto.UserName.ToLower();
            if(await _repo.UserExists(userForRegisterDto.UserName))
               return BadRequest("User is already taken");
            var userToCreate = new User(){
                UserName = userForRegisterDto.UserName 
            };
            var createUser = _repo.Register(userToCreate, userForRegisterDto.Password);
            return StatusCode(201);

        }
    }
}