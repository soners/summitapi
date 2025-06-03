namespace Summit.FunctionalTests.Helpers;

using System;
using System.Threading.Tasks;
using Polly;

public static class LocalStackReadinessExtensions
{
    private static readonly int MaxRetries = 5;

    public static async Task EnsureLocalStackIsReadyAsync(this AwsConnection awsConnection)
    {
        var retryPolicy = Policy
            .HandleResult<bool>(isReady => !isReady) // Retry if not ready
            .WaitAndRetryAsync(
                retryCount: MaxRetries,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponential backoff
                onRetry: (result, timeSpan, attempt, context) =>
                {
                    Console.WriteLine($"Attempt {attempt}: LocalStack not ready. Retrying in {timeSpan.TotalSeconds} seconds...");
                }
            );

        var isReady = await retryPolicy.ExecuteAsync(async () =>
        {
            var dynamoDbReady = await awsConnection.CheckDynamoDbReadiness();
            var sqsReady = await awsConnection.CheckSqsReadiness();
            return dynamoDbReady && sqsReady;
        });

        if (!isReady)
        {
            throw new TimeoutException("LocalStack services did not become ready within the expected time.");
        }

        Console.WriteLine("LocalStack is ready.");
    }
}
