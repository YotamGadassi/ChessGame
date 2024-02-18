using System;
using System.Windows.Input;
using Common;
using FrontCommon;

namespace Client.Game;

public class GameControllerViewModel : BaseGameControllerViewModel, IDisposable
{
    public override ICommand StartResume { get; }
    public override ICommand EndGame     { get; }
    public override ICommand Pause       { get; }

    private IGameStateController m_gameController;

    public GameControllerViewModel(IGameStateController gameController)
    {
        m_gameController              =  gameController;
        StartResume                =  new WpfCommand(onStartResumeExecute, onStartResumeCanExecute);
        Pause                      =  new WpfCommand(onPauseExecute,       onPauseCanExecute);
        EndGame                    =  new WpfCommand(onEndGameExecute,     onEndGameCanExecute);
        GameState                  =  gameController.State;
        registerToEvents();
    }

    public void Dispose()
    {
        unRegisterFromEvents();
    }

    private void registerToEvents()
    {
        m_gameController.StateChanged += onStateChanged;
    }

    private void unRegisterFromEvents()
    {
        m_gameController.StateChanged -= onStateChanged;
    }

    private void onStartResumeExecute(object state)
    {
        m_gameController.StartResumeGame();
    }

    private bool onStartResumeCanExecute(object state)
    {
        return isGameNotEnded()
            && m_gameController.State != GameStateEnum.Running;
    }

    private void onPauseExecute(object state)
    {
        m_gameController.PauseGame();
    }

    private bool onPauseCanExecute(object state)
    {
        return m_gameController.State == GameStateEnum.Running;
    }

    private void onEndGameExecute(object state)
    {
        m_gameController.EndGame();
    }

    private bool onEndGameCanExecute(object state)
    {
        return m_gameController.State != GameStateEnum.Ended;
    }

    private void onStateChanged(object?   sender
                              , GameStateEnum newGameState)
    {
        GameState = newGameState;
    }

    private bool isGameNotEnded()
    {
        return GameState != GameStateEnum.Ended;
    }


}