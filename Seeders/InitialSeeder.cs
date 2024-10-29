using Bogus;
using BookManagement.Constants;
using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Seeders;

public class InitialSeeder
{
    private readonly ApplicationDbContext dbContext;

    public InitialSeeder(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var seeder = new InitialSeeder(dbContext);

        await seeder.BookSeeder();
        await seeder.UserSeeder(serviceProvider);
        await seeder.RoleSeeder();
    }

    public async Task BookSeeder()
    {
        // Apply any pending migrations
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }

        if (await dbContext.Database.CanConnectAsync())
        {
            if (!dbContext.Books.Any())
            {
                var bookFaker = new BookFaker();
                var fakeBooks = bookFaker.Generate(10);
                dbContext.Books.AddRange(fakeBooks);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    public async Task UserSeeder(IServiceProvider serviceProvider)
    {
        // Apply any pending migrations
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }

        if (await dbContext.Database.CanConnectAsync())
        {
            if (!await dbContext.Users.AnyAsync())
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Seed the Admin role
                string roleName = UserRoles.Admin;
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                // Seed the Admin user
                var adminEmail = "admin@example.com"; // Change as needed
                var adminPassword = "Admin@123"; // Change as needed

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, roleName);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                    }
                }
            }
        }
    }

    public async Task RoleSeeder()
    {
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }

        if (await dbContext.Database.CanConnectAsync())
        {
            if (!dbContext.Roles.Any())
            {
                var roles = GetRoles();
                dbContext.Roles.AddRange(roles);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private IEnumerable<IdentityRole> GetRoles()
    {
        return new List<IdentityRole>
        {
            new IdentityRole(UserRoles.User) { NormalizedName = UserRoles.User.ToUpper() },
            new IdentityRole(UserRoles.Manager) { NormalizedName = UserRoles.Manager.ToUpper() },
            new IdentityRole(UserRoles.Admin) { NormalizedName = UserRoles.Admin.ToUpper() }
        };
    }

    public class BookFaker : Faker<Book>
    {
        public BookFaker()
        {
            RuleFor(b => b.Title, f => f.Lorem.Sentence(3));
            RuleFor(b => b.Author, f => f.Name.FullName());
            RuleFor(b => b.Genre, f => f.PickRandom(new[] { "Fiction", "Non-Fiction", "Science Fiction", "Fantasy", "Mystery", "Biography", null }));
            RuleFor(b => b.YearPublished, f => f.Date.Past(30).Year);
        }
    }
}
