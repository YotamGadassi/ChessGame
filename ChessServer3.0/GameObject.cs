using System.Collections.Concurrent;
using System.Windows.Media;
using ChessGame;
using Common.ChessBoardEventArgs;
using Common_6;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer3._0;

public class GameUnit : IDisposable
{

    public GameUnit(string groupName)
    {
        GroupName            = Guid.NewGuid().ToString();
        CurrentGameVersion   = Guid.Empty;
        m_gameManager        = new OfflineGameManager();
        m_players            = new HashSet<PlayerObject>(2);
        m_blockingCollection = new BlockingCollection<Action>();
    }

    public  string          GroupName          { get; }
    public  PlayerObject[]  Players            => m_players.ToArray();
    public  Guid            CurrentGameVersion { get; private set; }
    private BaseGameManager m_gameManager;

    private HashSet<PlayerObject>      m_players;
    private BlockingCollection<Action> m_blockingCollection;
    private bool                       isRun  = false;
    private object                     m_lock = new object();
    public async void Init()
    {
        isRun = true;
        await Task.Run(runTasks);
    }

    private void runTasks()
    {
        while (isRun)
        {
            Action action = m_blockingCollection.Take();
            action.Invoke();
        }
    }

    public bool AddPlayer(PlayerObject player
                        , Hub          hub)
    {
        if (m_players.Count > 1 || m_players.Contains(player))
        {
            return false;
        }

        m_players.Add(player);
        hub.Groups.AddToGroupAsync(player.ConnectionId, GroupName);
        return true;
    }

    public void StartGame(Hub hub)
    {
        Action action = () =>
                        {
                            if (false == CurrentGameVersion.Equals(Guid.Empty))
                            {
                                return;
                            }

                            CurrentGameVersion = Guid.NewGuid();

                            if (m_players.Count != 2)
                            {
                                return;
                            }

                            m_gameManager.StartGame();
                            PlayerObject[] playersArr = m_players.ToArray();
                            hub.Clients.Client(playersArr[0].ConnectionId).SendAsync("StartGame", Colors.White);
                            hub.Clients.Client(playersArr[1].ConnectionId).SendAsync("StartGame", Colors.Black);
                        };
        m_blockingCollection.Add(action);
    }

    public void EndGame(Hub hub)
    {
        m_blockingCollection.Add(() => endGameSync(hub));
    }

    private void endGameSync(Hub hub)
    {
        if (CurrentGameVersion.Equals(Guid.Empty))
        {
            return;
        }

        CurrentGameVersion = Guid.Empty;

        m_gameManager.EndGame();
        PlayerObject[] playersArr = m_players.ToArray();
        foreach (PlayerObject player in playersArr)
        {
            hub.Groups.RemoveFromGroupAsync(player.ConnectionId, GroupName);
        }
    }

    public void Move(Hub           hub
                   , PlayerObject  player
                   , Guid          gameVersion
                   , BoardPosition start
                   , BoardPosition end)
    {
        Action action = () =>
                        {
                            if (false == m_players.Contains(player) ||
                                gameVersion.Equals(CurrentGameVersion))
                            {
                                return;
                            }

                            MoveResult result = m_gameManager.Move(start, end);
                            if (result.Result == MoveResultEnum.NoChangeOccurred)
                            {
                                return;
                            }

                            CurrentGameVersion = Guid.NewGuid();
                            switch (result.Result)
                            {
                                case MoveResultEnum.ToolMoved:
                                case MoveResultEnum.ToolKilled:
                                {
                                    hub.Clients.Group(GroupName)
                                       .SendAsync("Move", start, end, CurrentGameVersion);
                                    break;
                                }
                            }
                        };
        m_blockingCollection.Add(action);
    }

    public void Dispose()
    {
        Action action = () =>
                        {
                            isRun = false;
                            m_blockingCollection.Dispose();
                        };
        m_blockingCollection.Add(action);
    }
}