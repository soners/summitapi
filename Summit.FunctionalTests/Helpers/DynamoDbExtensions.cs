using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;

namespace Summit.FunctionalTests.Helpers;

public static class DynamoDbExtensions
{
    public static async Task InsertStudent(this AwsConnection awsConnection, string studentId, string name, string email)
    {
        var dynamoDbConfig = new AmazonDynamoDBConfig
        {
            EndpointProvider = new StaticEndpointProvider(awsConnection.Url),
            RegionEndpoint = RegionEndpoint.EUCentral1,
            AuthenticationRegion = RegionEndpoint.EUCentral1.SystemName,
        };
        
        var awsCredentials = new SessionAWSCredentials("test", "test", "test");
        var dynamoDbClient = new AmazonDynamoDBClient(awsCredentials, dynamoDbConfig);

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
}