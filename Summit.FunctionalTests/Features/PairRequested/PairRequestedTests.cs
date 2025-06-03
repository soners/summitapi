using System.Text.Json;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AWS.Messaging;
using Confluent.Kafka;
using Xunit.Abstractions;

namespace Summit.FunctionalTests.Features.PairRequested;

[Collection("AspireAppHostCollection")]
public class PairRequestedTests(AspireAppHostFixture fixture, ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task Pair_WhenRequestedForTeacherAndStudentWithHighScore_ShouldReturnAccepted()
    {
        // Arrange
        var snsConfig = new AmazonSimpleNotificationServiceConfig
        {
            EndpointProvider = new StaticEndpointProvider(fixture.AwsConnection.Url),
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
            Message = JsonSerializer.Serialize(new
            {
                Student = new
                {
                    StudentId = "student-id"
                },
                Teacher = new
                {
                    TeacherId = "teacher-id"
                }
            }),
            TimeStamp = DateTime.UtcNow
        };

        // Act
        var pushRequest = new PublishRequest
        {
            TopicArn = "arn:aws:sns:eu-central-1:000000000000:teacher-student-pair-requested",
            Message = JsonSerializer.Serialize(message)
        };
        
        await snsClient.PublishAsync(pushRequest);

        using var cts = new CancellationTokenSource();

        try
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = fixture.KafkaConnectionString,
                GroupId = "summit-worker-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            // Wait for kafka to be available
            await Task.Delay(10 * 1000);
            consumer.Subscribe(topic: "pair-request-processed");

            while (!cts.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cts.Token);

                Assert.NotNull(consumeResult.Message.Value);

                await cts.CancelAsync();
            }
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
            testOutputHelper.WriteLine(e.ToString());
        }
    }
}