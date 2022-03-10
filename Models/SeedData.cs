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
                    Email = "sherryxu@163.com",
                    NormalizedEmail = "SHERRYXU@163.COM",
                    UserName = "sherryxu@163.com",
                    NormalizedUserName = "SHERRYXU@163.COM",
                    EmailConfirmed = true,
                };

                ApplicationUser seedUser3 = new ApplicationUser()
                {
                    Email = "mingtianxu@163.com",
                    NormalizedEmail = "MINGTIANXU@163.COM",
                    UserName = "mingtianxu@163.com",
                    NormalizedUserName = "MINGTIANXU@163.COM",
                    EmailConfirmed = true,
                };

                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(seedUser1, "P@ssord1");
                var hashed2 = password.HashPassword(seedUser2, "P@ssord1");
                var hashed3 = password.HashPassword(seedUser3, "P@ssord1");
                seedUser1.PasswordHash = hashed;
                seedUser2.PasswordHash = hashed2;
                seedUser3.PasswordHash = hashed3;

                await userManager.CreateAsync(seedUser1);
                await userManager.CreateAsync(seedUser2);
                await userManager.CreateAsync(seedUser3);

                await userManager.AddToRoleAsync(seedUser1, "Admin");
                await userManager.AddToRoleAsync(seedUser2, "User");
                await userManager.AddToRoleAsync(seedUser3, "User");
                context.SaveChanges();

                //if (!context.Journal.Any())
                //{
                //    Journal journal1 = new Journal(seedUser1.UserName, "water my flower");
                //    Journal journal2 = new Journal(seedUser2.UserName, "feed my cat");
                //    Journal journal3 = new Journal(seedUser3.UserName, "wash my car");

                //    List<Journal> journals = new List<Journal> { journal1, journal2, journal3 };
                //    context.Journal.AddRange(journals);
                //    context.SaveChanges();
                //}
            }


            await context.SaveChangesAsync();
        }
    }
}
