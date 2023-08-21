using Common.MainWindow;

namespace FrontCommon.Facade
{
    public abstract class BaseGameFacade
    {
        public static IGameFacade Instance { get; protected set; }
    }
}