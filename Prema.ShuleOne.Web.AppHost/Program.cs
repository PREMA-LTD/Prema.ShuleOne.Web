using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithRedisInsight();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var dbmanager = builder.AddProject<Projects.Prema_Services_ShuleOneDbManager>("dbmanager")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Prema_ShuleOne_Web_Server>("web-server")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(dbmanager);

builder.AddProject<Projects.prema_shuleone_web_client>("client");

builder.AddProject<Projects.Prema_Services_UnifiedNotifier>("unified-notifier")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.Prema_Services_StorageHub>("storagehub")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(redis)
    .WaitFor(redis);

builder.Build().Run();
