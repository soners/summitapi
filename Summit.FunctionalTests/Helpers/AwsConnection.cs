namespace Summit.FunctionalTests.Helpers;

public class AwsConnection(string Url)
{
    public string Url { get; } = Url;
}