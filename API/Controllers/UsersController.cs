using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;
using API.Extensions;
using API.Entities;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public UsersController(IMapper mapper,IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }
            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("getmemberprofile")]
        public async Task<ActionResult<MemberDto>> GetMember(string username)
        {            
            var member = await _unitOfWork.UserRepository.GetMemberAsync(username);
            return Ok(member);
        }

        [HttpPut("updatememberprofile")]
        public async Task<ActionResult> UpdateMemberProfile(MemberUpdateDto memberUpdateDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.Complete())
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }

        [HttpPost("addphoto")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if(await _unitOfWork.Complete())
            {
                return CreatedAtRoute("getmemberprofile", new {username = user.UserName} ,_mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("There is a problem with your photo.");
        }

        [HttpPut("setmainphoto/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo.IsMain)
            {
                return BadRequest("This is already your main photo!");
            }
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null)
            {
                currentMain.IsMain = false;
            }
            photo.IsMain = true;
            if(await _unitOfWork.Complete())
            {
                return NoContent();
            }
            return BadRequest("Failed to set your main photo");
        }

        [HttpDelete("deletephoto/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo == null)
            {
                return NotFound();
            }
            if(photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo!");
            }    
            if(photo.PublicId != null)
            {
                var resutl = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(resutl.Error != null)
                {
                    return BadRequest(resutl.Error.Message);
                }
            }
            user.Photos.Remove(photo);
            if(await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Failed to delete the photo!");
        }
    }
}
