var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TaskManager_Api>("taskmanager-api");

builder.Build().Run();
