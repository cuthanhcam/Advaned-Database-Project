using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using C4FAMS.Data;
using C4FAMS.Models;
using C4FAMS.Interfaces;
using C4FAMS.Repositories;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<NguoiDung, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<SignInManager<NguoiDung>>();
builder.Services.AddScoped<UserManager<NguoiDung>>();
builder.Services.AddScoped<RoleManager<IdentityRole<int>>>();

builder.Services.AddControllersWithViews();

// Đăng ký các repository
builder.Services.AddScoped<IKhoaRepository, KhoaRepository>();
builder.Services.AddScoped<IChuyenNganhRepository, ChuyenNganhRepository>();
builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
builder.Services.AddScoped<ISuKienRepository, SuKienRepository>();
builder.Services.AddScoped<ISuKienChiTietRepository, SuKienChiTietRepository>();
builder.Services.AddScoped<ISuKienHinhAnhRepository, SuKienHinhAnhRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Seed dữ liệu mẫu
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    await SeedData.SeedInitialData(context, logger);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();