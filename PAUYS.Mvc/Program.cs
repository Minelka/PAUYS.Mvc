using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using PAUYS.Mvc.ActionFilters__;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Log4Net yapýlandýrmasý
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
var logConfigFile = new System.IO.FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
XmlConfigurator.Configure(logRepository, logConfigFile);

// Örnek bir log mesajý
var logger = LogManager.GetLogger(typeof(Program));
logger.Info("Uygulama baþlatýldý.");

// Servisleri konteynýra ekleyin
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
// Servisleri ekleyin
builder.Services.AddControllersWithViews(options =>
{
    // Global olarak Log4NetActionFilter'ý ekliyoruz
    options.Filters.Add(new Log4NetActionFilter());
});

// Kimlik doðrulama ve yetkilendirme ayarlarý
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie("AdminScheme", opt =>
{
    opt.Cookie.Name = "Auth.Admin.Cookie";
    opt.LoginPath = "/Auth/Login";
    opt.LogoutPath = "/Auth/Logout";
    opt.AccessDeniedPath = "/Auth/AccessDenied";
})
.AddCookie("ContentScheme", opt =>
{
    opt.Cookie.Name = "Auth.Content.Cookie";
    opt.LoginPath = "/Auth/Login";
    opt.LogoutPath = "/Auth/Logout";
    opt.AccessDeniedPath = "/Auth/AccessDenied";
})
.AddCookie("CustomerScheme", opt =>
{
    opt.Cookie.Name = "Auth.Customer.Cookie";
    opt.LoginPath = "/Auth/Login";
    opt.LogoutPath = "/Auth/Logout";
    opt.AccessDeniedPath = "/Auth/AccessDenied";
});

builder.Services.AddAuthorization(opt =>
{
    // Admin Policy
    opt.AddPolicy("AdminPolicy", policy =>
    {
        policy.AddAuthenticationSchemes("AdminScheme");
        policy.RequireAuthenticatedUser();
    });

    // Content Policy
    opt.AddPolicy("ContentPolicy", policy =>
    {
        policy.AddAuthenticationSchemes("ContentScheme");
        policy.RequireAuthenticatedUser();
    });

    // Customer Policy
    opt.AddPolicy("CustomerPolicy", policy =>
    {
        policy.AddAuthenticationSchemes("CustomerScheme");
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.AddSession(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
    //opt.Cookie.Expiration = TimeSpan.FromMinutes(60);
    opt.IdleTimeout = TimeSpan.FromMinutes(60);
});

var app = builder.Build();

// HTTP istekleri iþleme zincirini yapýlandýrýn
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapRazorPages();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulamayý çalýþtýrýn
app.Run();
