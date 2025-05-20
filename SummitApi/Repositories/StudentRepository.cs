using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SummitApi.Models;

namespace SummitApi.Repositories;

public interface IStudentRepository
{
    Task<RepositoryOperation<Student>> GetStudent(string studentId, CancellationToken cancellationToken);
}

public class StudentRepository(AmazonDynamoDBClient dynamoDbClient) : IStudentRepository
{
    public async Task<RepositoryOperation<Student>> GetStudent(string studentId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(studentId))
        {
            return new RepositoryOperation<Student>.Failure("StudentId is required!");
        }

        var getItemRequest = new GetItemRequest
        {
            TableName = "all-students",
            Key = new Dictionary<string, AttributeValue>
            {
                { "StudentId", new AttributeValue { S = studentId } },
            },
        };

        try
        {
            var getItemResponse = await dynamoDbClient.GetItemAsync(getItemRequest, cancellationToken);

            if (getItemResponse.HttpStatusCode != HttpStatusCode.OK || !getItemResponse.IsItemSet)
            {
                return new RepositoryOperation<Student>.Failure("Student not found!");
            }
            
            return new RepositoryOperation<Student>.Success(new Student(
                getItemResponse.Item["StudentId"].S,
                getItemResponse.Item["Name"].S,
                getItemResponse.Item["Email"].S));
        }
        catch (Exception ex)
        {
            return new RepositoryOperation<Student>.Error(ex);
        }
    }
}