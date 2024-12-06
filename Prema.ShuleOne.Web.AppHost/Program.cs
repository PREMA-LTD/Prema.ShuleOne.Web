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
//var mysqldb = mysql.AddDatabase(databaseName);

//var postgres = builder.AddPostgres("postgres")
//    // Set the name of the default database to auto-create on container startup.
//    .WithEnvironment("POSTGRES_DB", databaseName)
//    // Mount the SQL scripts directory into the container so that the init scripts run.
//    .WithBindMount("../DatabaseContainers.ApiService/data/postgres", "/docker-entrypoint-initdb.d")
//    // Configure the container to store data in a volume so that it persists across instances.
//    // Keep the container running between app host sessions.
//    .WithLifetime(ContainerLifetime.Persistent)
//    .WithPgAdmin();

//var postgres = builder.AddPostgres("postgres")
//    .WithPgAdmin()
//    .WithLifetime(ContainerLifetime.Persistent);

//if (builder.ExecutionContext.IsRunMode)
//{
//    // Data volumes don't work on ACA for Postgres so only add when running
//    postgres.WithDataVolume();
//}


//// Add the default database to the application model so that it can be referenced by other resources.
//var postgresqldb = postgres.AddDatabase(databaseName);

//var sqlserver = builder.AddSqlServer("sqlserver")
//    // Mount the init scripts directory into the container.
//    .WithBindMount("./sqlserverconfig", "/usr/config")
//    // Mount the SQL scripts directory into the container so that the init scripts run.
//    .WithBindMount("../DatabaseContainers.ApiService/data/sqlserver", "/docker-entrypoint-initdb.d")
//    // Run the custom entrypoint script on startup.
//    .WithEntrypoint("/usr/config/entrypoint.sh")
//    // Configure the container to store data in a volume so that it persists across instances.
//    .WithDataVolume()
//    // Keep the container running between app host sessions.
//    .WithLifetime(ContainerLifetime.Persistent);

//// Add the database to the application model so that it can be referenced by other resources.
//var sqlserverDb = sqlserver.AddDatabase(databaseName);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var dbmanager = builder.AddProject<Projects.Prema_Services_ShuleOneDbManager>("dbmanager")
    //.WithReference(postgresqldb)
    //.WaitFor(postgresqldb)
    .WithHttpHealthCheck("/health");
//.WithHttpsCommand("/reset-db", "Reset Database", iconName: "DatabaseLightning");

builder.AddProject<Projects.Prema_ShuleOne_Web_Server>("web-server")
    //.WithReference(postgresqldb)
    //.WaitFor(postgresqldb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(dbmanager);

builder.AddProject<Projects.prema_shuleone_web_client>("client");

builder.AddProject<Projects.Prema_Services_UnifiedNotifier>("unified-notifier")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

//builder.AddProject<Projects.Prema_Services_MigrationService>("migration-service")
//    .WithReference(postgresqldb)
//    .WaitFor(postgresqldb);


builder.Build().Run();
