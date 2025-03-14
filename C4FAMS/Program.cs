using C4FAMS.Data;
using C4FAMS.Models;
using C4FAMS.Interfaces;
using C4FAMS.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<NguoiDung, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI() // Thêm UI mặc định cho Identity
    .AddDefaultTokenProviders();

// Register repositories
builder.Services.AddScoped<IKhoaRepository, KhoaRepository>();
builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
builder.Services.AddScoped<ICuuSinhVienRepository, CuuSinhVienRepository>();
builder.Services.AddScoped<ISuKienRepository, SuKienRepository>();
builder.Services.AddScoped<IDangKySuKienRepository, DangKySuKienRepository>();
builder.Services.AddScoped<ICongViecRepository, CongViecRepository>();
builder.Services.AddScoped<IThanhTuuRepository, ThanhTuuRepository>();
builder.Services.AddScoped<IThongBaoRepository, ThongBaoRepository>();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    await SeedData.Initialize(scope.ServiceProvider);
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Thêm để hỗ trợ Identity UI

app.Run();