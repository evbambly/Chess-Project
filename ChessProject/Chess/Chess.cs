using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public struct CanCastle
    {
        public bool kingHasMoved;
        public bool rightRookHasMoved;
        public bool leftRookHasMoved;
    }
    public struct Tile
    {
        //For distinguishing moves etc.
        public int x;
        public int y;
    }
    public struct MoveStats
    {
        //A packaging struct to send vital data to methods
        public char[,] board;
        public CanCastle canPlayerCastle;
        public bool whiteTurn;
        public Tile enPassant;
    }

    enum GameState
    {
        Play, CheckMate, StaleMate
    }
    public class Chess
    {
        private char[,] board = new char[8, 8];
        private readonly static char emptyTile = '-';
        private bool whiteTurn;
        private GameState gameState;
        private CanCastle canWhiteCastle;
        private CanCastle canBlackCastle;
        private Tile enPassant;

        ///Switches between White on top or White on bottom
        ///counts the moves that have been made without moving a pawn or eating a piece, else it restarts
        ///if it reaches 50, it is a stalemate. 
        public static bool flipBoard = false;
        ///Determines how many moves can be played without moving Pawns or capturing.
        ///Default- 50
        private readonly static int maxNonLinearMoves = 50;
        private int stalemateMoveCount = 0;
        //saves each turn's hash function, and counts how many of that turn has happened.
        //if any of the values reach 3, it is a stalemate.
        private Dictionary<char[,], int> boardStatesCount = new Dictionary<char[,], int>();
        public Chess()
        {
            //Initializing board, white player represented by lower case (row 0,1) 
            //and black is represented by higher case (row 6,7)
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i > 1 && i < 6)
                        board[i, j] = EmptyTile;
                    else if (i == 1 || i == 6)
                    {
                        board[i, j] = 'p';
                    }
                    else
                    {
                        if (j == 0 || j == 7)
                            board[i, j] = 'r';
                        else if (j == 1 || j == 6)
                            board[i, j] = 'n';
                        else if (j == 2 || j == 5)
                            board[i, j] = 'b';
                        else if (j == 3)
                            board[i, j] = 'q';
                        else
                            board[i, j] = 'k';

                    }
                    if (i > 5)
                        board[i, j] = (char)(board[i, j] - 32);
                }
            }
            //Initializing flags and gameStates
            gameState = GameState.Play;
            whiteTurn = true;
            canWhiteCastle.kingHasMoved = false;
            canWhiteCastle.leftRookHasMoved = false;
            canWhiteCastle.rightRookHasMoved = false;
            canBlackCastle.kingHasMoved = false;
            canBlackCastle.leftRookHasMoved = false;
            canBlackCastle.rightRookHasMoved = false;
            stalemateMoveCount = 0;
        }
        public void Play()
        {
            while (gameState != GameState.StaleMate && gameState != GameState.CheckMate)
            {
                Console.Clear();
                FancyPrintGame(board);
                Console.WriteLine($"It is {(whiteTurn ? "white" : "black")}'s turn to play");
                MoveCheck.ChooseLegalMove(CreateMoveStats(), out Tile fromTile, out Tile toTile);
                MakeMove(fromTile, toTile);
                if (MoveCheck.IsGameStaleMate(CreateMoveStats(), StalemateMoveCount, BoardStatesCount))
                {
                    gameState = GameState.StaleMate;
                }
                if (gameState == GameState.Play && MoveCheck.IsGameCheckMate(CreateMoveStats(), toTile))
                {
                    gameState = GameState.CheckMate;
                }
            }
            while (true)
            {
                Console.Clear();
                FancyPrintGame(board);
                if (gameState == GameState.CheckMate)
                {
                    Console.WriteLine($"{(whiteTurn ? "Black" : "White")} player wins!");
                }
                else
                {
                    Console.WriteLine("Draw!");
                }
            }

        }
        public static void PrintGame(char[,] board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(board[i, j]);
                }
                Console.WriteLine();
            }
        }
        public static void OldPrintGame(char[,] board)
        {
            //Creating clear and defined borders, delimited by '#'
            Console.Title = "Chess";
            char[,] displayBoard = new char[9, 9];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    displayBoard[i + 1, j + 1] = board[i, j];
                }
            }
            for (int i = 1; i < 9; i++)
            {
                displayBoard[0, i] = (char)(64 + i);
                displayBoard[i, 0] = (char)(48 + i);
            }
            if (flipBoard)
            {
                char[,] flippedBoard = new char[9, 9];
                for (int i = 0; i < 9; i++)
                {
                    flippedBoard[0, i] = displayBoard[0, i];
                }
                for (int i = 1; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        flippedBoard[i, j] = displayBoard[9 - i, j];
                    }
                }
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        displayBoard[i, j] = flippedBoard[i, j];
                    }
                }
            }
            for (int j = 0; j < 9; j++)
            {
                Console.Write("#########");
            }
            Console.WriteLine();
            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Console.Write('#');
                    for (int j = 0; j < 9; j++)
                    {
                        if (k == 0 || k == 2)
                        {
                            Console.Write("        #");
                        }
                        else if (k == 1)
                        {

                            string middle = "";
                            if (i == 0 || j == 0)
                                middle = $" {displayBoard[i, j]}  ";
                            else if (displayBoard[i, j] > 96)
                                middle = $" w{displayBoard[i, j]} ";
                            else if (displayBoard[i, j] > 64)
                                middle = $" b{displayBoard[i, j].ToString().ToLower()} ";
                            else
                                middle = $"    ";
                            Console.Write($"  {middle}  #");
                        }
                        else
                        {
                            Console.Write("#########");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        public static void FancyPrintGame(char[,] board)
        {
            //Creating clear and defined borders, delimited by '#'
            Console.Title = "Chess";
            char[,] displayBoard = new char[9, 9];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    displayBoard[i + 1, j + 1] = board[i, j];
                }
            }
            for (int i = 1; i < 9; i++)
            {
                displayBoard[0, i] = (char)(64 + i);
                displayBoard[i, 0] = (char)(48 + i);
            }
            if (flipBoard)
            {
                char[,] flippedBoard = new char[9, 9];
                for (int i = 0; i < 9; i++)
                {
                    flippedBoard[0, i] = displayBoard[0, i];
                }
                for (int i = 1; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        flippedBoard[i, j] = displayBoard[9 - i, j];
                    }
                }
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        displayBoard[i, j] = flippedBoard[i, j];
                    }
                }
            }
            for (int j = 0; j < 9; j++)
            {
                Console.Write("#########");
            }
            Console.WriteLine();
            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Console.Write('#');
                    for (int j = 0; j < 9; j++)
                    {
                        if (k == 0 || k == 2)
                        {
                            if ((i + j) % 2 == 1 && i > 0 && j > 0)
                            {
                                Console.Write("////////#");
                            }
                            else
                            {
                                Console.Write("        #");
                            }
                        }
                        else if (k == 1)
                        {

                            string middle = "";
                            if (i == 0 || j == 0)
                                middle = $" {displayBoard[i, j]}  ";
                            else if (displayBoard[i, j] > 96)
                                middle = $" w{displayBoard[i, j]} ";
                            else if (displayBoard[i, j] > 64)
                                middle = $" b{displayBoard[i, j].ToString().ToLower()} ";
                            else
                            {
                                if ((i + j) % 2 == 1 && i > 0 && j > 0)
                                {
                                    middle = "////";
                                }
                                else
                                {
                                    middle = $"    ";
                                }
                                }
                            if ((i + j) % 2 == 1 && i>0 && j> 0)
                            {
                                Console.Write($"//{middle}//#");
                            }
                            else
                            {
                                Console.Write($"  {middle}  #");
                            }
                            }
                        else
                        {
                            Console.Write("#########");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        public void MakeMove(Tile fromTile, Tile toTile)
        {
            //This method moves the game along-
            //Progresses the various flags and counts,
            //As well as amending the board
            CanCastle canPlayerCastle = whiteTurn ? canWhiteCastle : canBlackCastle;
            int lastRow = whiteTurn ? 7 : 0;
            int firstRow = whiteTurn ? 0 : 7;
            enPassant.x = -1;
            enPassant.y = -1;
            bool noPiecesEaten = board[toTile.x, toTile.y] == EmptyTile ? true : false;
            board = AmendBoard(board, fromTile, toTile);
            //If a piece has been eaten or the move was with a Pawn, the turns till stalemate get reset
            //A Pawn which has reached the final row will be crowned
            //enPassant will be assigned a Pawn which has moved two spaces, Else it will not be assigned at all
            if (board[toTile.x, toTile.y] == Pawn(whiteTurn))
            {
                stalemateMoveCount = 0;
                if (toTile.x == lastRow)
                {
                    board[toTile.x, toTile.y] = CrownPawn();
                    if (!whiteTurn)
                    {
                        board[toTile.x, toTile.y] = (char)(board[toTile.x, toTile.y] - 32);
                    }
                }
                if (toTile.x - fromTile.x == -2 || toTile.x - fromTile.x == 2)
                {
                    enPassant.x = toTile.x;
                    enPassant.y = toTile.y;
                }
            }
            else
            {
                if (noPiecesEaten)
                {
                    stalemateMoveCount++;
                }
                else
                {
                    stalemateMoveCount = 0;
                }
                if (board[toTile.x, toTile.y] == King(whiteTurn))
                {
                    canPlayerCastle.kingHasMoved = true;
                }
                if (board[toTile.x, toTile.y] == Rook(whiteTurn))
                {
                    if (fromTile.x == firstRow)
                    {
                        if (fromTile.y == 0)
                        {
                            canPlayerCastle.leftRookHasMoved = true;
                        }
                        if (fromTile.y == 7)
                        {
                            canPlayerCastle.rightRookHasMoved = true;
                        }
                    }
                }
            }
            //If the same board state (thus hash) occurs three times in a game- it is a stalemate 
            boardStatesCount = AmendBoardStatesCount();

            if (whiteTurn)
            {
                canWhiteCastle = canPlayerCastle;
                whiteTurn = false;
            }
            else
            {
                canBlackCastle = canPlayerCastle;
                whiteTurn = true;
            }
        }
        public static char[,] AmendBoard(char[,] board, Tile fromTile, Tile toTile)
        {
            //This function amends the board in the desired manner,
            //Used either to check for Check or to advance the game
            bool whiteTurn = false;
            if (board[fromTile.x, fromTile.y] > 96)
            {
                whiteTurn = true;
            }
            //Ensuring an enPassant Pawn will be removed-
            //If the moved piece is a Pawn && it's moving diagonally && it's destination is empty
            int advance = whiteTurn ? 1 : -1;
            if (board[fromTile.x, fromTile.y] == Pawn(whiteTurn))
            {
                if (toTile.y - fromTile.y == -1 || toTile.y - fromTile.y == 1)
                {
                    if (board[toTile.x, toTile.y] == EmptyTile)
                    {
                        board[toTile.x - advance, toTile.y] = EmptyTile;
                    }
                }
            }
            //Ensuring the Rook will move in a castling
            if (board[fromTile.x, fromTile.y] == King(whiteTurn))
            {
                int yDistance = toTile.y - fromTile.y;
                if (yDistance == 2)
                {
                    board[toTile.x, 5] = Rook(whiteTurn);
                    board[toTile.x, 7] = EmptyTile;
                }
                if (yDistance == -2)
                {
                    board[toTile.x, 3] = Rook(whiteTurn);
                    board[toTile.x, 0] = EmptyTile;
                }
            }
            board[toTile.x, toTile.y] = board[fromTile.x, fromTile.y];
            board[fromTile.x, fromTile.y] = EmptyTile;
            return board;
        }
        private char CrownPawn()
        {
            //A method to Crown a Pawn that has reached the end of it's column
            bool crownChosen = false;
            char chosenCrown = ' ';
            int menuOption = 0;
            string[] menu = { "Bishop", "Knight", "Queen", "Rook" };
            while (!crownChosen)
            {
                Console.Clear();
                FancyPrintGame(board);
                Console.WriteLine("You can crown your Pawn! Please choose it's new designation");
                for (int i = 0; i < 4; i++)
                {
                    if (menuOption == i)
                    {
                        Console.Write("-->");
                    }
                    Console.WriteLine(menu[i]);
                }
                ConsoleKeyInfo input = Console.ReadKey();
                switch (input.Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        if (menuOption > 0)
                        {
                            menuOption--;
                        }
                        else
                        {
                            menuOption = 3;
                        }
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (menuOption < 3)
                        {
                            menuOption++;
                        }
                        else
                        {
                            menuOption = 0;
                        }
                        break;
                    case ConsoleKey.E:
                    case ConsoleKey.Enter:
                        crownChosen = true;
                        break;
                }
            }
            switch (menuOption)
            {
                case 0:
                    chosenCrown = 'b';
                    break;
                case 1:
                    chosenCrown = 'n';
                    break;
                case 2:
                    chosenCrown = 'q';
                    break;
                case 3:
                    chosenCrown = 'r';
                    break;
            }
            return chosenCrown;
        }
        private Dictionary<char[,], int> AmendBoardStatesCount()
        {
            //A list of states is saved (as char[,]) 
            //in order to make sure no state repeated itself 3 times or more
            char[,] thisBoardState = new char[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    thisBoardState[i, j] = board[i, j];
                }
            }
            //Try matching each existing board to this new one.
            //If found, add counter and stop searching
            //If none exist or no match found, add the current board
            bool existingBoardState = false;
            foreach (char[,] state in boardStatesCount.Keys)
            {
                existingBoardState = true;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (state[i, j] != thisBoardState[i, j])
                        {
                            existingBoardState = false;
                        }
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
        #region AssistingMethods
        public static bool SameTile(Tile a, Tile b)
        {
            return a.x == b.x && a.y == b.y;
        }
        private MoveStats CreateMoveStats()
        {
            //A method to package and send the important stats of the current move to any external class
            MoveStats moveStats;
            moveStats.board = new char[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    moveStats.board[i, j] = board[i, j];
                }
            }

            moveStats.canPlayerCastle = whiteTurn ? canWhiteCastle : canBlackCastle;
            moveStats.whiteTurn = whiteTurn;
            moveStats.enPassant = enPassant;
            return moveStats;
        }
        #region BoardPieces
        public static char EmptyTile
        {
            get => emptyTile;
        }
        public static char Bishop(bool whiteTurn)
        {
            return whiteTurn ? 'b' : 'B';
        }
        public static char King(bool whiteTurn)
        {
            return whiteTurn ? 'k' : 'K';

        }
        public static char Knight(bool whiteTurn)
        {
            return whiteTurn ? 'n' : 'N';
        }
        public static char Pawn(bool whiteTurn)
        {
            return whiteTurn ? 'p' : 'P';
        }
        public static char Queen(bool whiteTurn)
        {
            return whiteTurn ? 'q' : 'Q';
        }
        public static char Rook(bool whiteTurn)
        {
            return whiteTurn ? 'r' : 'R';
        }
        #endregion
        public int StalemateMoveCount
        {
            get => stalemateMoveCount;
        }
        public Dictionary<char[,], int> BoardStatesCount
        {
            get => boardStatesCount;
        }
        public static int MaxNonLinearMoves
            {
            get => maxNonLinearMoves;
            }
        public static List<Tile> FindPlayerPieces(char[,] board, bool whitePlayer)
        {
            //This method finds all of a specified player's pieces and returns them in a list
            //Putting {whiteTurn} will return the Player's pieces, 
            //putting {!whiteTurn} will return the foe's pieces
            List<Tile> piecesList = new List<Tile>();
            int playerBias = whitePlayer ? 97 : 65;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] - playerBias > -1 && board[i, j] - playerBias < 26)
                    {
                        Tile newPiece;
                        newPiece.x = i;
                        newPiece.y = j;
                        piecesList.Add(newPiece);
                    }
                }
            }
            return piecesList;
        }
        public static Tile FindKing(char[,] board, bool whitePlayer)
        {
            //This method finds the specified player's King and returns it's tile.
            Tile king;
            king.x = -1;
            king.y = -1;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == King(whitePlayer))
                    {
                        king.x = i;
                        king.y = j;
                    }
                }
            }
            return king;
        }
        #endregion
    }
}
