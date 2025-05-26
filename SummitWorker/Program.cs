using Amazon;
using Amazon.SQS;
using SummitWorker;
using SummitWorker.Handler;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

configuration.AddEnvironmentVariables("SUMMITWORKER_");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Services.AddSingleton(_ =>
{
    var sqsConfig = new AmazonSQSConfig
    {
        AuthenticationRegion = configuration["AWS:Region"],
        RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]),
    };

    return new AmazonSQSClient(sqsConfig);
});

// Wait for 10 seconds to start polling
Thread.Sleep(10 * 1000);

builder.Services.AddAWSMessageBus(busBuilder =>
{
    busBuilder.AddMessageHandler<PairMessageHandler, PairMessage>("summitapi");

    busBuilder.AddSQSPoller("http://sqs.eu-central-1.localhost.localstack.cloud:4566/000000000000/teacher-student-pair-requested-queue");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
