using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookSaleFair.api.Data
{
    public class BSFAuthDbContext: IdentityDbContext
    {
        public BSFAuthDbContext(DbContextOptions<BSFAuthDbContext> options): base(options) 
        {
                
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var CustomerRoleId = "2b6a11a6-94cc-4fac-9ae5-69b1ef97f97a";
            var EmployeeRoleId = "41d1e0ef-8f8b-4ba9-808e-7f9733fe9652";
            var AdminRoleId = "ccf9682f-7112-41e8-8e83-54fca0a6821a";
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = CustomerRoleId,
                    ConcurrencyStamp = CustomerRoleId,
                    Name = "Customer",
                    NormalizedName = "Customer".ToUpper()
                },
                new IdentityRole
                {
                    Id = EmployeeRoleId,
                    ConcurrencyStamp = EmployeeRoleId,
                    Name = "Employee",
                    NormalizedName = "Employee".ToUpper()
                },
                new IdentityRole
                {
                    Id = AdminRoleId,
                    ConcurrencyStamp = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }


            };
            builder.Entity<IdentityRole>().HasData(roles);

        }
    }
}
