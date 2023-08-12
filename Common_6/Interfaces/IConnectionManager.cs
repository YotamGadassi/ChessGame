using System;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public enum ConnectionStatus
    {
        Connected,
        Disconnected,
        Degraded
    }

    public enum ConnectionErrorType
    {

    }

    public interface IConnectionManagerBase<out T>
    {
        event Action<ConnectionErrorType> ConnectionError;

        event Action<ConnectionStatus> ConnectionStatusChanged;

        T ConnectionHandler { get; }

        ConnectionStatus ConnectionStatus { get; }
    }

    public interface IConnectionManager<out T> : IConnectionManagerBase<T>
    {
        bool Connect();
        void Disconnect();
    }

    public interface IAsyncConnectionManager<out T> : IConnectionManagerBase<T>
    {
        Task<bool> Connect();
        Task       Disconnect();
    }
}
