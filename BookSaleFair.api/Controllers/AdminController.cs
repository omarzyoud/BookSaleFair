using BookSaleFair.api.Data;
using BookSaleFair.api.Domain.DTOs;
using BookSaleFair.api.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookSaleFair.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly BSFDbContext bSFDbContext;
        private readonly BSFAuthDbContext bsfAuthDbContext;
        private readonly UserManager<IdentityUser> userManager;
        public AdminController(BSFDbContext bSFDbContext, BSFAuthDbContext bsfAuthDbContext, UserManager<IdentityUser> userManager)
        {
            this.bSFDbContext = bSFDbContext;
            this.bsfAuthDbContext = bsfAuthDbContext;
            this.userManager = userManager;
        }
        [HttpPost]
        [Route("AddEmployee")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeDTO model)
        {
            var identityUser = new IdentityUser
            {
                UserName = model.email,
                Email = model.email,

            };
            var identityResult = await userManager.CreateAsync(identityUser, model.password);
            if (identityResult.Succeeded)
            {
                var user = new User
                {
                    Name = model.name,
                    Type = "Employee",
                    Email = model.email,

                };
                bSFDbContext.Users.Add(user);
                await bSFDbContext.SaveChangesAsync();


                identityResult = await userManager.AddToRolesAsync(identityUser, new string[] { "Employee" });
                if (identityResult.Succeeded)
                {
                    return Ok("Employee Added succecfully");
                }


            }
            else
            {
                return BadRequest("Not saved");
            }
            return BadRequest("something went wrong");
        }
        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string email)
        {            
            var identityUser = await userManager.FindByEmailAsync(email);
            if (identityUser == null)
            {
                return NotFound("Employee not found in the Identity system.");
            }
           
            var user = await bSFDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("Employee not found in the application database.");
            }
            
            var identityResult = await userManager.DeleteAsync(identityUser);
            if (!identityResult.Succeeded)
            {
                return BadRequest("Failed to delete employee from the Identity system.");
            }
            
            bSFDbContext.Users.Remove(user);
            await bSFDbContext.SaveChangesAsync();

            return Ok("Employee deleted successfully.");
        }

    }
}
