using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers 
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route ("api/[controller]")]
    public class UsersController : Controller {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController (IDatingRepository repo, IMapper mapper) {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers (UserParams userParams) {
          var users = await _repo.GetUsers (userParams);

          var usersReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
          Response.AddPagination(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);

            return Ok (usersReturn);
        }

        [HttpGet ("{id}" , Name= "GetUser")]
        public async Task<IActionResult> GetUser (int id) {
            var user = await _repo.GetUser (id);

            var userReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok (userReturn);
        }

       //api/user/1  PUT:
        [HttpPut ("{id}")]
        public async Task<IActionResult> UpdateUser (int id , [FromBody] UserForUpdateDto userForUpdateDto)
        {
            if (!ModelState.IsValid)
              return BadRequest(ModelState);

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFormRepo =await _repo.GetUser(id);  

            if (userFormRepo == null)
              return NotFound($"could not find user with an ID of {id}");

            if (currentUserId != userFormRepo.Id)
                return Unauthorized();

            _mapper.Map(userForUpdateDto,userFormRepo);    

            if (await _repo.SaveAll())
                return NoContent();

             throw new Exception($"Update user {id} faild on save"); 
        }
    }
}