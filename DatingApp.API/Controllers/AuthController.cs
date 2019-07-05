using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using DatingApp.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _authRepository;
        private IConfiguration _config;

        public AuthController(IAuthRepository authRepository, IConfiguration config)
        {
            _authRepository = authRepository;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterViewModel userForRegisterViewModel)
        {
            // check username is unique
            string username = userForRegisterViewModel.Username.ToLower().Trim();
            if (await _authRepository.UserExists(username))
                return BadRequest("Username already exists");

            User userToCreate = new User()
            {
                Username = username
            };

            User createdUser = await _authRepository.Register(userToCreate, userForRegisterViewModel.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginViewModel userForLoginViewModel)
        {
            User userFromRepo = await _authRepository.Login(userForLoginViewModel.Username, userForLoginViewModel.Password);
            if (userFromRepo == null)
                return Unauthorized();

            // token will contains to claims: userid and username
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // sign the token by creating a security key and encrypting it with a hashing algorithm, and will expire in 24 hrs
            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // create the JWT security token handler that allows to create the token based on the token descriptor
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // finally use this token to write it to the response back to the client
            return Ok(new { token = tokenHandler.WriteToken(token)});

            // to inspect the JWT token, goto jwt.io
        }

    }
}