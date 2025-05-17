using Wattmate_Site;
using Wattmate_Site.DataProcessing.Interfaces;
using Wattmate_Site.Users.UserAuthentication.Interfaces;
using Wattmate_Site.Users.UserAuthentication.Processors;
using Wattmate_Site.WDatabase.Interfaces;
using Wattmate_Site.WDatabase.Queries;
using Wattmate_Site.WLog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

GlobalSettings.TestMode = false;
GlobalSettings.LocalMode = false;

if (GlobalSettings.TestMode == false)
{
    //builder.Services.AddTransient<ISystemDataHandler, LiveSystemDataHandler>();
    builder.Services.AddTransient<IWattmateAuthenticationService, LiveAuthenticationProcessor>();
    builder.Services.AddTransient<IWDatabaseQueries, WDatabaseQueries>();

}
else
{
    builder.Services.AddTransient<IWattmateAuthenticationService, LiveAuthenticationProcessor>();
    builder.Services.AddTransient<IWDatabaseQueries, WLocalFileQueries>();
}

WLogging.Initialize();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    GlobalSettings.LocalMode = false;
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    GlobalSettings.LocalMode = true;
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
