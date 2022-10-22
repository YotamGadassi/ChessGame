using System.Reflection;
using ChessServer3._0;
using log4net.Config;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

builder.Logging.AddLog4Net("loggingConfiguration.xml");

builder.Services.Add(new ServiceDescriptor(typeof(IServerState), typeof(ServerState), ServiceLifetime.Singleton));
WebApplication                       app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHub<ChessHub>("/ChessHub");
app.Run();

