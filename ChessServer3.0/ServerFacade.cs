using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0
{
    public static class ServerFacade
    {
        public static IHubContext<ChessHub> HubContext { get; set; }
    }
}
