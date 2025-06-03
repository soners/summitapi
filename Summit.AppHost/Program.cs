using SummitApi.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var localstack = builder.AddContainer("localstack", "localstack/localstack")
    .WithBindMount("../local/init-dynamodb.sh", "/etc/localstack/init/ready.d/init-dynamodb.sh")
    .WithBindMount("../local/init-sns.sh", "/etc/localstack/init/ready.d/init-sns.sh")
    .WithBindMount("../local/init-sqs.sh", "/etc/localstack/init/ready.d/init-sqs.sh")
    .WithHttpEndpoint(targetPort: 4566);

var mysql = builder.AddMySql("mysql")
    .WithBindMount("../local/init-db.sql", "/docker-entrypoint-initdb.d/1.sql")
    .AddDatabase("public");

var kafka = builder.AddKafka("kafka")
    .WithBindMount("../local/init-kafka.sh", "/tmp/create-topics.sh")
    .WithKafkaUI();

var studentScoreMockApi = builder.AddProject<Projects.Summit_Mocks_StudentScoreApi>("scoremockapi");

builder.AddProject<Projects.SummitApi>("summitapi")
    .WaitFor(mysql)
    .WithReference(mysql)
    .WaitFor(localstack)
    .WaitFor(studentScoreMockApi)
    .WithStudentScoreMockApiReference(studentScoreMockApi)
    .WithLocalStackReference(localstack);

builder.AddProject<Projects.SummitWorker>("summitworker")
    .WaitFor(localstack)
    .WithLocalStackReference(localstack)
    .WaitFor(kafka)
    .WithReference(kafka);

builder.Build().Run();