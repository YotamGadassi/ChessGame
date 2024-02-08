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
        event Action<ConnectionErrorType> ConnectionErrorEvent;
        event Func<Exception?, Task>      ConnectionClosedEvent;
        ConnectionStatus                  ConnectionStatus { get; }

        Task<bool> Connect();
        Task       Disconnect();
    }

    public interface IConnectionManager<out T> : IConnectionManager
    {
        T ServerAgent { get; }
    }
}
