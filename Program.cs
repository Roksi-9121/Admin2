using BusinessLogic.Concrete;
using BusinessLogic.Interface;
using DALWithEF;
using DALWithEF.Context;
using DALWithEF.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<userContext>(options => options.UseSqlServer(connectionString));


builder.Services.AddScoped<IUserDALWithEF, UserDalEF>(provider =>
{
    var mapper = provider.GetRequiredService<AutoMapper.IMapper>();
    return new UserDalEF(mapper, connectionString);
});
builder.Services.AddScoped<IUserManager, UserManager>();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseSession();
app.UseCookiePolicy();


app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "home",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
