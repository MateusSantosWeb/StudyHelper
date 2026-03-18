using StudyHelperAPI.Services.Interfaces;

namespace StudyHelperAPI.Services;

public class AssignmentCheckerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AssignmentCheckerService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

    public AssignmentCheckerService(
        IServiceProvider serviceProvider,
        ILogger<AssignmentCheckerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AssignmentCheckerService iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                
                await notificationService.CheckAndNotifyPendingAssignmentsAsync();
                _logger.LogInformation("Verificação de atividades concluída: {time}", DateTime.Now);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar atividades pendentes.");
            }
            
            await Task.Delay(_interval, stoppingToken);
        }
    }
}