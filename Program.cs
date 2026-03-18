using Microsoft.AspNetCore.HttpOverrides;
using StudyHelperAPI.Hubs;
using StudyHelperAPI.Services;
using StudyHelperAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// MVC
builder.Services.AddControllersWithViews();

// SignalR
builder.Services.AddSignalR();

// HttpClient
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

// Services
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Background Service
builder.Services.AddHostedService<AssignmentCheckerService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// SignalR Hub
app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
