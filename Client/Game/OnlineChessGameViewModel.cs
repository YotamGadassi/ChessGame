using System;
using Board;
using Client.Board;
using Client.GameManagers;
using Common;
using FrontCommon;
using Tools;

namespace Client.Game
{
    public class OnlineChessGameViewModel : ChessGameViewModel
    {
        private OnlineChessGameManager m_gameManager;
        private IGameServerAgent m_gameServerAgent;

        public OnlineChessGameViewModel(OnlineChessGameManager gameManager, IGameServerAgent gameServerAgent) : base(gameManager)
        {
            m_gameManager = gameManager;
            m_gameServerAgent = gameServerAgent;
            m_gameManager.TeamsInitialized += onTeamsInitialized;
        }

        protected override async void onSqualeClickHandler(object?         sender
                                                         , SquareViewModel squareVM)
        {
            bool isLocalTeamTurn = await m_gameServerAgent.IsMyTurn();
            if (false == isLocalTeamTurn)
            {
                return;
            }

            ITool? tool = squareVM.Tool;
            BoardPosition position = squareVM.Position;

            bool           isPositionToolSameTeam = null != tool && tool.Color.Equals(m_gameManager.LocalMachineTeam.Color);
            if (isPositionToolSameTeam)
            {
                BoardViewModel.ClearSelectedAndHintedBoardPositions();
                BoardViewModel.SelectedBoardPosition = position;
                //BoardPosition[] positionToMove = await m_gameManager.AvailableMovesHelper.GetAvailablePositionToMove(position);
                //BoardViewModel.SetHintedBoardPosition(positionToMove);
                return;
            }

            if (false == BoardViewModel.SelectedBoardPosition.IsEmpty())
            {
                //MoveResult moveResult = await sendMoveRequest(board.SelectedBoardPosition, position);
                //handleMoveResult(moveResult);
            }
            BoardViewModel.ClearSelectedAndHintedBoardPositions();
        }

        protected override void onPromotionEvent(BoardPosition position
                                               , ITool         toolToPromote)
        {
            throw new NotImplementedException();
        }

        protected override void onCheckMateEvent(BoardPosition position
                                               , ITool         tool)
        {
            throw new NotImplementedException();
        }

        private void onTeamsInitialized(TeamWithTimer[] teams)
        {
            initTeams(teams[0], teams[1]);
        }
    }
}
