using Amazon;
using Amazon.DynamoDBv2;
using Amazon.SimpleNotificationService;
using MySqlConnector;
using SummitApi;
using SummitApi.Models;
using SummitApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

configuration.AddEnvironmentVariables("SUMMITAPI_");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Services.AddMySqlDataSource(configuration.GetConnectionString("public"));

builder.Services.AddSingleton(_ =>
{
    var clientConfig = new AmazonDynamoDBConfig
    {
        AuthenticationRegion = configuration["AWS:Region"],
        RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]),
    };
        
    return new AmazonDynamoDBClient(clientConfig);
});

builder.Services.AddSingleton(_ =>
{
    var clientConfig = new AmazonSimpleNotificationServiceConfig
    {
        AuthenticationRegion = configuration["AWS:Region"],
        RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]),
    };
        
    return new AmazonSimpleNotificationServiceClient(clientConfig);
});

builder.Services.AddSingleton<IStudentRepository, StudentRepository>();
builder.Services.AddSingleton<ITeacherRepository, TeacherRepository>();
builder.Services.AddSingleton<IStudentScoreRepository, StudentScoreRepository>();
builder.Services.AddSingleton<IPairRepository, PairRepository>();
builder.Services.AddSingleton<IPairHandler, PairHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/teachers/{teacherId}/students/{studentId}/pair", async (
        string teacherId,
        string studentId,
        CancellationToken cancellationToken,
        IPairHandler pairHandler) =>
    {
        var request = new PairRequest(studentId, teacherId);
        
        var result = await pairHandler.Handle(request, cancellationToken);

        return result switch
        {
            PairResponse.Success _ => Results.Accepted(),
            PairResponse.Failure failure => Results.BadRequest(failure.Reason),
            PairResponse.Error error => Results.InternalServerError(error.Exception.Message),
        };
    })
    .WithName("PairStudentTeacher");

app.Run();
