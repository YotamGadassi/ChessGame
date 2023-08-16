﻿using Board;

namespace FrontCommon
{
    public interface IAvailableMovesHelper
    {
        public bool ValidatePositionOnBoard(BoardPosition position);

        public BoardPosition[] GetAvailablePositionToMove(BoardPosition position);
    }

    public interface IAsyncAvailableMovesHelper
    {
        public Task<bool> ValidatePositionOnBoard(BoardPosition position);

        public Task<BoardPosition[]> GetAvailablePositionToMove(BoardPosition position);
    }
}