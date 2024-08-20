using BookSaleFair.api.Data;
using BookSaleFair.api.Domain.DTOs;
using BookSaleFair.api.Domain.Models;
using BookSaleFair.api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookSaleFair.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BSFDbContext bSFDbContext;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
        public AuthController(BSFDbContext bSFDbContext, UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.bSFDbContext = bSFDbContext;
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO requestDTO)

        {
            var identityUser = new IdentityUser
            {
                UserName = requestDTO.Email,
                Email = requestDTO.Email,

            };
            var identityResult = await userManager.CreateAsync(identityUser, requestDTO.password);
            if (identityResult.Succeeded)
            {
                var user = new User
                {
                    Name = requestDTO.Name,
                    Type = "Customer",
                    Email = requestDTO.Email,
                    PasswordHash = requestDTO.password
                };
                bSFDbContext.Users.Add(user);
                await bSFDbContext.SaveChangesAsync();
                
                
                    identityResult = await userManager.AddToRolesAsync(identityUser, new string[] { "Customer" });
                    if (identityResult.Succeeded)
                    {
                        return Ok("succecfull registered, please login");
                    }
                
               
            }
            else
            {
                return BadRequest("Not saved");
            }    
            return BadRequest("something went wrong");

        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO lrd)
        {

            var user = await userManager.FindByEmailAsync(lrd.Email);
            if (user != null)
            {
                var checkPassword = await userManager.CheckPasswordAsync(user, lrd.Password);
                if (checkPassword)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    User usr = bSFDbContext.Users.FirstOrDefault(u => u.Email == lrd.Email);
                    int usrid = usr.UserId;
                    string name = usr.Name;
                    

                    if (roles != null)
                    {


                        var jwttoken = tokenRepository.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDTO
                        {
                            JwtToken = jwttoken,
                            Roles = roles.ToList(),
                            UserId = usrid,
                            Name = name,
                            

                        };

                        return Ok(response);
                    }
                }
            }

            return BadRequest("Username or password incorrect");
        }

    }
}
