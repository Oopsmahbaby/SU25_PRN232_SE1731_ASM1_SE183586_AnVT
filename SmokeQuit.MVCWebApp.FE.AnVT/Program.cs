using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmokeQuit.Repositories.AnVT.DBContext;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SU25_PRN232_SE1731_G6_SmokeQuitContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication()
	.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
	{
		options.LoginPath = new PathString("/Account/Login");
		options.AccessDeniedPath = new PathString("/Account/Forbidden");
		options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

	});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
