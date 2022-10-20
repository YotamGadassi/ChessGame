using ChessServer3._0;
using Microsoft.AspNetCore.SignalR;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args)
builder.Services.AddSignalR();
WebApplication                       app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHub<ChessHub>("/ChessHub");

IHubContext<ChessHub> hubContext = app.Services.GetService<IHubContext<ChessHub>>();
ServerFacade.HubContext = hubContext;
app.Run();
