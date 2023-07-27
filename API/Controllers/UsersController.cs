using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    // [ApiController]
    // [Route("api/[controller]")] // localhost:-----/api/users
    [Authorize]
    public class UsersController : BaseApiController
    {
        // private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
         private readonly IMapper _mapper;

        // public UsersController(DataContext context)
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
             _mapper = mapper;
        }

        [HttpGet]
        // public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);


            //commenting below code to optimize automapper
            // var users = await _userRepository.GetUsersAsync();
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            // return Ok(usersToReturn);

            //Commenting below line to adjust to AutoMapper
            // return Ok(await _userRepository.GetUsersAsync());
            // return BadRequest();
        }

        [HttpGet("{username}")]
        // public async Task<ActionResult<AppUser>> GetUser(string username)
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {

            return await _userRepository.GetMemberAsync(username);

            // optimizing Automapper = automapping in repository insead of usercontroller
            // var user = await _userRepository.GetUserByUsernameAsync(username);
            
            // return _mapper.Map<MemberDto>(user);



            //Commenting below line to adjust to AutoMapper
            // return await _userRepository.GetUserByUsernameAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> updateUser(MemberUpdateDto memberUpdateDto)
        {
            var username =  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if(user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

    }
}