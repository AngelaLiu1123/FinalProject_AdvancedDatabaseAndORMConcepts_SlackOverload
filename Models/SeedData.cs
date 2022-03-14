using FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models
{
    public class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //insert some seed data(user/role) into Database if there are any user and roles in it.
            if (!context.Roles.Any())
            {
                List<string> roles = new List<string>
                {
                    "Admin",
                    "User",
                    "Visitor"
                };

                foreach (string role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    context.SaveChanges();
                }
            }

            if (!context.Users.Any())
            {
                ApplicationUser seedUser1 = new ApplicationUser()
                {
                    Email = "lingliu@mitt.ca",
                    NormalizedEmail = "LINGLIU@MITT.CA",
                    UserName = "lingliu@mitt.ca",
                    NormalizedUserName = "LINGLIU@MITT.CA",
                    EmailConfirmed = true,
                };

                ApplicationUser seedUser2 = new ApplicationUser()
                {
                    Email = "test@163.com",
                    NormalizedEmail = "TEST@163.COM",
                    UserName = "test@163.com",
                    NormalizedUserName = "TEST@163.COM",
                    EmailConfirmed = true,
                };

                ApplicationUser seedUser3 = new ApplicationUser()
                {
                    Email = "test2@163.com",
                    NormalizedEmail = "TEST2@163.COM",
                    UserName = "test2@163.com",
                    NormalizedUserName = "TEST2@163.COM",
                    EmailConfirmed = true,
                };

                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(seedUser1, "P@ssword1");
                var hashed2 = password.HashPassword(seedUser2, "P@ssword1");
                var hashed3 = password.HashPassword(seedUser3, "P@ssword1");
                seedUser1.PasswordHash = hashed;
                seedUser2.PasswordHash = hashed2;
                seedUser3.PasswordHash = hashed3;

                await userManager.CreateAsync(seedUser1);
                await userManager.CreateAsync(seedUser2);
                await userManager.CreateAsync(seedUser3);

                await userManager.AddToRoleAsync(seedUser1, "Admin");
                await userManager.AddToRoleAsync(seedUser2, "User");
                await userManager.AddToRoleAsync(seedUser3, "User");
            }

            await context.SaveChangesAsync();
        }
    }
}
