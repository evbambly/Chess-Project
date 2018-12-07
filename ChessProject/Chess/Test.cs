using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    class Test
    {
        //A testing class, designed to input data and see if it works
        //Always set Chess.flipBoard to False before testing
        static MoveStats move = CreateMoveStats();
        static int testCounter = 1;
        public static void TestMoveCheckBishop()
        {
            move.whiteTurn = true;

            move.board[4, 4] = Chess.Bishop(move.whiteTurn);
            move.board[6, 2] = Chess.Pawn(!move.whiteTurn);
            move.board[3, 5] = Chess.King(move.whiteTurn);

            ExecuteMoveCheckTest(CreateTile(4, 4), CreateTile(5, 6), false);
            ExecuteMoveCheckTest(CreateTile(4, 4), CreateTile(3, 3), true);
            ExecuteMoveCheckTest(CreateTile(4, 4), CreateTile(7, 1), false);
            ExecuteMoveCheckTest(CreateTile(4, 4), CreateTile(2, 6), false);
            //Tile amended
            move.whiteTurn = false;
            move.board[4, 3] = Chess.Bishop(move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(4, 3), CreateTile(6, 5), true);
            ExecuteMoveCheckTest(CreateTile(4, 3), CreateTile(1, 0), true);
            ExecuteMoveCheckTest(CreateTile(4, 3), CreateTile(1, 6), true);
            ExecuteMoveCheckTest(CreateTile(4, 3), CreateTile(5, 2), true);
        }
        public static void TestMoveCheckKing()
        {
            bool testCastlingRules = true;
            if (!testCastlingRules)
            {
            move.whiteTurn = true;
                move.board[2, 5] = Chess.King(move.whiteTurn);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(1, 6), true);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(1, 5), true);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(1, 4), true);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(2, 4), true);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(3, 4), true);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(3, 5), true);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(3, 6), true);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(2, 6), true);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(1, 7), false);
                ExecuteMoveCheckTest(CreateTile(2, 5), CreateTile(0, 5), false);
            }
            if (testCastlingRules)
            {
                move.whiteTurn = true;
                move.board[0, 4] = Chess.King(move.whiteTurn);
                move.board[0, 0] = Chess.Rook(move.whiteTurn);
                move.board[0, 7] = Chess.Rook(move.whiteTurn);
                move.board[7, 4] = Chess.King(!move.whiteTurn);
                move.board[7, 0] = Chess.Rook(!move.whiteTurn);
                move.board[7, 7] = Chess.Rook(!move.whiteTurn);
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(0, 2), true);
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(0, 6), true);
                move.whiteTurn = false;
                ExecuteMoveCheckTest(CreateTile(7, 4), CreateTile(7, 2), true);
                ExecuteMoveCheckTest(CreateTile(7, 4), CreateTile(7, 6), true);
                move.whiteTurn = true;
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(1, 2), false);
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(7, 6), false);
                //Condition change
                move.canPlayerCastle.rightRookHasMoved = true;
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(0, 6), false);
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(0, 2), true);
                //Tile amended
                move.board[0, 0] = Chess.EmptyTile;
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(0, 2), false);
                //Condition change
                move.whiteTurn = false;
                ExecuteMoveCheckTest(CreateTile(7, 4), CreateTile(7, 2), true);
                //Condition change
                move.canPlayerCastle.kingHasMoved = true;
                ExecuteMoveCheckTest(CreateTile(7, 4), CreateTile(7, 2), false);
                //Condition reset
                move = CreateMoveStats();
                move.whiteTurn = true;
                move.board[0, 4] = Chess.King(move.whiteTurn);
                move.board[0, 0] = Chess.Rook(move.whiteTurn);
                move.board[0, 7] = Chess.Rook(move.whiteTurn);
                move.board[7, 4] = Chess.King(!move.whiteTurn);
                move.board[7, 0] = Chess.Rook(!move.whiteTurn);
                move.board[7, 7] = Chess.Rook(!move.whiteTurn);
                move.whiteTurn = false;
                move.board[7, 5] = Chess.Bishop(move.whiteTurn);
                ExecuteMoveCheckTest(CreateTile(7, 4), CreateTile(7, 6), false);
                //Tile amended
                move.board[7, 5] = Chess.EmptyTile;
                move.board[7, 6] = Chess.Bishop(!move.whiteTurn);
                ExecuteMoveCheckTest(CreateTile(7, 4), CreateTile(7, 6), false);
                ExecuteMoveCheckTest(CreateTile(7, 4), CreateTile(7, 2), true);
                //Tile amended
                move.board[7, 1] = Chess.Bishop(!move.whiteTurn);
                ExecuteMoveCheckTest(CreateTile(7, 4), CreateTile(7, 2), false);
                //Condition change
                move.whiteTurn = true;
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(0, 1), false);
                ExecuteMoveCheckTest(CreateTile(0, 4), CreateTile(0, 7), false);
            }
        }
        public static void TestMoveCheckKnight()
        {
            move.whiteTurn = true;
            move.board[3, 3] = Chess.Knight(move.whiteTurn);
            move.board[1, 1] = Chess.Queen(!move.whiteTurn);
            move.board[1, 3] = Chess.Queen(!move.whiteTurn);
            move.board[1, 5] = Chess.Queen(!move.whiteTurn);
            move.board[2, 2] = Chess.Queen(move.whiteTurn);
            move.board[2, 3] = Chess.Queen(move.whiteTurn);
            move.board[2, 4] = Chess.Queen(!move.whiteTurn);
            move.board[3, 1] = Chess.Queen(move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(4, 4), false);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(5, 5), false);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(2, 2), false);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(1, 1), false);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(1, 2), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(1, 4), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(2, 5), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(4, 5), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(5, 2), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(5, 4), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(2, 1), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(4, 1), true);
        }
        public static void TestMoveCheckPawn()
        {
            move.whiteTurn = true;            
            move.board[1, 4] = Chess.Pawn(move.whiteTurn);
            move.board[6, 4] = Chess.Pawn(!move.whiteTurn);

            ExecuteMoveCheckTest(CreateTile(1, 4), CreateTile(2, 4), true);
            move.whiteTurn = false;
            ExecuteMoveCheckTest(CreateTile(6, 4), CreateTile(5, 4), true);
            //Tile amended
            move.board[5, 4] = Chess.Queen(!move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(6, 4), CreateTile(5, 4), false);
            //Tile amended
            move.board[5, 4] = Chess.EmptyTile;
            ExecuteMoveCheckTest(CreateTile(6, 4), CreateTile(4, 4), true);
            move.whiteTurn = true;
            ExecuteMoveCheckTest(CreateTile(1, 4), CreateTile(3, 4), true);
            //Tile amended
            move.board[2, 0] = Chess.Pawn(move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(2, 0), CreateTile(3, 0), true);
            ExecuteMoveCheckTest(CreateTile(2, 0), CreateTile(4, 0), false);
            //Tile amended
            move.board[3, 4] = Chess.Rook(move.whiteTurn);
            move.board[5, 4] = Chess.King(!move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(1, 4), CreateTile(3, 4), false);
            move.whiteTurn = false;
            ExecuteMoveCheckTest(CreateTile(6, 4), CreateTile(4, 4), false);
            ExecuteMoveCheckTest(CreateTile(6, 4), CreateTile(5, 3), false);
            ExecuteMoveCheckTest(CreateTile(6, 4), CreateTile(5, 5), false);
            //Tile amended
            move.board[5, 3] = Chess.Knight(!move.whiteTurn);
            move.board[3, 1] = Chess.Knight(move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(6, 4), CreateTile(5, 3), true);
            move.whiteTurn = true;
            ExecuteMoveCheckTest(CreateTile(2, 0), CreateTile(3, 1), true);
            //Tile amended
            move.board[4, 1] = Chess.Knight(!move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(2, 0), CreateTile(4, 1), false);
            //Tile amended
            move.board[3, 1] = Chess.EmptyTile;
            ExecuteMoveCheckTest(CreateTile(2, 0), CreateTile(4, 1), false);
            //Tile amended
            move.board[3, 1] = Chess.Pawn(move.whiteTurn);
            move.board[3, 2] = Chess.Pawn(!move.whiteTurn);
            move.whiteTurn = false;
            ExecuteMoveCheckTest(CreateTile(3, 2), CreateTile(2, 1), false);
            //Condition change
            move.enPassant.x = 3;
            move.enPassant.y = 1;
            ExecuteMoveCheckTest(CreateTile(3, 2), CreateTile(2, 1), true);
            //Tile amended 
            move.board[3, 3] = Chess.Pawn(move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(2, 2), false);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(2, 1), false);
            ExecuteMoveCheckTest(CreateTile(3, 2), CreateTile(2, 3), false);
            ExecuteMoveCheckTest(CreateTile(3, 2), CreateTile(4, 2), false);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(4, 3), false);
            move.whiteTurn = true;
            ExecuteMoveCheckTest(CreateTile(3, 1), CreateTile(2, 1), false);
            ExecuteMoveCheckTest(CreateTile(2, 0), CreateTile(1, 1), false);
        }
        public static void TestMoveCheckQueen()
        {
            move.whiteTurn = true;

            move.board[4, 4] = Chess.Queen(move.whiteTurn);
            move.board[6, 2] = Chess.Pawn(!move.whiteTurn);
            move.board[3, 5] = Chess.King(move.whiteTurn);

            ExecuteMoveCheckTest(CreateTile(4, 4), CreateTile(5, 6), false);
            ExecuteMoveCheckTest(CreateTile(4, 4), CreateTile(3, 3), true);
            ExecuteMoveCheckTest(CreateTile(4, 4), CreateTile(7, 1), false);
            ExecuteMoveCheckTest(CreateTile(4, 4), CreateTile(2, 6), false);
            //Tile amended
            move.whiteTurn = false;
            move.board[4, 3] = Chess.Queen(move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(4, 3), CreateTile(6, 5), true);
            ExecuteMoveCheckTest(CreateTile(4, 3), CreateTile(1, 0), true);
            ExecuteMoveCheckTest(CreateTile(4, 3), CreateTile(1, 6), true);
            ExecuteMoveCheckTest(CreateTile(4, 3), CreateTile(5, 2), true);

            move.board[3, 3] = Chess.Queen(move.whiteTurn);
            move.board[3, 1] = Chess.Pawn(!move.whiteTurn);

            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(3, 2), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(3, 0), false);
            //Tile amended
            move.board[4, 3] = Chess.EmptyTile;
            move.board[6, 3] = Chess.King(move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(7, 3), false);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(5, 3), true);
            //Tile amended
            move.board[4, 3] = Chess.EmptyTile;
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(5, 3), true);
            //Tile amended
            move.whiteTurn = true;
            move.board[6, 2] = Chess.Queen(move.whiteTurn);
            move.board[6, 3] = Chess.EmptyTile;
            ExecuteMoveCheckTest(CreateTile(6, 2), CreateTile(6, 7), true);
            ExecuteMoveCheckTest(CreateTile(6, 2), CreateTile(2, 2), true);
        }
        public static void TestMoveCheckRook()
        {
            move.whiteTurn = false;
            move.board[3, 3] = Chess.Rook(move.whiteTurn);
            move.board[3, 1] = Chess.Pawn(!move.whiteTurn);

            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(3, 2), true);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(3, 0), false);
            //Tile amended
            move.board[6, 3] = Chess.King(move.whiteTurn);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(7, 3), false);
            ExecuteMoveCheckTest(CreateTile(3, 3), CreateTile(5, 3), true);
            //Tile amended
            move.whiteTurn = true;
            move.board[6, 2] = Chess.Rook(move.whiteTurn);
            move.board[6, 3] = Chess.EmptyTile;
            ExecuteMoveCheckTest(CreateTile(6, 2), CreateTile(6, 7), true);
            ExecuteMoveCheckTest(CreateTile(6, 2), CreateTile(2, 2), true);
        }
        public static void TestChessCrownPawn()
        {
            Chess chess = new Chess();
            chess.MakeMove(CreateTile(0, 5), CreateTile(5, 5));
            chess.MakeMove(CreateTile(6, 3), CreateTile(1, 5));
            chess.MakeMove(CreateTile(0,1), CreateTile(2,2));
            chess.Play();
        }
        public static void EnPassantSetup()
        {
            Chess chess = new Chess();
            chess.MakeMove(CreateTile(0, 7), CreateTile(1, 7));
            chess.MakeMove(CreateTile(6, 4), CreateTile(4, 4));
            chess.MakeMove(CreateTile(0, 1), CreateTile(2, 2));
            chess.MakeMove(CreateTile(4, 4), CreateTile(3, 4));
            chess.MakeMove(CreateTile(1, 5), CreateTile(3, 5));
            chess.Play();
        }
        public static void CheckSetup()
        {
            Chess chess = new Chess();
            chess.MakeMove(CreateTile(1, 3), CreateTile(3, 3));
            chess.MakeMove(CreateTile(6, 2), CreateTile(4, 2));
            chess.MakeMove(CreateTile(3, 3), CreateTile(4, 2));
            chess.MakeMove(CreateTile(7, 3), CreateTile(4, 0));
            chess.Play();
        }
            enum StaleMateTest
        {
            moveCounter, sameStateCounter, lackOfMaterial, cantMove
        }
        public static void TestStaleMate()
        {
            StaleMateTest test = StaleMateTest.cantMove;
            Chess chess = new Chess();
            if (test == StaleMateTest.moveCounter)
            {
                //Set maxNonLinearMoves to three before this test;
                chess.MakeMove(CreateTile(0, 1), CreateTile(2, 0));
                chess.MakeMove(CreateTile(7, 1), CreateTile(5, 0));
                chess.Play();
            }
            else if (test == StaleMateTest.sameStateCounter)
            {
                chess.MakeMove(CreateTile(0, 1), CreateTile(2, 0));
                chess.MakeMove(CreateTile(7, 1), CreateTile(5, 0));
                chess.MakeMove(CreateTile(2, 0), CreateTile(0, 1));
                chess.MakeMove(CreateTile(5, 0), CreateTile(7, 1));
                chess.MakeMove(CreateTile(0, 1), CreateTile(2, 0));
                chess.MakeMove(CreateTile(7, 1), CreateTile(5, 0));
                chess.MakeMove(CreateTile(2, 0), CreateTile(0, 1));
                chess.Play();
            }
            else if (test == StaleMateTest.lackOfMaterial)
            {
                move.board[1, 4] = Chess.King(move.whiteTurn);
                move.board[2, 0] = Chess.Rook(move.whiteTurn);
                move.board[7, 5] = Chess.King(!move.whiteTurn);
                ExecuteStaleMateTest(move, false);
                move.board[2, 0] = Chess.EmptyTile;
                ExecuteStaleMateTest(move, true);
                move.board[2, 2] = Chess.Bishop(!move.whiteTurn);
                ExecuteStaleMateTest(move, true);
                move.board[7, 7] = Chess.Knight(move.whiteTurn);
                ExecuteStaleMateTest(move, true);
                move.board[7, 0] = Chess.Knight(move.whiteTurn);
                ExecuteStaleMateTest(move, false);
            }
            else
            {
                move.whiteTurn = false;
                move.board[5, 6] = Chess.King(move.whiteTurn);
                move.board[4, 6] = Chess.Pawn(move.whiteTurn);
                move.board[3, 6] = Chess.Pawn(!move.whiteTurn);
                move.board[7, 5] = Chess.King(!move.whiteTurn);
                move.board[2, 5] = Chess.Rook(!move.whiteTurn);
                ExecuteStaleMateTest(move, false);
                move.board[2, 7] = Chess.Rook(!move.whiteTurn);
                ExecuteStaleMateTest(move, true);                
                move.board[3, 7] = Chess.Queen(!move.whiteTurn);
                ExecuteStaleMateTest(move, false);
            }
        }
        public static void TestCheckMate()
        {
            move.whiteTurn = true;
            move.board[0, 1] = Chess.King(move.whiteTurn);
            move.board[0, 5] = Chess.Rook(!move.whiteTurn);
            move.board[1, 4] = Chess.Queen(!move.whiteTurn);            
            ExecuteCheckMateTest(move, CreateTile(0, 5), true);
            //Tile amended
            move.board[7, 5] = Chess.Rook(move.whiteTurn);
            ExecuteCheckMateTest(move, CreateTile(0, 5), false);
            //Tile amended
            move.board[7, 5] = Chess.EmptyTile;
            move.board[4, 2] = Chess.Rook(move.whiteTurn);
           ExecuteCheckMateTest(move, CreateTile(0, 5), false);
            move.board[3, 4] = Chess.Bishop(!move.whiteTurn);
            ExecuteCheckMateTest(move, CreateTile(0, 5), true);          
            //Tile amended
            move.board[4, 2] = Chess.EmptyTile;
            move.board[1, 1] = Chess.Pawn(move.whiteTurn);            
            ExecuteCheckMateTest(move, CreateTile(0, 5), false);
        }
        private static void ExecuteCheckMateTest(MoveStats move, Tile attackerTile, bool desiredTestResult)
        {
            bool testResult = MoveCheck.IsGameCheckMate(move, attackerTile);
            string success = desiredTestResult == testResult ? "Sucess!" : "***";
            Console.WriteLine($"Test {testCounter} expected {desiredTestResult}, is {testResult}. {success}");
            testCounter++;
        }
        private static void ExecuteStaleMateTest(MoveStats move, bool desiredTestResult)
        {
            bool testResult = MoveCheck.IsGameStaleMate(move, 0, new Dictionary<char[,], int>());
            string success = desiredTestResult == testResult ? "Success!" : "***";
            Console.WriteLine($"Test {testCounter} expected {desiredTestResult}, is {testResult}. {success}");
            testCounter++;

        }
        private static void ExecuteMoveCheckTest(Tile fromTile, Tile toTile, bool desiredTestResult)
        {
            bool testResult = MoveCheck.IsMoveLegal(move, fromTile, toTile);
            string success = desiredTestResult == testResult ? "Success!" : "***";
            Console.WriteLine($"Test {testCounter} expected {desiredTestResult}, is {testResult}. {success}");

            testCounter++;
        }
        private static Tile CreateTile(int x, int y)
        {
            Tile tile;
            tile.x = x;
            tile.y = y;
            return tile;
        }
        private static MoveStats CreateMoveStats()
        {
            MoveStats move;
            move.board = new char[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    move.board[i, j] = Chess.EmptyTile;
                }
            }

            move.canPlayerCastle.kingHasMoved = false;
            move.canPlayerCastle.leftRookHasMoved = false;
            move.canPlayerCastle.rightRookHasMoved = false;
            move.whiteTurn = false;
            move.enPassant.x = -1;
            move.enPassant.y = -1;
            return move;

        }
    }
}
