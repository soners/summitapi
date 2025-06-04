using System.Text.Json;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using AWS.Messaging;

namespace Summit.FunctionalTests.Helpers;

public static class SqsExtensions
{
    public static async Task<bool> CheckSqsReadiness(this AwsConnection awsConnection)
    {
        var sqsClient = CreateSqsClient(awsConnection);

        var getQueueUrlRequest = new GetQueueUrlRequest(queueName: "teacher-student-pair-requested-queue");

        try
        {
            var response = await sqsClient.GetQueueUrlAsync(getQueueUrlRequest);

            return response.HttpStatusCode.Equals(HttpStatusCode.OK);
        }
        catch
        {
            return false;
        }
    } 
    
    private static AmazonSQSClient CreateSqsClient(AwsConnection awsConnection)
    {
        var sqsConfig = new AmazonSQSConfig
        {
            EndpointProvider = new StaticEndpointProvider(awsConnection.Url),
            RegionEndpoint = RegionEndpoint.EUCentral1,
            AuthenticationRegion = RegionEndpoint.EUCentral1.SystemName,
        };
        
        var awsCredentials = new SessionAWSCredentials("test", "test", "test");
        return new AmazonSQSClient(awsCredentials, sqsConfig);
    }

    public static async Task PushMessage(this AwsConnection awsConnection, object data)
    {
        var snsConfig = new AmazonSimpleNotificationServiceConfig
        {
            EndpointProvider = new StaticEndpointProvider(awsConnection.Url),
            RegionEndpoint = RegionEndpoint.EUCentral1,
            AuthenticationRegion = RegionEndpoint.EUCentral1.SystemName,
        };
        var awsCredentials = new SessionAWSCredentials("test", "test", "test");

        var snsClient = new AmazonSimpleNotificationServiceClient(awsCredentials, snsConfig);
        
        var message = new MessageEnvelope<string>
        {
            Id = Guid.NewGuid().ToString(),
            Source = new Uri("/aws/messaging"),
            Version = "1.0",
            MessageTypeIdentifier = "summitapi",
            Message = JsonSerializer.Serialize(data),
            TimeStamp = DateTime.UtcNow
        };

        // Act
        var pushRequest = new PublishRequest
        {
            TopicArn = "arn:aws:sns:eu-central-1:000000000000:teacher-student-pair-requested",
            Message = JsonSerializer.Serialize(message)
        };
        
        await snsClient.PublishAsync(pushRequest);
    }
}