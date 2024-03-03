using ChessServer.ServerManager;
using Common;
using Tools;

namespace ChessServer;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();

        builder.Logging.AddLog4Net("loggingConfiguration.xml");

        builder.Services.Add(new ServiceDescriptor(typeof(IServerFacade<string>), typeof(SignalRServerFacade)
                                                 , ServiceLifetime.Singleton));
        WebApplication app = builder.Build();

        app.MapGet("/", () => "Hello World!");
        app.MapHub<ChessHub>("/ChessHub");
        app.Run();
    }
}