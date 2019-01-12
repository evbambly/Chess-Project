using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public struct Tile
    {
        //For distinguishing moves etc.
        public int x;
        public int y;
    }
    public enum GameState
    {
        Play, CheckMate, StaleMate
    }
    public class Chess
    {
        private Piece[,] board = new Piece[8, 8];
        private bool whiteTurn;
        private GameState gameState;
        //Switches between White on top or White on bottom
        private readonly bool flipBoard;
        //counts the moves that have been made without moving a pawn or eating a piece, else it restarts
        //if it reaches 50, it is a stalemate. 
        //Determines how many moves can be played without moving Pawns or capturing.
        private const int maxNonLinearMoves = 50;
        private int stalemateMoveCount = 0;
        //saves each turn's hash function, and counts how many of that turn has happened.
        //if any of the values reach 3, it is a stalemate.
        private Dictionary<char[,], int> boardStatesCount = new Dictionary<char[,], int>();
        public Chess()
        {
            //Board Initialization
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bool whitePiece = i < 5 ? true : false;
                    if (i > 1 && i < 6)
                        board[i, j] = new EmptyTile();
                    else if (i == 1 | i == 6)
                    {
                        board[i, j] = new Pawn(whitePiece);
                    }
                    else
                    {
                        if (j == 0 || j == 7)
                            board[i, j] = new Rook(whitePiece);
                        else if (j == 1 || j == 6)
                            board[i, j] = new Knight(whitePiece);
                        else if (j == 2 || j == 5)
                            board[i, j] = new Bishop(whitePiece);
                        else if (j == 3)
                            board[i, j] = new Queen(whitePiece);
                        else
                            board[i, j] = new King(whitePiece);
                    }
                }
            }
            //Initializing flags and gameStates
            gameState = GameState.Play;
            whiteTurn = true;
            flipBoard = true;
            stalemateMoveCount = 0;
        }
        public void Play()
        {
            while (gameState == GameState.Play)
            {
                Console.Clear();
                PrintGameGraphics(board, flipBoard);
                Console.WriteLine($"It is {(whiteTurn ? "white" : "black")}'s turn to play");
                MoveCheck.ChooseMove(board, flipBoard, whiteTurn, out Tile fromTile, out Tile toTile);
                MakeMove(fromTile, toTile);
                gameState = MoveCheck.IsGameCheckMate(board, toTile);
                if (gameState == GameState.Play)
                {
                    gameState = MoveCheck.IsGameStaleMate(board, whiteTurn, maxNonLinearMoves- stalemateMoveCount, boardStatesCount);
                }
            }
            Console.Clear();
            PrintGameGraphics(board, flipBoard);
            if (gameState == GameState.CheckMate)
            {
                Console.WriteLine($"{(whiteTurn ? "Black" : "White")} player wins!");
            }
            else
            {
                Console.WriteLine("Draw!");
            }
            Console.ReadLine();
        }
        private void MakeMove(Tile fromTile, Tile toTile)
        {
            //This method moves the game along-
            //Progresses the various flags and counts,
            //As well as amending the board          
            //If a piece has been eaten or the move was with a Pawn, the turns till stalemate get reset
            stalemateMoveCount = board[toTile.x, toTile.y] is EmptyTile ? stalemateMoveCount + 1 : 0;
            board = AmendBoard(board, fromTile, toTile);
            //EnPassant is wiped from all Pawns, then reassigned if a Pawn has moved two spaces
            foreach (var piece in board)
            {
                if (piece is Pawn enPassant)
                {
                    enPassant.EnPassant = false;
                }
            }
            //A Pawn which has reached the final row will be crowned
            if (board[toTile.x, toTile.y] is Pawn pawn)
            {
                stalemateMoveCount = 0;
                if (toTile.x == 0 || toTile.x == 7)
                {
                    board[toTile.x, toTile.y] = CrownPawn(whiteTurn);
                }
                else if (Math.Abs(toTile.x - fromTile.x) == 2)
                {
                    pawn.EnPassant = true;
                }
            }
            else if (board[toTile.x, toTile.y] is King king)
            {
                king.HasMoved = true;
            }
            else if (board[toTile.x, toTile.y] is Rook rook)
            {
                rook.HasMoved = true;
            }
            //If the same board state occurs three times in a game- it is a stalemate 
            boardStatesCount = AmendBoardStatesCount();
            whiteTurn = whiteTurn ? false : true;
        }
        public static Piece[,] AmendBoard(Piece[,] board, Tile fromTile, Tile toTile)
        {
            //This function amends the board in the desired manner,
            //Used either to check for Check or to advance the game
            //If the fromTile and toTile are identical, no change is needed
            if (fromTile.x != toTile.x || fromTile.y != toTile.y)
            {
                bool whiteTurn = board[fromTile.x, fromTile.y].whitePiece ? true : false;
                //Ensuring an enPassant Pawn will be removed-
                //If the moved piece is a Pawn && the enPassant is behind it's destination
                if (board[fromTile.x, fromTile.y] is Pawn)
                {
                    if (board[fromTile.x, toTile.y] is Pawn enPassant)
                    {
                        if (enPassant.EnPassant)
                        {
                            board[fromTile.x, toTile.y] = new EmptyTile();
                        }
                    }
                }
                //Ensuring the Rook will move in a castling
                if (board[fromTile.x, fromTile.y] is King)
                {
                    int yDistance = toTile.y - fromTile.y;
                    if (yDistance == 2)
                    {
                        board[toTile.x, 5] = new Rook(whiteTurn);
                        board[toTile.x, 7] = new EmptyTile();
                    }
                    if (yDistance == -2)
                    {
                        board[toTile.x, 3] = new Rook(whiteTurn);
                        board[toTile.x, 0] = new EmptyTile();
                    }
                }
                board[toTile.x, toTile.y] = board[fromTile.x, fromTile.y];
                board[fromTile.x, fromTile.y] = new EmptyTile();
            }
            return board;
        }
        private Piece CrownPawn(bool whiteTurn)
        {
            //A method to Crown a Pawn that has reached the end of it's column
            Piece chosenCrown = null;
            while (chosenCrown is null)
            {
                Console.Clear();
                PrintGameGraphics(board, flipBoard);
                Console.WriteLine("You can crown your Pawn! Please choose it's new designation" +
                Environment.NewLine + "Bishop | Knight | Queen | Rook");
                ConsoleKeyInfo input = Console.ReadKey(false);
                switch (input.Key)
                {
                    case ConsoleKey.B:
                        chosenCrown = new Bishop(whiteTurn);
                        break;
                    case ConsoleKey.K:
                        chosenCrown = new Knight(whiteTurn);
                        break;
                    case ConsoleKey.Q:
                        chosenCrown = new Queen(whiteTurn);
                        break;
                    case ConsoleKey.R:
                        chosenCrown = new Rook(whiteTurn);
                        break;
                }
            }
            return chosenCrown;
        }
        private Dictionary<char[,], int> AmendBoardStatesCount()
        {
            //A list of states is saved (as Char[,]) 
            //in order to make sure no state repeated itself 3 times or more
            char[,] thisBoardState = new char[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    thisBoardState[i, j] = board[i,j].whitePiece ? board[i, j].ToString().ToUpper()[0] : board[i,j].ToString()[0];
                }
            }
            //Try matching each existing board to this new one.
            //If found, add counter and stop searching
            //If none exist or no match found, add the current board
            bool existingBoardState = false;
            foreach (char[,] state in boardStatesCount.Keys)
            {
                existingBoardState = true;
                for (int i = 0; existingBoardState && i < 8; i++)
                {
                    for (int j = 0; existingBoardState && j < 8; j++)
                    {
                        existingBoardState = state[i, j] == thisBoardState[i, j] ? true : false;
                    }
                }
                if (existingBoardState)
                {
                    boardStatesCount[state]++;
                    break;
                }
            }
            if (!existingBoardState)
            {
                boardStatesCount.Add(thisBoardState, 1);
            }
            return boardStatesCount;
        }
        public static void PrintGameGraphics(Piece[,] board, bool flipBoard)
        {
            //Creating clear and defined borders, delimited by '#'
            Console.Title = "Chess";
            for (int j = 0; j < 9; j++)
            {
                Console.Write("#########");
            }
            Console.WriteLine();
            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        Console.Write("#");
                        string middle = "    ";
                        int relativeX = flipBoard ? 8 - i : i - 1;
                        if (i == 0 && j == 0) { }
                        else if (i > 0 && j > 0 && (i + j) % 2 == 0)
                        {
                            if (k != 1 || board[relativeX, j - 1] is EmptyTile)
                            {
                                middle = "////";
                            }
                            else
                            {
                                Piece piece = board[relativeX, j - 1];
                                middle = piece.whitePiece ? $" w{piece.ToString()[0]} " : $" b{piece.ToString()[0]} ";
                            }
                        }
                        else if (k == 1)
                        {
                            if (i == 0 && j > 0)
                            {
                                middle = ($" {(char)('A' - 1 + j)}  ");
                            }
                            else if (i > 0 && j == 0)
                            {
                                middle = ($" {relativeX+1}  ");
                            }
                            else if (!(board[relativeX, j - 1] is EmptyTile))
                            {
                                Piece piece = board[relativeX, j - 1];
                                middle = piece.whitePiece ? $" w{piece.ToString()[0]} " : $" b{piece.ToString()[0]} ";
                            }
                        }
                        if (k == 3)
                        {
                            Console.Write("########");
                        }
                        else
                        {
                            if ((i + j) % 2 == 0 && i > 0 && j > 0)
                            {
                                Console.Write($"//{middle}//");
                            }
                            else
                            {
                                Console.Write($"  {middle}  ");
                            }
                        }
                    }
                    Console.WriteLine("#");
                }
            }
        }
        #region AssistingMethods
        public static List<Tile> NewFindPlayerPieces(Piece[,] board, bool whitePlayer)
        {
            //This method finds all of a specified player's pieces and returns them in a list
            List<Tile> piecesList = new List<Tile>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].whitePiece == whitePlayer && !(board[i, j] is EmptyTile))
                    {
                        Tile newPiece = CreateTile(i, j);
                        piecesList.Add(newPiece);
                    }
                }
            }
            return piecesList;
        }
        public static Tile FindKing(Piece[,] board, bool whitePlayer)
        {
            //This method finds the specified player's King and returns it's tile.
            List<Tile> playerPieces = NewFindPlayerPieces(board, whitePlayer);
            Tile king = CreateTile(-1, -1);
            foreach (Tile piece in playerPieces)
            {
                if (board[piece.x, piece.y] is King)
                {
                    king = piece;
                }
            }
            return king;
        }
        public static Tile CreateTile(int x, int y)
        {
            Tile tile;
            tile.x = x;
            tile.y = y;
            return tile;
        }
        public static Piece[,] TestGameAdvance(Chess chess, List<Tile> fromTiles, List<Tile> toTiles, out int stalemateMoveCount, out Dictionary<char[,], int> boardStatesCount)
        {
            stalemateMoveCount = 0;
            boardStatesCount = new Dictionary<char[,], int>();
            for (int i = 0; i < fromTiles.Count; i++)
            {
                chess.MakeMove(fromTiles[i], toTiles[i]);
            }
            stalemateMoveCount = chess.stalemateMoveCount;
            boardStatesCount = chess.boardStatesCount;
            return chess.board;
        }
        #endregion
    }
}
