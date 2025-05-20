using System.Text;
using System.Text.Json;
using Summit.FunctionalTests.Features.Pair;

namespace Summit.FunctionalTests.Helpers;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PairTeacherStudent(
        this HttpClient client,
        string teacherId,
        string studentId)
    {
        var pairRequest = new PairRequest(teacherId, studentId);

        HttpContent content =
            new StringContent(JsonSerializer.Serialize(pairRequest), Encoding.UTF8, "application/json");

        return await client.PostAsync(
                $"/teachers/{pairRequest.TeacherId}/students/{pairRequest.StudentId}/pair", content);
    }
    
    public static async Task<HttpResponseMessage> InsertStudentScore(
        this HttpClient client,
        string studentId,
        int score)
    {
        return await client.PostAsync(
            $"/api/students/{studentId}/score?score={score}", new StringContent(string.Empty));
    }
}