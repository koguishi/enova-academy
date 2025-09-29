using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;

public class SqsService
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl;

    public SqsService(IConfiguration config)
    {
        var awsConfig = new AmazonSQSConfig
        {
            ServiceURL = config["AWS:ServiceURL"],
            UseHttp = true,
            // ao usar o localstack NÃO configurar RegionEndpoint
            // configurar quando usar AWS
            // RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(config["AWS:Region"])
        };

        _sqs = new AmazonSQSClient(config["AWS:AccessKey"], config["AWS:SecretKey"], awsConfig);

        // Obter URL da fila (cria se não existir)
        _queueUrl = _sqs.GetQueueUrlAsync(config["AWS:QueueName"]).Result.QueueUrl;
    }

    public async Task SendMessageAsync(PaymentRequestedEvent evt)
    {
        var msg = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = JsonSerializer.Serialize(evt)
        };

        await _sqs.SendMessageAsync(msg);
    }

    public async Task<List<Message>> ReceiveMessagesAsync(int maxMessages = 10)
    {
        var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = _queueUrl,
            MaxNumberOfMessages = maxMessages,
            WaitTimeSeconds = 5
        });

        return response.Messages;
    }

    public async Task DeleteMessageAsync(string receiptHandle)
    {
        await _sqs.DeleteMessageAsync(_queueUrl, receiptHandle);
    }
}

public record PaymentRequestedEvent(
    Guid EnrollmentId
    // Guid StudentId,
    // Guid CourseId
);
