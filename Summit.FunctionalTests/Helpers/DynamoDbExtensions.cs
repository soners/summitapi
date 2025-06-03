using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;

namespace Summit.FunctionalTests.Helpers;

public static class DynamoDbExtensions
{
    public static async Task<bool> CheckDynamoDbReadiness(this AwsConnection awsConnection)
    {
        var dynamoDbClient = CreateAmazonDynamoDbClient(awsConnection);

        var describeTableRequest = new DescribeTableRequest
        {
            TableName = "all-students"
        };

        try
        {
            var response = await dynamoDbClient.DescribeTableAsync(describeTableRequest);

            return response.HttpStatusCode.Equals(HttpStatusCode.OK);
        }
        catch
        {
            return false;
        }
    } 

    public static async Task InsertStudent(this AwsConnection awsConnection, string studentId, string name, string email)
    {
        var dynamoDbClient = CreateAmazonDynamoDbClient(awsConnection);

        var putItemRequest = new PutItemRequest
        {
            TableName = "all-students",
            Item = new Dictionary<string, AttributeValue>
            {
                { "StudentId", new AttributeValue { S = studentId } },
                { "Name", new AttributeValue { S = name } },
                { "Email", new AttributeValue { S = email } },
            }
        };
        
        await dynamoDbClient.PutItemAsync(putItemRequest);
    }

    private static AmazonDynamoDBClient CreateAmazonDynamoDbClient(AwsConnection awsConnection)
    {
        var dynamoDbConfig = new AmazonDynamoDBConfig
        {
            EndpointProvider = new StaticEndpointProvider(awsConnection.Url),
            RegionEndpoint = RegionEndpoint.EUCentral1,
            AuthenticationRegion = RegionEndpoint.EUCentral1.SystemName,
        };
        
        var awsCredentials = new SessionAWSCredentials("test", "test", "test");
        return new AmazonDynamoDBClient(awsCredentials, dynamoDbConfig);
    }
}