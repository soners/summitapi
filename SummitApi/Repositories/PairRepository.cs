using System.Text;
using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AWS.Messaging;
using SummitApi.Models;

namespace SummitApi.Repositories;

public interface IPairRepository
{
    Task<RepositoryOperation<string>> Pair(Teacher teacher, Student student, CancellationToken cancellationToken);
}

public class PairRepository(
    IConfiguration configuration,
    AmazonSimpleNotificationServiceClient snsClient) : IPairRepository
{
    public async Task<RepositoryOperation<string>> Pair(Teacher teacher, Student student, CancellationToken cancellationToken)
    {
        var message = new MessageEnvelope<string>
        {
            Id = Guid.NewGuid().ToString(),
            Source = new Uri("/aws/messaging"),
            Version = "1.0",
            MessageTypeIdentifier = "summitapi",
            Message = JsonSerializer.Serialize(new PairMessage(teacher, student)),
            TimeStamp = DateTime.UtcNow
        };

        try
        {
            var publishRequest = new PublishRequest
            {
                TopicArn = configuration["AWS:Sns:TeacherStudentPairRequestedTopicArn"],
                Message = JsonSerializer.Serialize(message),
            };

            var response = await snsClient.PublishAsync(publishRequest, cancellationToken);
            
            return new RepositoryOperation<string>.Success(response.MessageId);
        }
        catch (Exception ex)
        {
            return new RepositoryOperation<string>.Error(ex);
        }
    }
}