using ChessServer;
using ChessServer.ServerManager;
using Common;
using Tools;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR()
       .AddJsonProtocol(option =>
                        {
                            option.PayloadSerializerOptions.Converters.Add(new IToolConverter());
                        });

builder.Logging.AddLog4Net("loggingConfiguration.xml");

builder.Services.Add(new ServiceDescriptor(typeof(IServerManager<string>), typeof(SignalRServerManager), ServiceLifetime.Singleton));
WebApplication                       app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHub<ChessHub>("/ChessHub");
app.Run();

