﻿using System.Windows.Input;
using Common;
using FrontCommon;

namespace Client.Game;

public class GameControllerViewModel : BaseGameControllerViewModel
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
        m_gameController.StateChanged += onStateChanged;
    }

    private void onStartResumeExecute(object state)
    {
        m_gameController.StartResumeGame();
    }

    private bool onStartResumeCanExecute(object state)
    {
        return isGameNotEnded()
            && m_gameController.State != GameState.Running;
    }

    private void onPauseExecute(object state)
    {
        m_gameController.PauseGame();
    }

    private bool onPauseCanExecute(object state)
    {
        return m_gameController.State == GameState.Running;
    }

    private void onEndGameExecute(object state)
    {
        m_gameController.EndGame();
    }

    private bool onEndGameCanExecute(object state)
    {
        return m_gameController.State != GameState.Ended;
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