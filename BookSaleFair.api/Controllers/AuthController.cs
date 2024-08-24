using BookSaleFair.api.Data;
using BookSaleFair.api.Domain.DTOs;
using BookSaleFair.api.Domain.Models;
using BookSaleFair.api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;


namespace BookSaleFair.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BSFDbContext bSFDbContext;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly IEmailSender emailSender;
        public AuthController(BSFDbContext bSFDbContext, UserManager<IdentityUser> userManager, ITokenRepository tokenRepository, IEmailSender emailSender)
        {
            this.bSFDbContext = bSFDbContext;
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.emailSender = emailSender;
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
        [HttpPost]
        [Route("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId == null)
            {
                return Unauthorized();
            }
            
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            string email = user.Email;
            User user2 = await bSFDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user2==null)
            {
                return BadRequest("User not found");
            }
            user2.PasswordHash = model.NewPassword;
            bSFDbContext.Users.Update(user2);           
            await bSFDbContext.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }
        [HttpPost]
        [Route("SendEmail")]
        [Authorize]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailDTO model)
        {
            try
            {
                await emailSender.SendEmailAsync(model.toemail, model.subject, model.message);
                return Ok("Email sent Succefully");
            }
            catch (Exception ex) 
            {
                return  StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("ForgotPassword")]        
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Generate password reset token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            



            // Create the reset URL
            var resetUrl = Url.Action("ResetPassword", "Auth", new { token = token, email = user.Email }, Request.Scheme);

            // Send the email with the reset URL (you can use your EmailSender service here)
            await emailSender.SendEmailAsync(user.Email, "Reset Password",token);            
            return Ok("Password reset link has been sent to your email.");
        }
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO requestDTO)
        {
            var user = await userManager.FindByEmailAsync(requestDTO.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }           
            var result = await userManager.ResetPasswordAsync(user, requestDTO.Token, requestDTO.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }

            return BadRequest("Error while resetting the password.");
        }

    }
}
