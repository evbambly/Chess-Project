using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    //Static service class to take care of legality of moves
    public static class MoveCheck
    {
        public static GameState IsGameStaleMate(Piece[,] board, bool whitePlayer, int movestoStaleMate, Dictionary<char[,], int> boardStatesCount)
        {
            /*
             * A stalemate is a situation in which one of the following is true:
             * 1. 50 plays have been made without moving a Pawn or capturing a piece
             * 2. The same board arrangement appears for the 3rd time
             * 3. Both players don't have enough material to win
             * 4. The player cannot legally move without being in Check
             */
            bool staleMate = movestoStaleMate > 0 && !boardStatesCount.ContainsValue(3) ? false : true;
            List<Tile> playerPieces = Chess.NewFindPlayerPieces(board, whitePlayer ? Color.white : Color.black);
            List<Tile> foePieces = Chess.NewFindPlayerPieces(board, whitePlayer ? Color.black : Color.white);
            List<Tile> neutralPieces = Chess.NewFindPlayerPieces(board, Color.neutral);
            //If both players don't have enough material to force a checkmate 
            //(King || King + Knight || King + Bishop) -> It is a stalemate
            if (!staleMate && playerPieces.Count + neutralPieces.Count < 3 && foePieces.Count + neutralPieces.Count < 3)
            {
                bool playerHasEnoughMaterial = false;
                bool foeHasEnoughMaterial = false;
                foreach (Tile piece in neutralPieces)
                {
                    if (board[piece.x, piece.y] is Rook || board[piece.x, piece.y] is Queen || board[piece.x, piece.y] is Pawn)
                    {
                        playerHasEnoughMaterial = true;
                        foeHasEnoughMaterial = true;
                    }
                }
                foreach (Tile piece in playerPieces)
                {
                    if (board[piece.x, piece.y] is Rook || board[piece.x, piece.y] is Queen || board[piece.x, piece.y] is Pawn)
                        playerHasEnoughMaterial = true;
                }
                foreach (Tile piece in foePieces)
                {
                    if (board[piece.x, piece.y] is Rook || board[piece.x, piece.y] is Queen || board[piece.x, piece.y] is Pawn)
                        foeHasEnoughMaterial = true;
                }
                if (!playerHasEnoughMaterial && !foeHasEnoughMaterial)
                {
                    staleMate = true;
                }
            }
            //Try to move all of the player's pieces to every tile on the board
            //If no piece can move legally without being in check- it's a staleMate
            if (!staleMate)
            {
                playerPieces.AddRange(neutralPieces);
                bool playerCannotMove = true;
                foreach (Tile piece in playerPieces)
                {
                    for (int i = 0; playerCannotMove && i < 8; i++)
                    {
                        for (int j = 0; playerCannotMove && j < 8; j++)
                        {
                            Tile tryMove = Chess.CreateTile(i, j);
                            if (IsMovePossible(board, whitePlayer, piece, tryMove))
                            {
                                playerCannotMove = false;
                            }
                        }
                    }
                }
                staleMate = playerCannotMove ? true : false;
            }
            return staleMate ? GameState.StaleMate : GameState.Play;
        }
        public static GameState IsGameCheckMate(Piece[,] board, Tile landedOnTile, bool whiteTurn)
        {
            /* CheckMate occurs if the following are true:
             * The player is in Check
             * The player has no legal move to exit the Check
             */
            bool checkMate = false;
            if (IsPlayerInCheck(board, landedOnTile, landedOnTile, whiteTurn))
            {
                checkMate = true;
                List<Tile> neutralPieces = Chess.NewFindPlayerPieces(board, Color.neutral);
                List<Tile> playerPieces = Chess.NewFindPlayerPieces(board, whiteTurn? Color.white : Color.black);
                playerPieces.AddRange(neutralPieces);
                foreach (Tile piece in playerPieces)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Tile tryMove = Chess.CreateTile(i, j);
                            if (IsMovePossible(board, whiteTurn, piece, tryMove))
                                checkMate = false;
                        }
                    }
                }
            }
            return checkMate ? GameState.CheckMate : GameState.Play;
        }
        public static void ChooseMove(Piece[,] board, bool flipBoard, bool whiteTurn, out Tile chosenFromTile, out Tile chosenToTile)
        {
            //Making sure the input is correct and the move is possible
            bool legalMove = false;
            do
            {
                string output = "";
                if (IsInputLegal(Console.ReadLine(), out Tile fromTile, out Tile toTile))
                {
                    if (IsMovePossible(board, whiteTurn, fromTile, toTile))
                    {
                        legalMove = true;
                    }
                    else
                    {
                        output = "Illegal Move!";
                    }
                }
                else
                {
                    output = "Invalid input!";
                }
                if (!legalMove)
                {
                    Console.Clear();
                    Chess.PrintGameGraphics(board, flipBoard);
                    Console.WriteLine(output);
                    Console.WriteLine($"It is {(whiteTurn ? "white" : "black")}'s turn to play");
                }
                chosenFromTile = fromTile;
                chosenToTile = toTile;
            } while (!legalMove);
        }
        private static bool IsInputLegal(string input, out Tile fromTile, out Tile toTile)
        {
            //An input is legal only if it is formatted A1B2, letters A-H, 1 - 8. Upper & Lower case are acceptable
            bool inputIsLegal = false;
            fromTile = Chess.CreateTile(-1, -1);
            toTile = Chess.CreateTile(-1, -1);
            input = input.ToUpper();
            if (input.Length == 4)
            {
                if (input[0] >= 'A' && input[0] < 'J')
                {
                    if (input[2] >= 'A' && input[2] < 'J')
                    {
                        if (int.TryParse(input[1].ToString(), out int a) && int.TryParse(input[3].ToString(), out int b))
                        {
                            if (a > 0 && a < 9 && b > 0 && b < 9)
                            {
                                inputIsLegal = true;
                                fromTile.y = input[0] - 'A';
                                fromTile.x = int.Parse(input[1].ToString()) - 1;
                                toTile.y = input[2] - 'A';
                                toTile.x = int.Parse(input[3].ToString()) - 1;
                            }
                        }
                    }

                }
            }
            return inputIsLegal;
        }
        public static bool IsMovePossible(Piece[,] board, bool whiteTurn, Tile fromTile, Tile toTile)
        {
            //An assisting method, 
            //testing whether a given move is legal in four ways:
            //Sane, Unobstructed, Legal or SecondMove and !Check
            bool moveIsPossible = false;
            Piece chosenPiece = board[fromTile.x, fromTile.y];
            if (IsMoveSane(whiteTurn, chosenPiece, board[toTile.x, toTile.y]))
            {
                if (IsMoveUnobstructed(board, fromTile, toTile))
                {
                    if (chosenPiece.IsMoveLegal(fromTile, toTile) | chosenPiece.SecondMove(board, fromTile, toTile))
                    {
                        moveIsPossible = IsPlayerInCheck(board, fromTile, toTile, whiteTurn) ? false : true;
                    }
                }
            }
            return moveIsPossible;
        }
        private static bool IsMoveSane(bool whiteTurn, Piece fromPiece, Piece toPiece)
        {
            //A sane move starts from the player's piece 
            //and finishes in an empty or foe tile (anything but the player's piece)
            Color foe = whiteTurn ? Color.black : Color.white;
            Color friend = whiteTurn ? Color.white : Color.black;
            bool moveIsSane = fromPiece.color != foe && !(fromPiece is EmptyTile);
            return moveIsSane && toPiece.color != friend;
        }
        public static bool IsPlayerInCheck(Piece[,] importedBoard, Tile fromTile, Tile toTile, bool whiteTurn)
        {
            //A player is in Check if one or more of the foe's pieces can legally capture their King
            Piece[,] board = ImportBoard(importedBoard);
            Color player = board[fromTile.x, fromTile.y].color;
            bool playerInCheck = false;
            board = Chess.AmendBoard(board, fromTile, toTile);
            Tile king = Chess.FindKing(board, whiteTurn);
            List<Tile> foePieces = Chess.NewFindPlayerPieces(board, whiteTurn ? Color.black : Color.white);
            List<Tile> neutralPieces = Chess.NewFindPlayerPieces(board, Color.neutral);
            foePieces.AddRange(neutralPieces);
            //When checking for the foe's move, 
            //the exported stats must include the opposite of the current turn
            foreach (Tile foe in foePieces)
            {
                if (IsMoveUnobstructed(board, foe, king))
                {
                    if (board[foe.x, foe.y].IsMoveLegal(foe, king) | board[foe.x, foe.y].SecondMove(board, foe, king))
                    {
                        playerInCheck = true;
                    }
                }

            }
            return playerInCheck;
        }
        public static bool IsMoveUnobstructed(Piece[,] board, Tile fromTile, Tile toTile)
        {
            //Moves that can be obstructed are only those where (x == y)
            //Or where one of them is 0. Otherwise, there is no need to test obstruction
            int tilesInBetween;
            //Moves either are in both the X && Y axes for the same distance
            //Or they occur only on one of axes            
            int xDistance = Math.Abs(toTile.x - fromTile.x);
            int yDistance = Math.Abs(toTile.y - fromTile.y);
            if (xDistance != yDistance && xDistance != 0 && yDistance != 0)
            {
                tilesInBetween = 0;
            }
            else
            {
                tilesInBetween = xDistance > yDistance ? xDistance - 1 : yDistance - 1;
            }
            int xAdvance = toTile.x - fromTile.x > 0 ? 1 : 0;
            if (toTile.x - fromTile.x < 0)
            {
                xAdvance = -1;
            }
            int yAdvance = toTile.y - fromTile.y > 0 ? 1 : 0;
            if (toTile.y - fromTile.y < 0)
            {
                yAdvance = -1;
            }
            bool moveUnobstructed = true;
            Tile checkEmptyTile = fromTile;
            for (int i = 0; i < tilesInBetween; i++)
            {
                checkEmptyTile.y += yAdvance;
                checkEmptyTile.x += xAdvance;
                if (!(board[checkEmptyTile.x, checkEmptyTile.y] is EmptyTile))
                {
                    moveUnobstructed = false;
                }
            }
            return moveUnobstructed;
        }
        private static Piece[,] ImportBoard(Piece[,] importedBoard)
        {
            //Copying the board in order to not backpropagate changes
            Piece[,] board = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = importedBoard[i, j];
                }
            }
            return board;
        }
    }
}