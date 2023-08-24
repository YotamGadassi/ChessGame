using System;
using System.Threading.Tasks;

namespace FrontCommon
{
    public enum ConnectionStatus
    {
        Connecting,
        Reconnecting,
        Connected,
        Disconnected
    }

    public enum ConnectionErrorType { }

    public interface IConnectionManager
    {
        event Action<ConnectionErrorType> ConnectionError;
        event Func<Exception?, Task>      ConnectionClosed;
        ConnectionStatus                  ConnectionStatus { get; }

        Task<bool> Connect();
        Task       Disconnect();
    }

    public interface IConnectionManager<out T> : IConnectionManager
    {
        T ServerAgent { get; }
    }
}
