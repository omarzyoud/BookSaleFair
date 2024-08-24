using BookSaleFair.api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BookSaleFair.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly BSFDbContext bSFDbContext;
        private BSFAuthDbContext BSFAuthDbContext;
        private UserManager<IdentityUser> userManager;
        public EmployeeController(BSFDbContext bSFDbContext, BSFAuthDbContext BSFAuthDbContext, UserManager<IdentityUser> userManager)
        {
            this.bSFDbContext = bSFDbContext;
            this.BSFAuthDbContext = BSFAuthDbContext;
            this.userManager = userManager;
        }

    }
}
