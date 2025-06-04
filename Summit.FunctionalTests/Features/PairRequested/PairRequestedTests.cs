using Confluent.Kafka;
using Summit.FunctionalTests.Helpers;
using Xunit.Abstractions;

namespace Summit.FunctionalTests.Features.PairRequested;

[Collection("AspireAppHostCollection")]
public class PairRequestedTests(AspireAppHostFixture fixture, ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task PairRequested_WhenReceived_ShouldProcessAndPublishToKafka()
    {
        // Arrange
        await fixture.AwsConnection.PushMessage(new
        {
            Student = new
            {
                StudentId = "student-id",
                Name = "John",
                Email = "john.doe@gmail.com",
            },
            Teacher = new
            {
                TeacherId = "teacher-id",
                Name = "Brian",
                Email = "brian.doe@gmail.com",
            }
        });

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

            // Wait for kafka to be available, consider using Polly.
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