using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
               ModelState.AddModelError("UserName", errorMessage: "User already exsits");

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var userToCreate = new User(){
                UserName = userForRegisterDto.UserName 
            };
            var createUser = _repo.Register(userToCreate, userForRegisterDto.Password);
           
            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.UserName.ToLower(),userForLoginDto.Password);
           
            
            if(userFromRepo == null)
               return Unauthorized();
            
            // generate token 

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Super secret key");
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim( ClaimTypes.Name , userFromRepo.UserName)
                }),
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                 SecurityAlgorithms.HmacSha512Signature)

            };
            var token=tokenHandler.CreateToken(tokenDescriptor);
            var tokenString=tokenHandler.WriteToken(token);
            return Ok( new {tokenString});

        }
    }
}