using BookSaleFair.api.Data;
using BookSaleFair.api.Domain.DTOs;
using BookSaleFair.api.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NsfwSpyNS;
using System.Diagnostics.Eventing.Reader;

namespace BookSaleFair.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly BSFDbContext bSFDbContext;
        private BSFAuthDbContext BSFAuthDbContext;
        private UserManager<IdentityUser> userManager;
        private readonly INsfwSpy nsfwSpy;
        public EmployeeController(BSFDbContext bSFDbContext, BSFAuthDbContext BSFAuthDbContext, UserManager<IdentityUser> userManager, INsfwSpy nsfwSpy)
        {
            this.bSFDbContext = bSFDbContext;
            this.BSFAuthDbContext = BSFAuthDbContext;
            this.userManager = userManager;
            this.nsfwSpy = nsfwSpy;
        }
        [HttpPost]
        [Route("AddBook")]
        //[Authorize(Roles ="Admin,Employee")]

        public async Task<IActionResult> AddBook([FromForm] AddBookDTO model)
        {
            try
            {
                IFormFile file = model.Cover;

                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file");
                }
                if (IsImage(file.ContentType))
                
                {                    
                    byte[] fileData;
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        fileData = ms.ToArray();
                    }
                    
                    var result = nsfwSpy.ClassifyImage(fileData);

                    if (result.IsNsfw)
                    {                       
                        return BadRequest("The uploaded image contains nude content and cannot be processed.");
                    }

                    var book = new Book
                    {
                        Title=model.Title,
                        Author=model.Auther,
                        FileData=fileData,
                        Price=model.price,
                        Subject=model.Subject,
                        QuantityAvailable=model.Quantity


                    };
                    bSFDbContext.Books.AddAsync(book);
                    await bSFDbContext.SaveChangesAsync();
                    return Ok(  "Book Added successfully" );
                }
                else
                {
                    return BadRequest("The cover should be an Image");
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }

        }
        private bool IsImage(string contentType)
        {
            
            var acceptedImageTypes = new string[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/jpg", "image/webp" };

            
            return acceptedImageTypes.Contains(contentType);
        }
    }
}
