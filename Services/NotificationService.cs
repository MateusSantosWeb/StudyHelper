using Microsoft.AspNetCore.SignalR;
using StudyHelperAPI.Services.Interfaces;

namespace StudyHelperAPI.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IClassroomService _classroomService;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        IClassroomService classroomService)
    {
        _hubContext = hubContext;
        _classroomService = classroomService;
        
    }
    
    
    public async Task SendAssignmentReminderAsync(string title, string dueDate)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
        {
            Type = "Reminder",
            Title = "⏰ Prazo se aproximando!",
            Message = $"A Atividade '{title}' vence em {dueDate}",
            Time = DateTime.Now.ToString("HH:mm")
        });
    }

    public async Task SendNewAssignmentAlertAsync(string title, string courseName)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
        {
            Type = "Reminder",
            Title = "📝 Nova atividade!",
            Message = $"'{title}' foi postada em {courseName}",
            Time = DateTime.Now.ToString("HH:mm")
        });
    }

    public async Task CheckAndNotifyPendingAssignmentsAsync()
    {
        var pending = await _classroomService.GetPendingAssignmentsAsync();

        foreach (var assignment in pending)
        {
            if (!assignment.DueDate.HasValue) continue;
            
            var daysLeft = (assignment.DueDate.Value - DateTime.Now).TotalDays;

            if (daysLeft <= 1)
            {
                await SendAssignmentReminderAsync(
                    assignment.Title,
                    assignment.DueDate.Value.ToString("dd/MM/yyyy HH:mm"));
            }
            else if (daysLeft > 3)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
                    {
                        Type = "warning",
                        Title = "⚠️ Atividade pendente!",
                        Message = $"'{assignment.Title}' vence em {Math.Round(daysLeft)} dias",
                        Time = DateTime.Now.ToString("HH:mm")
                    }
                );
                
            }
        }
    }
}
