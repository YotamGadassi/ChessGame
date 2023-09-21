using System.Reflection;
using FrontCommon;
using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineChess;
using Tools;

namespace Frameworks
{
    public interface IChessConnctionManager : IConnectionManager<IChessServerAgent>
    {

    }

    public class SignalRConnectionManager : IChessConnctionManager
    {
        private static readonly ILog   s_log        = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string s_hubAddress = @"https://localhost:7034/ChessHub";

        public event Action<ConnectionErrorType>? ConnectionError;
        public event Func<Exception?, Task>?          ConnectionClosed;
        public IChessServerAgent                   ServerAgent      => m_gameServerAgent;
        public ConnectionStatus                   ConnectionStatus => convertHubConnectionState(m_connection.State);

        private GameServerAgent m_gameServerAgent;
        private HubConnection  m_connection;

        public SignalRConnectionManager()
        {
            m_connection = new HubConnectionBuilder().WithUrl(s_hubAddress)
                                                     .ConfigureLogging(builder => builder.AddLog4Net("LogConfiguration.xml"))
                                                     .AddJsonProtocol(options => options.PayloadSerializerOptions
                                                                                        .Converters
                                                                                        .Add(new IToolConverter()))
                                                     .Build();
            m_gameServerAgent = new GameServerAgent(m_connection);
        }

        public async Task<bool> Connect()
        {
            s_log.Debug("Connect");
            if (m_connection.State != HubConnectionState.Disconnected)
            {
                s_log.Info($"Connection state is {m_connection.State}. Cannot connect again.");
                return true;
            }

            m_connection.Closed += onConnectionClosed;
            s_log.Info($"Starting connection to client. server state:{m_connection.State}");

            try
            {
                await m_connection.StartAsync();
            }
            catch (Exception e)
            {
                s_log.Error(e.Message);
                return false;
            }

            m_gameServerAgent = new GameServerAgent(m_connection);
            s_log.Info($"connection to client completed. server state:{m_connection.State}");

            return true;
        }

        public async Task Disconnect()
        {
            s_log.Debug("Disconnect");
            await m_connection.StopAsync();
        }

        private Task onConnectionClosed(Exception? exception)
        {
            return ConnectionClosed?.Invoke(exception);
        }

        private static ConnectionStatus convertHubConnectionState(HubConnectionState connectionState)
        {
            switch (connectionState)
            {
                case HubConnectionState.Reconnecting:
                    return ConnectionStatus.Reconnecting;
                case HubConnectionState.Connecting:
                    return ConnectionStatus.Connecting;
                case HubConnectionState.Disconnected:
                    return ConnectionStatus.Disconnected;
                case HubConnectionState.Connected:
                    return ConnectionStatus.Connected;
                default:
                    return ConnectionStatus.Disconnected;
            }
        }

    }
}