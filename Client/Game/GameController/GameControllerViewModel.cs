using System.Windows.Input;
using Common;
using FrontCommon;

namespace Client.Game;

public class GameControllerViewModel : BaseGameControllerViewModel
{
    public override ICommand StartResume { get; }
    public override ICommand EndGame     { get; }
    public override ICommand Pause       { get; }

    private IGameManager m_gameManager;

    public GameControllerViewModel(IGameManager gameManager)
    {
        m_gameManager              =  gameManager;
        StartResume                =  new WpfCommand(onStartResumeExecute, onStartResumeCanExecute);
        Pause                      =  new WpfCommand(onPauseExecute,       onPauseCanExecute);
        EndGame                    =  new WpfCommand(onEndGameExecute,     onEndGameCanExecute);
        GameState                  =  gameManager.State;
        m_gameManager.StateChanged += onStateChanged;
    }

    private void onStartResumeExecute(object state)
    {
        m_gameManager.StartResumeGame();
    }

    private bool onStartResumeCanExecute(object state)
    {
        return isGameNotEnded()
            && m_gameManager.State != GameState.Running;
    }

    private void onPauseExecute(object state)
    {
        m_gameManager.PauseGame();
    }

    private bool onPauseCanExecute(object state)
    {
        return m_gameManager.State == GameState.Running;
    }

    private void onEndGameExecute(object state)
    {
        m_gameManager.EndGame();
    }

    private bool onEndGameCanExecute(object state)
    {
        return m_gameManager.State != GameState.Ended;
    }

    private void onStateChanged(object?   sender
                              , GameState newGameState)
    {
        GameState = newGameState;
    }

    private bool isGameNotEnded()
    {
        return GameState != GameState.Ended;
    }
}