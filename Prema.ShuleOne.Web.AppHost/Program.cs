var builder = DistributedApplication.CreateBuilder(args);

var databaseName = "shuleone-db"; // MySql database & table names are case-sensitive on non-Windows.

//missing.nrt 9 version for pomelo
//var mysql = builder.AddMySql("mysql")
//    // Set the name of the database to auto-create on container startup.
//    .WithEnvironment("MYSQL_DATABASE", databaseName)
//    // Mount the SQL scripts directory into the container so that the init scripts run.
//    .WithBindMount("../DatabaseContainers.ApiService/data/mysql", "/docker-entrypoint-initdb.d")
//    // Configure the container to store data in a volume so that it persists across instances.
//    .WithDataVolume()
//    // Keep the container running between app host sessions.
//    .WithLifetime(ContainerLifetime.Persistent)
//    .WithPhpMyAdmin();

//// Add the database to the application model so that it can be referenced by other resources.
//var catalogDb = mysql.AddDatabase(databaseName);

var postgres = builder.AddPostgres("postgres")
    // Set the name of the default database to auto-create on container startup.
    .WithEnvironment("POSTGRES_DB", databaseName)
    // Mount the SQL scripts directory into the container so that the init scripts run.
    .WithBindMount("../DatabaseContainers.ApiService/data/postgres", "/docker-entrypoint-initdb.d")
    // Configure the container to store data in a volume so that it persists across instances.
    .WithDataVolume()
    // Keep the container running between app host sessions.
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

// Add the default database to the application model so that it can be referenced by other resources.
var todosDb = postgres.AddDatabase(databaseName);

var rabbitmq = builder.AddRabbitMQ("shuleone-rabbitmq")
    .WithManagementPlugin();

builder.AddProject<Projects.Prema_ShuleOne_Web_Server>("prema-shuleone-web-server")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.prema_shuleone_web_client>("prema-shuleone-web-client");

builder.AddProject<Projects.Prema_Services_UnifiedNotifier>("prema-services-unifiednotifier")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
