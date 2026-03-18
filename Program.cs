using StudyHelperAPI.Hubs;
using StudyHelperAPI.Services;
using StudyHelperAPI.Services.Interfaces;
using StudyHelperAPI.Services;
using StudyHelperAPI.Services.Interfaces;
using NotificationHub = StudyHelperAPI.Hubs.NotificationHub;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// SignalR
builder.Services.AddSignalR();

// HttpClient
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

// Services
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Background Service
builder.Services.AddHostedService<AssignmentCheckerService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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