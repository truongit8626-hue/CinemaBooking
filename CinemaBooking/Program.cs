using CinemaBooking.Data;
using CinemaBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
// 1. Kết nối DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity + ROLE (🔥 QUAN TRỌNG)
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>() // 🔥 BẮT BUỘC để dùng Admin
.AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Cookie login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// 4. MVC + Razor + Session
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();

var app = builder.Build();

// Middleware xử lý lỗi tổng quát
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/html; charset=utf-8";

        await context.Response.WriteAsync("""
            <html>
            <head>
                <meta charset="utf-8">
                <title>Lỗi hệ thống</title>
            </head>
            <body style="font-family: Arial; text-align: center; padding: 50px;">
                <h1>⚠️ Có lỗi xảy ra</h1>
                <p>Hệ thống gặp sự cố khi xử lý yêu cầu.</p>
                <p>Vui lòng thử lại sau.</p>
                <a href="/" style="display:inline-block; margin-top:20px;">
                    ← Quay lại trang chủ
                </a>
            </body>
            </html>
            """);
    });
});

// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movie}/{action=Index}/{id?}");

app.MapRazorPages();

// 5. Seed dữ liệu phim (giữ nguyên code cũ của bạn)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.Movies.Any())
    {
        db.Movies.AddRange(
            new Movie { Title = "Avengers", Genre = "Action", Duration = 120, Description = "Marvel" },
            new Movie { Title = "Conan", Genre = "Anime", Duration = 90, Description = "Detective" }
        );

        db.SaveChanges();
    }
}

// 6. TẠO ADMIN (🔥 QUAN TRỌNG NHẤT)
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Tạo role Admin nếu chưa có
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // 🔥 ĐỔI EMAIL THEO TÀI KHOẢN BẠN ĐÃ REGISTER
    var user = await userManager.FindByEmailAsync("hoangayvip@gmail.com");

    if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
    {
        await userManager.AddToRoleAsync(user, "Admin");
    }
}


app.Run();  