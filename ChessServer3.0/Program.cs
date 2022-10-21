using ChessServer3._0;
using Microsoft.AspNetCore.SignalR;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.Add(new ServiceDescriptor(typeof(IServerState), typeof(ServerState), ServiceLifetime.Singleton));
WebApplication                       app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHub<ChessHub>("/ChessHub");
app.Run();
