var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TaskManager_Api>("taskmanager-api");
builder.AddProject<Projects.TaskManager_Web_Blazor>("web-blazor");

builder.Build().Run();
