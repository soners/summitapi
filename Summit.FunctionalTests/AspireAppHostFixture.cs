using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Summit.FunctionalTests.Helpers;

namespace Summit.FunctionalTests;

public class AspireAppHostFixture : IAsyncLifetime
{
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
    
    public HttpClient HttpClient { get; private set; }
    
    public HttpClient StudentScoreApiClient { get; private set; }
    
    public MySqlConnection MysqlConnection { get; private set; }
    
    public AwsConnection AwsConnection { get; private set; }
    
    public string KafkaConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Summit_AppHost>();
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout = options.TotalRequestTimeout = new HttpTimeoutStrategyOptions()
                {
                    Timeout = TimeSpan.FromMinutes(2)
                };
            });
        });

        var app = await appHost.BuildAsync().WaitAsync(_defaultTimeout);
        await app.StartAsync().WaitAsync(_defaultTimeout);

        // Act
        await app.ResourceNotifications.WaitForResourceHealthyAsync("kafka").WaitAsync(_defaultTimeout);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("public").WaitAsync(_defaultTimeout);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("localstack").WaitAsync(_defaultTimeout);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("mysql").WaitAsync(_defaultTimeout);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("scoremockapi").WaitAsync(_defaultTimeout);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("summitapi").WaitAsync(_defaultTimeout);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("summitworker").WaitAsync(_defaultTimeout);

        HttpClient = app.CreateHttpClient("summitapi");
        
        StudentScoreApiClient = app.CreateHttpClient("scoremockapi");

        var mysqlConnectionString = await app.GetConnectionStringAsync("public");
        MysqlConnection = new MySqlConnection(mysqlConnectionString);
        
        await MysqlConnection.OpenAsync();

        if (appHost.Resources.FirstOrDefault(x => x.Name == "localstack").TryGetEndpoints(out var endpoints))
        {
            AwsConnection = new AwsConnection(endpoints.First().AllocatedEndpoint.UriString);
        }

        KafkaConnectionString = await app.GetConnectionStringAsync("kafka");
    }

    public async Task DisposeAsync()
    {
        // throw new NotImplementedException();
    }
}