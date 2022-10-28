using CVMLabs.Domain.Entities;
using CVMLabs.Domain.Repositories.Abstract;
using CVMLabs.Domain.Repositories.EntityFramework;
using CVMLabs.Service;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
var mySqlServerVersion = new MySqlServerVersion(new Version(10, 4, 24));
var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var keysDbConnectionString = builder.Configuration.GetConnectionString("KeysConnection");
var environment = builder.Environment;

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<KeysDbContext>()
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

builder.Services.AddTransient<ISubjectsRepository, EFSubjectsRepository>();
builder.Services.AddTransient<ILessonsRepository, EFLessonsRepository>();
builder.Services.AddTransient<IStudentsRepository, EFStudentsRepository>();
builder.Services.AddTransient<DataManager>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(dbConnectionString, mySqlServerVersion, options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});
builder.Services.AddDbContext<KeysDbContext>(options => options.UseMySql(keysDbConnectionString, mySqlServerVersion));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "CVMLabsAuth";
    options.LoginPath = "/account/signin";
    options.AccessDeniedPath = "/error/acccesdenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(365);
    options.SlidingExpiration = true;
});

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddMvc();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
else app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default", 
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();