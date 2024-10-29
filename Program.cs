using BookManagement.Authorization.Requirements;
using BookManagement.Authorization;
using BookManagement.Authorization.Services;
using BookManagement.Configuration;
using BookManagement.Data;
using BookManagement.Models;
using BookManagement.Seeders;
using BookManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddClaimsPrincipalFactory<BooksUserClaimsPrincipalFactory>()
.AddDefaultTokenProviders();



builder.Services.AddAuthorizationBuilder()
          .AddPolicy(PolicyNames.HasNationality,
              builder => builder.RequireClaim(AppClaimTypes.Nationality, "VietNam", "Japan"))
          .AddPolicy(PolicyNames.AtLeast18,
              builder => builder.AddRequirements(new MinimumAgeRequirement(18)));
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<InitialSeeder>();
builder.Services.AddScoped<BookAuthorizationService>();
builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<InitialSeeder>();
await InitialSeeder.Initialize(scope.ServiceProvider);




app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
