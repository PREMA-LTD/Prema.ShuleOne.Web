var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Prema_ShuleOne_Web_Server>("prema-shuleone-web-server");

builder.AddProject<Projects.prema_shuleone_web_client>("prema-shuleone-web-client");

builder.AddProject<Projects.Prema_Services_UnifiedNotifier>("prema-services-unifiednotifier");

builder.Build().Run();
