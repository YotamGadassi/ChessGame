using ChessBoard;
using Common;
using System;
using System.Collections.Generic;
using Tools;

namespace ChessGame
{
    public static class GameInitHelper
    {
        public static IList<KeyValuePair<BoardPosition, ITool>> GeneratePawns(Team team)
        {
            int pawnsAmount = 8;

            int yAxis = 0;
            switch (team.MoveDirection)
            {
                case GameDirection.Forward:
                    yAxis = 1;
                    break;
                case GameDirection.Backward:
                    yAxis = 6;
                    break;
            }

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            for (int i = 0; i < pawnsAmount; ++i)
            {
                BoardPosition newPosition = new BoardPosition(i, yAxis);
                Pawn newPawn = new Pawn(team.Color);
                KeyValuePair<BoardPosition, ITool> newPair = new KeyValuePair<BoardPosition, ITool>(newPosition, newPawn);
                list.Add(newPair);
            }

            return list;
        }

        public static IList<KeyValuePair<BoardPosition, ITool>> GenerateRooks(Team team)
        {
            int yAxis = GetFirstRow(team.MoveDirection);

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            BoardPosition position = new BoardPosition(0, yAxis);
            ITool tool = new Rook(team.Color);
            KeyValuePair<BoardPosition, ITool> pair = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            position = new BoardPosition(7, yAxis);
            tool = new Rook(team.Color);
            pair = new KeyValuePair<BoardPosition, ITool>(position, tool);

            list.Add(pair);

            return list;
        }

        public static IList<KeyValuePair<BoardPosition,ITool>> GenerateBishops(Team team)
        {
            int yAxis = GetFirstRow(team.MoveDirection);

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            BoardPosition position = new BoardPosition(2, yAxis);
            ITool tool = new Bishop(team.Color);
            KeyValuePair<BoardPosition, ITool> pair = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            position = new BoardPosition(5, yAxis);
            tool = new Bishop(team.Color);
            pair = new KeyValuePair<BoardPosition, ITool>(position, tool);

            list.Add(pair);

            return list;
        }

        public static IList<KeyValuePair<BoardPosition, ITool>> GenerateKnights(Team team)
        {
            int yAxis = GetFirstRow(team.MoveDirection);

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            BoardPosition position = new BoardPosition(1, yAxis);
            ITool tool = new Knight(team.Color);
            KeyValuePair<BoardPosition, ITool> pair = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            position = new BoardPosition(6, yAxis);
            tool = new Knight(team.Color);
            pair = new KeyValuePair<BoardPosition, ITool>(position, tool);

            list.Add(pair);

            return list;
        }

        public static IList<KeyValuePair<BoardPosition, ITool>> GenerateQueenKing(Team team)
        {
            int yAxis = GetFirstRow(team.MoveDirection);

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            BoardPosition position = new BoardPosition(3, yAxis);
            ITool tool = new Queen(team.Color);
            KeyValuePair<BoardPosition, ITool> pair = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            position = new BoardPosition(4, yAxis);
            tool = new King(team.Color);
            pair = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            return list;
        }

        private static int GetFirstRow(GameDirection moveDirection)
        {
            switch (moveDirection)
            {
                case GameDirection.Forward:
                    return 0;
                case GameDirection.Backward:
                    return 7;
                default:
                    throw new ArgumentException(string.Format("Move direction: {0} is not allowed", moveDirection));
            }
        }
    }
}
