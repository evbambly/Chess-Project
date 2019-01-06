using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public enum StaleMateTest
    {
        maxMoves, maxBoardStates, NotEnoughMaterial, NoLegalMove
    }
    class Test
    {
        //A testing class, designed to input data and see if it works
        //Always set new flipBoard to False before testing
        //BoardVisualisation should be used if not clear of the board state
        private static int testCounter = 1;
        private static Piece[,] board = CreateBoard();
        private static bool whiteTurn;
        private static List<Tile> fromTiles = new List<Tile>();
        private static List<Tile> toTiles = new List<Tile>();
        public static void TestMoveCheckBishop()
        {
            whiteTurn = true;

            board[4, 4] = new Bishop(true);
            board[6, 2] = new Pawn(false);
            board[3, 5] = new Queen(true);

            ExecuteMoveCheckTest(board, CreateTile(4, 4), CreateTile(5, 6), false);
            ExecuteMoveCheckTest(board, CreateTile(4, 4), CreateTile(3, 3), true);
            ExecuteMoveCheckTest(board, CreateTile(4, 4), CreateTile(7, 1), false);
            ExecuteMoveCheckTest(board, CreateTile(4, 4), CreateTile(2, 6), false);
            //Tile amended
            whiteTurn = false;
            board[4, 3] = new Bishop(false);
            //Just to make sure the King doesn't screw the Check check
            board[0, 1] = new King(false);
            ExecuteMoveCheckTest(board, CreateTile(4, 3), CreateTile(6, 5), true);
            ExecuteMoveCheckTest(board, CreateTile(4, 3), CreateTile(1, 0), true);
            ExecuteMoveCheckTest(board, CreateTile(4, 3), CreateTile(1, 6), true);
            ExecuteMoveCheckTest(board, CreateTile(4, 3), CreateTile(5, 2), true);
        }
        public static void TestMoveCheckKing(bool testCastlingRules)
        {
            if (!testCastlingRules)
            {
                whiteTurn = true;
                board[2, 5] = new King(true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(1, 6), true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(1, 5), true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(1, 4), true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(2, 4), true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(3, 4), true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(3, 5), true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(3, 6), true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(2, 6), true);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(1, 7), false);
                ExecuteMoveCheckTest(board, CreateTile(2, 5), CreateTile(0, 5), false);
            }
            if (testCastlingRules)
            {
                whiteTurn = true;
                board[0, 4] = new King(true);
                board[0, 0] = new Rook(true);
                board[0, 7] = new Rook(true);
                board[7, 4] = new King(false);
                board[7, 0] = new Rook(false);
                board[7, 7] = new Rook(false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), true);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 6), true);
                whiteTurn = false;
                ExecuteMoveCheckTest(board, CreateTile(7, 4), CreateTile(7, 2), true);
                ExecuteMoveCheckTest(board, CreateTile(7, 4), CreateTile(7, 6), true);
                whiteTurn = true;
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(1, 2), false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(7, 6), false);
                //Tile amended
                board = HasMoved(board, CreateTile(0, 7));
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 6), false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), true);
                //Tile amended
                board[0, 0] = new EmptyTile();
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), false);
                //Condition change
                whiteTurn = false;
                ExecuteMoveCheckTest(board, CreateTile(7, 4), CreateTile(7, 2), true);
                //Tile amended
                board = HasMoved(board, CreateTile(7, 4));
                ExecuteMoveCheckTest(board, CreateTile(7, 4), CreateTile(7, 2), false);
                //Condition reset                
                whiteTurn = true;
                board[0, 4] = new King(true);
                board[0, 0] = new Rook(true);
                board[0, 7] = new Rook(true);
                board[7, 4] = new King(false);
                board[7, 0] = new Rook(false);
                board[7, 7] = new Rook(false);
                whiteTurn = false;
                board[7, 5] = new Bishop(false);
                ExecuteMoveCheckTest(board, CreateTile(7, 4), CreateTile(7, 6), false);
                //Tile amended
                board[7, 5] = new EmptyTile();
                board[7, 6] = new Bishop(true);
                ExecuteMoveCheckTest(board, CreateTile(7, 4), CreateTile(7, 6), false);
                ExecuteMoveCheckTest(board, CreateTile(7, 4), CreateTile(7, 2), true);
                //Tile amended
                board[7, 1] = new Bishop(true);
                ExecuteMoveCheckTest(board, CreateTile(7, 4), CreateTile(7, 2), false);
                //Condition change
                whiteTurn = true;
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), true);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 6), true);
                //Tile amended
                board[2, 2] = new Bishop(false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), false);
                //Tile amended
                //Rook can be in Check
                board[2, 2] = new EmptyTile();
                board[2, 3] = new Bishop(false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), true);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 6), false);
                //Tile amended
                board[3, 6] = new Bishop(false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), false);
                //Tile amended
                board[2, 5] = new Rook(false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), true);
                //Tile amended
                board[2, 4] = new Bishop(false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 2), false);
                board[2, 3] = new EmptyTile();
                board[3,6] = new EmptyTile();
                board[2, 5] = new EmptyTile();
                board[2, 4] = new EmptyTile();
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 1), false);
                ExecuteMoveCheckTest(board, CreateTile(0, 4), CreateTile(0, 7), false);
            }
        }
        public static void TestMoveCheckKnight()
        {
            whiteTurn = true;
            board[3, 3] = new Knight(true);
            board[1, 1] = new Queen(false);
            board[1, 3] = new Queen(false);
            board[1, 5] = new Queen(false);
            board[2, 2] = new Queen(true);
            board[2, 3] = new Queen(true);
            board[2, 4] = new Queen(false);
            board[3, 1] = new Queen(true);
            //Just to make sure the King doesn't screw the Check check
            board[6, 6] = new Pawn(true);
            board[7, 7] = new King(true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(4, 4), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(5, 5), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(2, 2), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(1, 1), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(1, 2), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(1, 4), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(2, 5), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(4, 5), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(5, 2), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(5, 4), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(2, 1), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(4, 1), true);
        }
        public static void TestMoveCheckPawn()
        {
            whiteTurn = true;
            board[1, 4] = new Pawn(true);
            board[6, 4] = new Pawn(false);
            board[7, 0] = new King(false);
            ExecuteMoveCheckTest(board, CreateTile(1, 4), CreateTile(2, 4), true);
            whiteTurn = false;
            ExecuteMoveCheckTest(board, CreateTile(6, 4), CreateTile(5, 4), true);
            //Tile amended
            board[5, 4] = new Queen(true);
            ExecuteMoveCheckTest(board, CreateTile(6, 4), CreateTile(5, 4), false);
            //Tile amended
            board[5, 4] = new EmptyTile();
            ExecuteMoveCheckTest(board, CreateTile(6, 4), CreateTile(4, 4), true);
            whiteTurn = true;
            ExecuteMoveCheckTest(board, CreateTile(1, 4), CreateTile(3, 4), true);
            //Tile amended
            board[2, 0] = new Pawn(true);
            ExecuteMoveCheckTest(board, CreateTile(2, 0), CreateTile(3, 0), true);
            ExecuteMoveCheckTest(board, CreateTile(2, 0), CreateTile(4, 0), false);
            //Tile amended
            board[3, 4] = new Rook(true);
            board[5, 4] = new Knight(false);
            ExecuteMoveCheckTest(board, CreateTile(1, 4), CreateTile(3, 4), false);
            whiteTurn = false;
            ExecuteMoveCheckTest(board, CreateTile(6, 4), CreateTile(4, 4), false);
            ExecuteMoveCheckTest(board, CreateTile(6, 4), CreateTile(5, 3), false);
            ExecuteMoveCheckTest(board, CreateTile(6, 4), CreateTile(5, 5), false);
            //Tile amended
            board[5, 3] = new Knight(true);
            board[3, 1] = new Knight(false);
            ExecuteMoveCheckTest(board, CreateTile(6, 4), CreateTile(5, 3), true);
            whiteTurn = true;
            ExecuteMoveCheckTest(board, CreateTile(2, 0), CreateTile(3, 1), true);
            //Tile amended
            board[4, 1] = new Knight(false);
            ExecuteMoveCheckTest(board, CreateTile(2, 0), CreateTile(4, 1), false);
            //Tile amended
            board[3, 1] = new EmptyTile();
            ExecuteMoveCheckTest(board, CreateTile(2, 0), CreateTile(4, 1), false);
            //Tile amended
            board[3, 1] = new Pawn(true);
            board[3, 2] = new Pawn(false);
            whiteTurn = false;
            ExecuteMoveCheckTest(board, CreateTile(3, 2), CreateTile(2, 1), false);
            //Condition change
            board[3, 1] = new Pawn(true) { EnPassant = true };
            ExecuteMoveCheckTest(board, CreateTile(3, 2), CreateTile(2, 1), true);
            //Tile amended 
            board[3, 3] = new Pawn(false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(2, 2), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(2, 1), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 2), CreateTile(2, 3), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 2), CreateTile(4, 2), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(4, 3), false);
            whiteTurn = true;
            ExecuteMoveCheckTest(board, CreateTile(3, 1), CreateTile(2, 1), false);
            ExecuteMoveCheckTest(board, CreateTile(2, 0), CreateTile(1, 1), false);
        }
        public static void TestMoveCheckQueen()
        {
            whiteTurn = true;

            board[4, 4] = new Queen(true);
            board[6, 2] = new Pawn(false);
            board[3, 5] = new King(true);

            ExecuteMoveCheckTest(board, CreateTile(4, 4), CreateTile(5, 6), false);
            ExecuteMoveCheckTest(board, CreateTile(4, 4), CreateTile(3, 3), true);
            ExecuteMoveCheckTest(board, CreateTile(4, 4), CreateTile(7, 1), false);
            ExecuteMoveCheckTest(board, CreateTile(4, 4), CreateTile(2, 6), false);
            //Tile amended
            whiteTurn = false;
            //Just to make sure the King doesn't screw the Check check
            board[2, 0] = new King(false);
            board[4, 3] = new Queen(false);
            ExecuteMoveCheckTest(board, CreateTile(4, 3), CreateTile(6, 5), true);
            ExecuteMoveCheckTest(board, CreateTile(4, 3), CreateTile(1, 0), true);
            ExecuteMoveCheckTest(board, CreateTile(4, 3), CreateTile(1, 6), true);
            ExecuteMoveCheckTest(board, CreateTile(4, 3), CreateTile(5, 2), true);

            board[3, 3] = new Queen(false);
            board[3, 1] = new Pawn(true);

            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(3, 2), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(3, 0), false);
            //Tile amended
            board[4, 3] = new EmptyTile();
            board[6, 3] = new King(false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(7, 3), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(5, 3), true);
            //Tile amended
            board[4, 3] = new EmptyTile();
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(5, 3), true);
            //Tile amended
            whiteTurn = true;
            board[6, 2] = new Queen(false);
            board[6, 3] = new EmptyTile();
            ExecuteMoveCheckTest(board, CreateTile(6, 2), CreateTile(6, 7), true);
            ExecuteMoveCheckTest(board, CreateTile(6, 2), CreateTile(2, 2), true);
        }
        public static void TestMoveCheckRook()
        {
            whiteTurn = false;
            board[3, 3] = new Rook(false);
            board[3, 1] = new Pawn(true);

            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(3, 2), true);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(3, 0), false);
            //Tile amended
            board[6, 3] = new King(false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(7, 3), false);
            ExecuteMoveCheckTest(board, CreateTile(3, 3), CreateTile(5, 3), true);
            //Tile amended
            whiteTurn = true;
            board[6, 2] = new Rook(true);
            board[6, 3] = new EmptyTile();
            ExecuteMoveCheckTest(board, CreateTile(6, 2), CreateTile(6, 7), true);
            ExecuteMoveCheckTest(board, CreateTile(6, 2), CreateTile(2, 2), true);
        }
        public static void TestStaleMate(StaleMateTest currentTest)
        {
            if (currentTest == StaleMateTest.maxMoves)
            {
                //Testing max moves, decrease the maxNonLinearMoves to 5
                bool serialMoves = true;
                AddMoveToList(CreateTile(1, 4), CreateTile(3, 4));
                AddMoveToList(CreateTile(6, 4), CreateTile(4, 4));
                AddMoveToList(CreateTile(0, 1), CreateTile(2, 0));
                AddMoveToList(CreateTile(7, 6), CreateTile(5, 5));
                AddMoveToList(CreateTile(2, 0), CreateTile(3, 2));
                AddMoveToList(CreateTile(5, 5), CreateTile(4, 3));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                AddMoveToList(CreateTile(3, 2), CreateTile(5, 1));
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                AddMoveToList(CreateTile(1, 1), CreateTile(3, 1));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                AddMoveToList(CreateTile(3, 2), CreateTile(4, 4));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                AddMoveToList(CreateTile(3, 2), CreateTile(5, 3));
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
                Console.WriteLine("Return MaxNonLinearMoves to 50!!!");
            }
            if (currentTest == StaleMateTest.maxBoardStates)
            {
                bool serialMoves = true;
                AddMoveToList(CreateTile(1, 4), CreateTile(3, 4));
                AddMoveToList(CreateTile(6, 4), CreateTile(4, 4));
                AddMoveToList(CreateTile(3, 4), CreateTile(1, 4));
                AddMoveToList(CreateTile(4, 4), CreateTile(6, 4));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                AddMoveToList(CreateTile(1, 4), CreateTile(3, 4));
                AddMoveToList(CreateTile(6, 4), CreateTile(4, 4));
                AddMoveToList(CreateTile(3, 4), CreateTile(1, 4));
                AddMoveToList(CreateTile(4, 4), CreateTile(6, 4));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                AddMoveToList(CreateTile(1, 4), CreateTile(3, 4));
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
            }
            if (currentTest == StaleMateTest.NotEnoughMaterial)
            {
                bool serialMoves = false;
                AddMoveToList(CreateTile(0, 4), CreateTile(0, 4));
                AddMoveToList(CreateTile(7, 4), CreateTile(7, 4));
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
                AddMoveToList(CreateTile(0, 2), CreateTile(0, 2));
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                AddMoveToList(CreateTile(0, 6), CreateTile(0, 6));
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
                AddMoveToList(CreateTile(7, 1), CreateTile(7, 1));
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                AddMoveToList(CreateTile(0, 1), CreateTile(0, 1));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                RemoveMove(2);
                AddMoveToList(CreateTile(6, 5), CreateTile(3, 5));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                AddMoveToList(CreateTile(7, 0), CreateTile(7, 0));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                AddMoveToList(CreateTile(0, 3), CreateTile(0, 3));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
            }
            if (currentTest == StaleMateTest.NoLegalMove)
            {
                bool serialMoves = false;
                AddMoveToList(CreateTile(0, 4), CreateTile(0, 7));
                AddMoveToList(CreateTile(7, 4), CreateTile(7, 7));
                AddMoveToList(CreateTile(0, 3), CreateTile(0, 6));
                AddMoveToList(CreateTile(6, 5), CreateTile(4, 5));
                AddMoveToList(CreateTile(1, 5), CreateTile(3, 5));
                AddMoveToList(CreateTile(6, 5), CreateTile(6, 5));
                AddMoveToList(CreateTile(0, 6), CreateTile(5, 5));
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
                AddMoveToList(CreateTile(6, 4), CreateTile(6, 4));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                AddMoveToList(CreateTile(1, 1), CreateTile(1, 1));
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                RemoveMove(2);
                ExecuteStaleMateTest(GameState.StaleMate, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
                RemoveMove(1);
                ExecuteStaleMateTest(GameState.Play, fromTiles, toTiles, serialMoves);
            }
        }
        public static void TestCheckMate()
        {
            AddMoveToList(CreateTile(0, 2), CreateTile(4, 5));
            AddMoveToList(CreateTile(1, 4), CreateTile(6, 5));
            AddMoveToList(CreateTile(1, 4), CreateTile(5, 5));
            AddMoveToList(CreateTile(1, 4), CreateTile(6, 6));
            AddMoveToList(CreateTile(7, 4), CreateTile(7, 7));
            ExecuteCheckMateTest(GameState.CheckMate, fromTiles, toTiles);
            RemoveMove(3);
            AddMoveToList(CreateTile(7, 4), CreateTile(7, 7));
            ExecuteCheckMateTest(GameState.Play, fromTiles, toTiles);
            RemoveMove(1);
            AddMoveToList(CreateTile(0, 2), CreateTile(4, 4));
            AddMoveToList(CreateTile(7, 4), CreateTile(7, 7));
            ExecuteCheckMateTest(GameState.CheckMate, fromTiles, toTiles);
            RemoveMove(2);
            AddMoveToList(CreateTile(0, 6), CreateTile(6, 5));
            AddMoveToList(CreateTile(6, 7), CreateTile(6, 6));
            AddMoveToList(CreateTile(1, 6), CreateTile(6, 7));
            AddMoveToList(CreateTile(7, 4), CreateTile(7, 7));
            ExecuteCheckMateTest(GameState.CheckMate, fromTiles, toTiles);
            RemoveMove(2);
            AddMoveToList(CreateTile(0, 3), CreateTile(6, 7));
            AddMoveToList(CreateTile(7, 4), CreateTile(7, 7));
            ExecuteCheckMateTest(GameState.CheckMate, fromTiles, toTiles);
            RemoveMove(fromTiles.Count);
            AddMoveToList(CreateTile(0, 0), CreateTile(6, 0));
            AddMoveToList(CreateTile(0, 7), CreateTile(7, 7));
            AddMoveToList(CreateTile(7, 4), CreateTile(7, 4));
            ExecuteCheckMateTest(GameState.CheckMate, fromTiles, toTiles);
            RemoveMove(3);
            AddMoveToList(CreateTile(7, 3), CreateTile(1, 3));
            AddMoveToList(CreateTile(7, 6), CreateTile(2, 2));
            AddMoveToList(CreateTile(0, 3), CreateTile(1, 1));
            AddMoveToList(CreateTile(7, 3), CreateTile(1, 0));
            AddMoveToList(CreateTile(0, 4), CreateTile(0, 1));
            ExecuteCheckMateTest(GameState.CheckMate, fromTiles, toTiles);
            RemoveMove(2);
            AddMoveToList(CreateTile(7, 3), CreateTile(2, 0));
            AddMoveToList(CreateTile(0, 4), CreateTile(0, 1));
            ExecuteCheckMateTest(GameState.Play, fromTiles, toTiles);
            RemoveMove(fromTiles.Count);
            AddMoveToList(CreateTile(0, 0), CreateTile(6, 0));
            AddMoveToList(CreateTile(0, 7), CreateTile(7, 7));
            AddMoveToList(CreateTile(7, 6), CreateTile(5, 7));
            AddMoveToList(CreateTile(7, 4), CreateTile(7, 4));
            ExecuteCheckMateTest(GameState.Play, fromTiles, toTiles);
            RemoveMove(4);
            AddMoveToList(CreateTile(1, 6), CreateTile(1, 6));
            AddMoveToList(CreateTile(1, 5), CreateTile(1, 5));
            AddMoveToList(CreateTile(7, 7), CreateTile(0, 2));
            AddMoveToList(CreateTile(0, 4), CreateTile(0, 6));
            ExecuteCheckMateTest(GameState.Play, fromTiles, toTiles);
            AddMoveToList(CreateTile(1, 7), CreateTile(1, 7));
            ExecuteCheckMateTest(GameState.CheckMate, fromTiles, toTiles);
        }
        public static void TestChessCrownPawn()
        {
            AddMoveToList(CreateTile(1, 1), CreateTile(7, 1));
            Chess.TestGameAdvance(new Chess(), fromTiles, toTiles, out int a, out Dictionary<char[,], int> b);
        }
        private static void ExecuteMoveCheckTest(Piece[,] board, Tile fromTile, Tile toTile, bool desiredTestResult)
        {
            bool whiteTurn = board[fromTile.x, fromTile.y].whitePiece;
            bool testResult = MoveCheck.IsMovePossible(board, whiteTurn, fromTile, toTile);
            string success = desiredTestResult == testResult ? "Success!" : "***";
            Console.WriteLine($"Test {testCounter} expected {desiredTestResult}, is {testResult}. {success}");

            testCounter++;
        }
        private static void ExecuteStaleMateTest(GameState desiredTestResult, List<Tile> fromTiles, List<Tile> toTiles, bool serialMoves)
        {
            Chess chess = new Chess();
            int stalemateMoveCount;
            Dictionary<char[,], int> boardStateCount;
            if (serialMoves)
            {
                //This option performs the moves one after another
                board = Chess.TestGameAdvance(chess, fromTiles, toTiles, out stalemateMoveCount, out boardStateCount);
            }
            else
            {
                //This option copies pieces from a set board to a clear board-
                //i.e. only sets the necessary pieces for the test, the rest is empty
                Piece[,] sampleBoard = Chess.TestGameAdvance(chess, new List<Tile>(), new List<Tile>(), out stalemateMoveCount, out boardStateCount);
                board = CreateBoard();
                for (int i = 0; i < fromTiles.Count; i++)
                {
                    board[toTiles[i].x, toTiles[i].y] = sampleBoard[fromTiles[i].x, fromTiles[i].y];
                }
                stalemateMoveCount = 0;
                boardStateCount = new Dictionary<char[,], int>();
            }
            whiteTurn = fromTiles.Count % 2 == 0 ? true : false;
            //MovesToStalemate is sent as maxMoves - moveCount
            GameState testResult = MoveCheck.IsGameStaleMate(board, whiteTurn,5- stalemateMoveCount, boardStateCount);
            string success = desiredTestResult == testResult ? "Success!" : "***";
            Console.WriteLine($"Test {testCounter} expected {desiredTestResult}, is {testResult}. {success}");

            testCounter++;
        }
        private static void ExecuteCheckMateTest(GameState desiredTestResult, List<Tile> fromTiles, List<Tile> toTiles)
        {
            Chess chess = new Chess();
            Piece[,] sampleBoard = Chess.TestGameAdvance(chess, new List<Tile>(), new List<Tile>(), out int stalemateMoveCount, out Dictionary<char[,], int>  boardStateCount);
            board = CreateBoard();
            for (int i = 0; i < fromTiles.Count; i++)
            {
                board[toTiles[i].x, toTiles[i].y] = sampleBoard[fromTiles[i].x, fromTiles[i].y];
            }
            stalemateMoveCount = 0;
            boardStateCount = new Dictionary<char[,], int>();
            //Last tile must be the victim king
            GameState testResult = MoveCheck.IsGameCheckMate(board, toTiles[toTiles.Count-1]);
            string success = desiredTestResult == testResult ? "Success!" : "***";
            Console.WriteLine($"Test {testCounter} expected {desiredTestResult}, is {testResult}. {success}");

            testCounter++;
        }
        #region AssistingMethods
        private static Tile CreateTile(int x, int y)
        {
            Tile tile;
            tile.x = x;
            tile.y = y;
            return tile;
        }
        private static Piece[,] CreateBoard()
        {
            Piece[,] board = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new EmptyTile();
                }
            }
            return board;
        }
        private static Piece[,] HasMoved(Piece[,] board, Tile moved)
        {
            if (board[moved.x, moved.y] is King king)
            {
                king.HasMoved = true;
                board[moved.x, moved.y] = king;
            }
            if (board[moved.x, moved.y] is Rook rook)
            {
                rook.HasMoved = true;
                board[moved.x, moved.y] = rook;
            }
            return board;
        }
        private static string[] BoardVisualisation(Piece[,] board)
        {
            //A method designed to be used in a breakpoint,
            //To track the exact board state at any time
            string[] visualisation = new string[8];
            for (int i = 0; i < 8; i++)
            {
                string add = "";
                for (int j = 0; j < 8; j++)
                {
                    string color = board[i, j].whitePiece ? "w" : "b";
                    if (board[i, j] is EmptyTile)
                    {
                        color = "e";
                    }
                    add += $" {color}{board[i, j].ToString()[0]} |";
                }
                visualisation[i] = add;
            }
            return visualisation;
        }
        private static void AddMoveToList(Tile fromTile, Tile toTile)
        {
            fromTiles.Add(fromTile);
            toTiles.Add(toTile);
        }
        private static void RemoveMove(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                fromTiles.RemoveAt(fromTiles.Count - 1);
                toTiles.RemoveAt(toTiles.Count - 1);
            }
        }
        #endregion
    }
}
