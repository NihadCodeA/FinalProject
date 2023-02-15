using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Database>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequiredUniqueChars = 0;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase= true;
    opt.Password.RequiredLength = 8;
    opt.User.RequireUniqueEmail= true;
}).AddEntityFrameworkStores<Database>().AddDefaultTokenProviders();

//localization

builder.Services.AddLocalization(opt =>
{
    opt.ResourcesPath = "Resources";
});
//builder.Services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization(
//    opt =>
//    {
//        opt.DataAnnotationLocalizerProvider = (type, factory) =>
//        {
//            var assemblyName = new AssemblyName(typeof(SharedResource).Assembly.FullName);
//            return factory.Create(nameof(SharedResource), assemblyName.Name);
//        };
//    }
//    );
builder.Services.Configure<RequestLocalizationOptions>(opt =>
{
    var supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("az-Latn-AZ")
    };

    opt.DefaultRequestCulture = new RequestCulture("az-Latn-AZ");
    opt.SupportedCultures=supportedCultures;
    opt.SupportedUICultures = supportedCultures;

    opt.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider(),
    };
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

var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options.Value);

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{culture=az-Latn-AZ}/{controller=Home}/{action=Index}/{id?}");

app.Run();
