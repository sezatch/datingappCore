using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
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
         private readonly IPhotoService _photoService;

        // public UsersController(DataContext context)
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService )
        {
            _photoService = photoService;
            _userRepository = userRepository;
             _mapper = mapper;
        }

        [HttpGet]
        // public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize, users.TotalCount, users.TotalPages ));
            
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
            // Added Extension Method ClaimsPrincipleExtensions
            // var username =  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // var user = await _userRepository.GetUserByUsernameAsync(username);
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync()) 
            {
                // Addded CreatedAt response
                // return _mapper.Map<PhotoDto>(photo);

                return CreatedAtAction(nameof(GetUser),
                             new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem Adding Photo");
        }
        
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("This is already a main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete main photo");

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the photo"); 
        }

    }
}