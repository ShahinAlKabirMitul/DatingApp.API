using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var useeToReturn = _mapper.Map< IEnumerable<UserForListDto>>(users);
            return Ok(useeToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsers(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailDto>(user);
            return Ok(userToReturn);
        }

        // api/users/i PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] UserForUpdateDto userForUpdateDto) {
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var currentUserid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(id);
            if(userFromRepo == null){
                return NotFound($"Could not find user with an ID of {id} ");
            }
            if(currentUserid != userFromRepo.Id){
                return Unauthorized();
            }
            _mapper.Map(userForUpdateDto,userFromRepo);
            if(await _repo.SaveAll())
                return NoContent();
            throw new Exception($"Updating User Id {id} falied on save");
        }
    }
}