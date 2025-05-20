namespace SummitApi.AppHost;

public static class ResourceBuilderExtensions
{
    public static IResourceBuilder<ProjectResource> WithStudentScoreMockApiReference(
        this IResourceBuilder<ProjectResource> builder, 
        IResourceBuilder<ProjectResource> source)
    {
        builder.WithEnvironment("SUMMITAPI_StudentScoreApi__BaseUrl", source.GetEndpoint("http"));

        return builder;
    }
}