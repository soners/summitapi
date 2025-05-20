namespace SummitApi.AppHost;

public static class LocalStackResourceExtensions
{
    public static IResourceBuilder<ProjectResource> WithLocalStackReference(
        this IResourceBuilder<ProjectResource> builder, 
        IResourceBuilder<ContainerResource> source)
    {
        builder.WithEnvironment("AWS_ACCESS_KEY_ID", "test");
        builder.WithEnvironment("AWS_SECRET_ACCESS_KEY", "test");
        builder.WithEnvironment("AWS_REGION", "eu-central-1");
        builder.WithEnvironment("AWS_DEFAULT_REGION", "eu-central-1");
        builder.WithEnvironment("AWS_ENDPOINT_URL", source.GetEndpoint("http"));
        builder.WithEnvironment(
            "SUMMITAPI_AWS__Sns__TeacherStudentPairRequestedTopicArn", 
            "arn:aws:sns:eu-central-1:000000000000:teacher-student-pair-requested");

        return builder;
    }
}