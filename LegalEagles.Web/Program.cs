using LegalEagles.Web.Data;
using LegalEagles.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

//===================================================================================
// Data Seeding of Users & Clients
//===================================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var context = services.GetRequiredService<AppDbContext>();
    try
    {
        await context.Database.MigrateAsync();


        // -----------------------------
        // Seed Users
        // -----------------------------
        var users = new[]
        {
            new { Email = "bob@test.com", DisplayName = "Bob Smith" },
            new { Email = "tom@test.com", DisplayName = "Tom Johnson" },
            new { Email = "angela@test.com", DisplayName = "Angela Brown" }
        };

        foreach (var u in users)
        {
            if (await userManager.FindByEmailAsync(u.Email) == null)
            {
                await userManager.CreateAsync(
                    new ApplicationUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        DisplayName = u.DisplayName
                    },
                    "Password123!");
            }
        }
        // -----------------------------
        // Seed Clients (50 records)
        // -----------------------------
        if (!context.Clients.Any())
        {
            var firstNames = new[]
            {
                "John", "Jane", "Michael", "Sarah", "David", "Emily", "Daniel", "Anna",
                "Robert", "Laura", "James", "Maria", "Mark", "Sophia", "Paul", "Emma"
            };

            var lastNames = new[]
            {
                "Smith", "Johnson", "Brown", "Garcia", "Martinez",
                "Davis", "Rodriguez", "Wilson", "Anderson", "Taylor"
            };

            var clients = new List<Client>();

            for (int i = 1; i <= 50; i++)
            {
                var firstName = firstNames[i % firstNames.Length];
                var lastName = lastNames[i % lastNames.Length];

                clients.Add(new Client
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Address = $"#{i} Main Street, City {i}",
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@test.com",
                    MobileNumber = $"09{i:D9}".Substring(0, 11),
                    BirthDate = DateTime.Today.AddYears(-22 - (i % 20)).AddDays(i * 3)
                });
            }

            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during seeding");
    }
}
//===================================================================================

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Clients}/{action=Index}/{id?}");

app.Run();
