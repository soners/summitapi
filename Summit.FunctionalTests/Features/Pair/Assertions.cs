using System.Net.Http.Json;

namespace Summit.FunctionalTests.Features.Pair;

public static class Assertions
{
    public static async Task<string> ShouldBeFailureResponse(this HttpResponseMessage httpResponseMessage)
    {
        Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        
        var content = await httpResponseMessage.Content.ReadFromJsonAsync<string>();
        
        Assert.False(string.IsNullOrWhiteSpace(content));

        return content;
    }
}