using System.Text.Json;
using enova_academy.Data;

public class PaymentWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SqsService _sqsService;
    private readonly ILogger<PaymentWorker> _logger;

    public PaymentWorker(
        IServiceScopeFactory scopeFactory,
        SqsService sqsService,
        ILogger<PaymentWorker> logger
    )
    {
        _scopeFactory = scopeFactory;
        _sqsService = sqsService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($" >>> PaymentWorker eecutando em {DateTime.Now} <<< ");
            var messages = await _sqsService.ReceiveMessagesAsync();
            if (messages != null)
            {
                foreach (var msg in messages)
                {
                    var evt = JsonSerializer.Deserialize<PaymentRequestedEvent>(msg.Body);

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var enrollment = await db.Enrollments.FindAsync(evt!.EnrollmentId);
                    if (enrollment is not null && enrollment.Status == "pending_payment")
                    {
                        // Simula processamento 10–15s
                        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(10, 15)), stoppingToken);

                        enrollment.SetStatus("paid");
                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Enrollment {enrollmentId} set to PAID by Worker", enrollment.Id);

                        Console.WriteLine($@" >>> [WELCOME_EMAIL - via SQS] to enrollment: {enrollment.Id} <<< ");
                    }

                    // Deleta mensagem da fila
                    await _sqsService.DeleteMessageAsync(msg.ReceiptHandle);
                }
            }
        }
    }
}
