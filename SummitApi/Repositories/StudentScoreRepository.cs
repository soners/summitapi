using SummitApi.Models;

namespace SummitApi.Repositories;

public interface IStudentScoreRepository
{
    Task<RepositoryOperation<StudentScoreModel>> GetStudentScoreAsync(string studentId, CancellationToken cancellationToken);  
}

public class StudentScoreRepository(HttpClient httpClient, IConfiguration configuration) : IStudentScoreRepository
{
    public async Task<RepositoryOperation<StudentScoreModel>> GetStudentScoreAsync(string studentId, CancellationToken cancellationToken)
    {
        try
        {
            var studentScoreApiBaseUrl = configuration["StudentScoreApi:BaseUrl"];
            var response = await httpClient.GetAsync($"{studentScoreApiBaseUrl}/api/students/{studentId}/score", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var studentScore = await response.Content.ReadFromJsonAsync<StudentScoreModel>(cancellationToken);
                
                return new RepositoryOperation<StudentScoreModel>.Success(studentScore);
            }

            return new RepositoryOperation<StudentScoreModel>.Failure("FAILED_OPERATION");
        }
        catch (Exception ex)
        {
            return new RepositoryOperation<StudentScoreModel>.Error(ex);
        }
    }
}