using ChessServer3._0;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
WebApplication app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHub<ChessHub>("/ChessHub");

app.Run();
