using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Models;
using DatingApp.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepo, IMapper mapper)
        {
            _datingRepo = datingRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            IEnumerable<User> users = await _datingRepo.GetUsers();
            IEnumerable<UserForListViewModel> usersToReturn = _mapper.Map<IEnumerable<UserForListViewModel>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            User user = await _datingRepo.GetUser(id);
            UserForDetailViewModel userToReturn = _mapper.Map<UserForDetailViewModel>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateViewModel userForUpdateViewModel)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(id);
            _mapper.Map(userForUpdateViewModel, userFromRepo);

            if (await _datingRepo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }

    }
}