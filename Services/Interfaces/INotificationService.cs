namespace StudyHelperAPI.Services.Interfaces;

public interface INotificationService
{
    Task SendAssignmentReminderAsync(string title, string dueDate);
    Task SendNewAssignmentAlertAsync(string title, string courseName);
    Task CheckAndNotifyPendingAssignmentsAsync();
    
}