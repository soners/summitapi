using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;
using Amazon.SQS;
using Amazon.SQS.Model;

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
}