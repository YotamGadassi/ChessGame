﻿using System.Windows.Media;
using Board;
using Common;
using Tools;

namespace ChessGame
{
    public static class GameInitHelper
    {
        public static KeyValuePair<BoardPosition, ITool>[] GenerateInitialArrangement(GameDirection direction
          , Color                                                                             color)
        {
            IList<KeyValuePair<BoardPosition, ITool>> pawnList      = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> rookList      = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> bishopList    = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> knightList    = new List<KeyValuePair<BoardPosition, ITool>>();
            IList<KeyValuePair<BoardPosition, ITool>> queenKingList = new List<KeyValuePair<BoardPosition, ITool>>();

            pawnList      = GameInitHelper.GeneratePawns(direction, color);
            rookList      = GameInitHelper.GenerateRooks(direction, color);
            bishopList    = GameInitHelper.GenerateBishops(direction, color);
            knightList    = GameInitHelper.GenerateKnights(direction, color);
            queenKingList = GameInitHelper.GenerateQueenKing(direction, color);

            IList<KeyValuePair<BoardPosition, ITool>> toolsList = concatanateLists(pawnList, rookList, bishopList, knightList, queenKingList);

            return toolsList.ToArray();
        }
        
        private static IList<KeyValuePair<BoardPosition, ITool>> concatanateLists(params IList<KeyValuePair<BoardPosition, ITool>>[] lists)
        {
            IList<KeyValuePair<BoardPosition, ITool>> toolsList = new List<KeyValuePair<BoardPosition, ITool>>();

            foreach (IList<KeyValuePair<BoardPosition,ITool>> list in lists)
            {
                toolsList = toolsList.Concat(list).ToList();
            }

            return toolsList;
        }

        public static IList<KeyValuePair<BoardPosition, ITool>> GeneratePawns(GameDirection direction, Color color)
        {
            int pawnsAmount = 8;

            int yAxis = 0;
            switch (direction)
            {
                case GameDirection.North:
                    yAxis = 2;
                    break;
                case GameDirection.South:
                    yAxis = 7;
                    break;
            }

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            for (int i = 1; i <= pawnsAmount; ++i)
            {
                BoardPosition newPosition = new BoardPosition(i, yAxis);
                Pawn newPawn = new Pawn(color);
                KeyValuePair<BoardPosition, ITool> newPair = new KeyValuePair<BoardPosition, ITool>(newPosition, newPawn);
                list.Add(newPair);
            }

            return list;
        }

        public static IList<KeyValuePair<BoardPosition, ITool>> GenerateRooks(GameDirection direction, Color color)
        {
            int yAxis = GetFirstRow(direction);

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            BoardPosition                      position = new BoardPosition(1, yAxis);
            ITool                              tool     = new Rook(color);
            KeyValuePair<BoardPosition, ITool> pair     = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            position = new BoardPosition(8, yAxis);
            tool     = new Rook(color);
            pair     = new KeyValuePair<BoardPosition, ITool>(position, tool);

            list.Add(pair);

            return list;
        }

        public static IList<KeyValuePair<BoardPosition,ITool>> GenerateBishops(GameDirection direction, Color color)
        {
            int yAxis = GetFirstRow(direction);

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            BoardPosition                      position = new BoardPosition(3, yAxis);
            ITool                              tool     = new Bishop(color);
            KeyValuePair<BoardPosition, ITool> pair     = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            position = new BoardPosition(6, yAxis);
            tool     = new Bishop(color);
            pair     = new KeyValuePair<BoardPosition, ITool>(position, tool);

            list.Add(pair);

            return list;
        }

        public static IList<KeyValuePair<BoardPosition, ITool>> GenerateKnights(GameDirection direction, Color color)
        {
            int yAxis = GetFirstRow(direction);

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            BoardPosition                      position = new BoardPosition(2, yAxis);
            ITool                              tool     = new Knight(color);
            KeyValuePair<BoardPosition, ITool> pair     = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            position = new BoardPosition(7, yAxis);
            tool     = new Knight(color);
            pair     = new KeyValuePair<BoardPosition, ITool>(position, tool);

            list.Add(pair);

            return list;
        }

        public static IList<KeyValuePair<BoardPosition, ITool>> GenerateQueenKing(GameDirection direction, Color color)
        {
            int yAxis = GetFirstRow(direction);

            IList<KeyValuePair<BoardPosition, ITool>> list = new List<KeyValuePair<BoardPosition, ITool>>();

            BoardPosition                      position = new(5, yAxis);
            ITool                              tool     = new Queen(color);
            KeyValuePair<BoardPosition, ITool> pair     = new(position, tool);
            list.Add(pair);

            position = new BoardPosition(4, yAxis);
            tool     = new King(color);
            pair     = new KeyValuePair<BoardPosition, ITool>(position, tool);
            list.Add(pair);

            return list;
        }

        private static int GetFirstRow(GameDirection moveDirection)
        {
            switch (moveDirection)
            {
                case GameDirection.North:
                    return 1;
                case GameDirection.South:
                    return 8;
                default:
                    throw new ArgumentException(string.Format("Move direction: {0} is not allowed", moveDirection));
            }
        }
    }
}
