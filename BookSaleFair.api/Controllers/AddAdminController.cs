using BookSaleFair.api.Data;
using BookSaleFair.api.Domain.DTOs;
using BookSaleFair.api.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookSaleFair.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddAdminController : ControllerBase
    {
        private readonly BSFDbContext bSFDbContext;
        private readonly BSFAuthDbContext authContext;
        private readonly UserManager<IdentityUser> userManager;
        public AddAdminController(BSFAuthDbContext authContext, BSFDbContext bSFDbContext, UserManager<IdentityUser> userManager)
        {
            this.authContext = authContext;
            this.bSFDbContext = bSFDbContext;
            this.userManager = userManager;
        }
        [HttpPost]
        [Route("AddAmin")]
        public async Task<IActionResult> AddAdmin([FromBody] AddAdminDTO model)
        {
            var identityUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,

            };
            var identityResult = await userManager.CreateAsync(identityUser, model.Password);
            if (identityResult.Succeeded)
            {
                var user = new User
                {
                    Name = model.Name,
                    Type = "Admin",
                    Email = model.Email,
                    PasswordHash = model.Password
                };
                bSFDbContext.Users.Add(user);
                await bSFDbContext.SaveChangesAsync();


                identityResult = await userManager.AddToRolesAsync(identityUser, new string[] { "Admin" });
                if (identityResult.Succeeded)
                {
                    return Ok("Admin Adedd Successfully");
                }


            }
            else
            {
                return BadRequest("Not saved");
            }
            return BadRequest("something went wrong");
        }
    }
}
