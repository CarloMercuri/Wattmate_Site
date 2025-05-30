using Wattmate_Site;
using Wattmate_Site.DataProcessing.Interfaces;
using Wattmate_Site.UserAuthentication.Interfaces;
using Wattmate_Site.UserAuthentication.Processors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

GlobalSettings.TestMode = true;

if (GlobalSettings.TestMode == false)
{
    //builder.Services.AddTransient<ISystemDataHandler, LiveSystemDataHandler>();
    builder.Services.AddTransient<IWattmateAuthenticationService, LiveAuthenticationProcessor>();

}
else
{
    builder.Services.AddTransient<IWattmateAuthenticationService, LiveAuthenticationProcessor>();
}


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

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
