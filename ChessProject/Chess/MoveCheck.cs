using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    //Static service class to take care of legality of moves
    public static class MoveCheck
    {

        public static bool IsGameStaleMate(MoveStats moveStats, int stalemateMoveCount, Dictionary<char[,], int> boardStatesCount)
        {
            /*
             * A stalemate is a situation in which one of the following is true:
             * 1. 50 plays have been made without moving a Pawn or capturing a piece
             * 2. The same board arrangement appears for the 3rd time
             * 3. Both players don't have enough material to win
             * 4. The player cannot legally move without being in Check
             */
            bool staleMate = false;
            char[,] board = ExtractBoard(moveStats);
            bool whiteTurn = moveStats.whiteTurn;
            List<Tile> playerPieces = Chess.FindPlayerPieces(board, whiteTurn);
            List<Tile> foePieces = Chess.FindPlayerPieces(board, !whiteTurn);
            Tile piece;

            if (stalemateMoveCount == Chess.MaxNonLinearMoves)
            {
                staleMate = true;
            }
            foreach (int stateCounter in boardStatesCount.Values)
            {
                if (stateCounter == 3)
                {
                    staleMate = true;
                }
            }
            if (!staleMate)
            {
                //If both players don't have enough material to force a checkmate 
                //(King | King + Knight | King + Bishop) -> It is a stalemate
                if (playerPieces.Count < 3 && foePieces.Count < 3)
                {
                    bool playerHasEnoughMaterial = playerPieces.Count == 1 ? false : true;
                    bool foeHasEnoughMaterial = foePieces.Count == 1 ? false : true;

                    for (int i = 0; i < playerPieces.Count; i++)
                    {
                        piece = playerPieces[i];
                        if (board[piece.x, piece.y] == Chess.Knight(whiteTurn))
                        {
                            playerHasEnoughMaterial = false;
                        }
                        if (board[piece.x, piece.y] == Chess.Bishop(whiteTurn))
                        {
                            playerHasEnoughMaterial = false;
                        }
                    }
                    for (int i = 0; i < foePieces.Count; i++)
                    {
                        piece = foePieces[i];
                        if (board[piece.x, piece.y] == Chess.Knight(!whiteTurn))
                        {
                            foeHasEnoughMaterial = false;
                        }
                        if (board[piece.x, piece.y] == Chess.Bishop(!whiteTurn))
                        {
                            foeHasEnoughMaterial = false;
                        }
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
                    Tile tryMove;
                    bool playerCannotMove = true;
                    foreach (Tile playerPiece in playerPieces)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                tryMove.x = i;
                                tryMove.y = j;
                                if (IsMoveSane(moveStats, playerPiece, tryMove))
                                {
                                    if (IsMoveLegal(moveStats, playerPiece, tryMove))
                                    {
                                        if (!IsPlayerInCheck(moveStats, playerPiece, tryMove))
                                        {
                                            playerCannotMove = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (playerCannotMove)
                    {
                        staleMate = true;
                    }
                }
            }

            return staleMate;

        }
        public static bool IsGameCheckMate(MoveStats moveStats, Tile landedOnTile)
        {
            /* CheckMate occurs under three conditions:
             * 1. The player is under Check
             * 2. Non of the player's pieces can capture the piece causing the Check (including the King)
             * 3. Non of the player's pieces can block the Check
             * 4. The King can't escape Check
             */
            char[,] board = ExtractBoard(moveStats);
            bool whiteTurn = moveStats.whiteTurn;
            bool checkMate = false;
            if (IsPlayerInCheck(moveStats, landedOnTile, landedOnTile))
            {
                checkMate = true;
                List<Tile> playerPieces = Chess.FindPlayerPieces(board, whiteTurn);
                List<Tile> foePieces = Chess.FindPlayerPieces(board, !whiteTurn);
                Tile king = Chess.FindKing(board, whiteTurn);
                MoveStats export = moveStats;
                export.board = board;
                List<Tile> foes = new List<Tile>();
                foreach (Tile foe in foePieces)
                {
                    if (IsMoveLegal(export, foe, king))
                    {
                        foes.Add(foe);
                    }
                }
                if (foes.Count == 1)
                {
                    //If there is only one Foe, it can be captured and the Check ended-
                    //if one of the player's pieces can capture the Foe
                    Tile foe = foes[0];
                    foreach (Tile piece in playerPieces)
                    {
                        if (IsMoveLegal(moveStats, piece, foe))
                        {
                            if (!IsPlayerInCheck(moveStats, piece, foe))
                            {
                                checkMate = false;
                            }
                        }
                    }
                    //If the Foe is a Bishop, Queen or Rook, 
                    //its move may be blocked by one of the player's pieces
                    if (board[foe.x, foe.y] != Chess.Knight(!whiteTurn))
                    {
                        if (board[foe.x, foe.y] != Chess.Pawn(!whiteTurn))
                        {
                            if (board[foe.x, foe.y] != Chess.King(!whiteTurn))
                            {
                                int xDistance = king.x - foe.x;
                                int yDistance = king.y - foe.y;
                                int xAdvance = xDistance > -1 ? 1 : -1;
                                int yAdvance = yDistance > -1 ? 1 : -1;
                                int tilesInBetween = 0;
                                if (xDistance < 0)
                                {
                                    xDistance *= -1;
                                }
                                if (yDistance < 0)
                                {
                                    yDistance *= -1;
                                }
                                if (xDistance == 0)
                                {
                                    tilesInBetween = yDistance - 1;
                                    xAdvance = 0;
                                }
                                if (yDistance == 0)
                                {
                                    tilesInBetween = xDistance - 1;
                                    yAdvance = 0;
                                }
                                if (xDistance == yDistance)
                                {
                                    tilesInBetween = xDistance - 1;
                                }
                                foreach (Tile piece in playerPieces)
                                {
                                    //The King can't block the King
                                    if (piece.x == king.x && piece.y == king.y)
                                    {
                                        continue;
                                    }
                                    Tile blockTile;
                                    blockTile.x = foe.x;
                                    blockTile.y = foe.y;
                                    for (int i = 0; i < tilesInBetween; i++)
                                    {
                                        blockTile.x += xAdvance;
                                        blockTile.y += yAdvance;
                                        if (IsMoveLegal(moveStats, piece, blockTile))
                                        {
                                            if (!IsPlayerInCheck(moveStats, piece, blockTile))
                                            {
                                                checkMate = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (checkMate)
                {
                    //If no piece can block or eat the foe,
                    //the King will attempt escaping to any tile around him
                    //If he can't- it's checkMate
                    for (int i = -1; checkMate && i < 2; i++)
                    {
                        for (int j = -1; checkMate && j < 2; j++)
                        {
                            if (king.x + i > -1 && king.x + i < 8)
                            {
                                if (king.y + j > -1 && king.y + j < 8)
                                {
                                    Tile escapeTile;
                                    escapeTile.x = king.x + i;
                                    escapeTile.y = king.y + j;
                                    if (IsMoveSane(moveStats, king, escapeTile))
                                    {
                                        if (!IsPlayerInCheck(moveStats, king, escapeTile))
                                        {
                                            checkMate = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return checkMate;
        }
        public static void ChooseLegalMove(MoveStats moveStats, out Tile chosenFromTile, out Tile chosenToTile)
        {
            //Making sure the input is correct, the move is sane and legal.
            //Makes sure move does not end in Check
            bool whiteTurn = moveStats.whiteTurn;
            bool legalMove = false;
            do
            {
                char[,] board = ExtractBoard(moveStats);
                string output = "";
                if (IsInputLegal(Console.ReadLine(), out Tile fromTile, out Tile toTile))
                {
                    if (IsMoveSane(moveStats, fromTile, toTile))
                    {
                        if (IsMoveLegal(moveStats, fromTile, toTile))
                        {
                            if (IsPlayerInCheck(moveStats, fromTile, toTile))
                            {
                                output = "Move will result in Check!";
                            }
                            else
                            {
                                legalMove = true;
                            }
                        }
                        else
                        {
                            output = "Move is not legal!";
                        }
                    }
                    else
                    {
                        output = "This is an invalid move!";
                    }
                }
                else
                {
                    output = "Invalid input!";
                }
                if (!legalMove)
                {
                    Console.Clear();
                    Chess.FancyPrintGame(board);
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
            fromTile.x = -1;
            fromTile.y = -1;
            toTile.x = -1;
            toTile.y = -1;
            input = input.ToUpper();
            if (input.Length == 4)
            {
                if (input[0] > 64 && input[0] < 73)
                {
                    if (input[2] > 64 && input[2] < 73)
                    {
                        if (int.TryParse(input[1].ToString(), out int a) && int.TryParse(input[3].ToString(), out int b))
                        {
                            if (a > 0 && a < 9 && b > 0 && b < 9)
                            {
                                inputIsLegal = true;
                                fromTile.y = input[0] - 65;
                                fromTile.x = int.Parse(input[1].ToString()) - 1;
                                toTile.y = input[2] - 65;
                                toTile.x = int.Parse(input[3].ToString()) - 1;
                            }
                        }
                    }

                }
            }
            return inputIsLegal;
        }
        private static bool IsMoveSane(MoveStats moveStats, Tile fromTile, Tile toTile)
        {
            //A sane move starts from the player's piece 
            //and finishes in an empty or foe tile (anything but the player's piece)
            char[,] board = ExtractBoard(moveStats);
            bool whiteTurn = moveStats.whiteTurn;
            bool moveIsSane = true;
            /* White player has pieces with a char value of between ~97-122, 
             * black player has pieces with a char value of ~65-90.
             * if we subtract (piece - playerBias) 
             * we should get a result between 0 and 25 for the player piece and anything else for foe/empty
             */
            int calcPlayerBias = whiteTurn ? 97 : 65;
            if (board[fromTile.x, fromTile.y] - calcPlayerBias < 0 || board[fromTile.x, fromTile.y] - calcPlayerBias > 25)
            {
                moveIsSane = false;
            }
            if (board[toTile.x, toTile.y] - calcPlayerBias > -1 && board[toTile.x, toTile.y] - calcPlayerBias < 26)
            {
                moveIsSane = false;
            }
            return moveIsSane;
        }
        private static bool IsPlayerInCheck(MoveStats moveStats, Tile fromTile, Tile toTile)
        {
            char[,] board = ExtractBoard(moveStats);
            bool whiteTurn = moveStats.whiteTurn;
            //A player is in Check if one or more of the foe's pieces can legally capture their King
            //According to the inputed board data
            bool playerInCheck = false;
            //To check for CheckMate, only one tile is passed
            //So if fromTile == toTile no amendment is required
            if (fromTile.x != toTile.x || fromTile.y != toTile.y)
            {
                board = Chess.AmendBoard(board, fromTile, toTile);
            }
            Tile king = Chess.FindKing(board, whiteTurn);
            List<Tile> foePieces = Chess.FindPlayerPieces(board, !whiteTurn);
            MoveStats export = moveStats;
            export.board = board;
            foreach (Tile foe in foePieces)
            {
                if (IsMoveLegal(export, foe, king))
                {
                    playerInCheck = true;
                }

            }
            return playerInCheck;
        }
        public static bool IsMoveLegal(MoveStats moveStats, Tile fromTile, Tile toTile)
        {
            //Central method- recieves requests from the ChooseLegalMove method, 
            //passes it to the piece-specific method and returns the answer
            char[,] board = ExtractBoard(moveStats);
            bool moveIsLegal = true;
            switch (board[fromTile.x, fromTile.y].ToString().ToUpper())
            {
                case "B":
                    moveIsLegal = Bishop(board, fromTile, toTile);
                    break;
                case "K":
                    moveIsLegal = King(moveStats, fromTile, toTile);
                    break;
                case "N":
                    moveIsLegal = Knight(fromTile, toTile);
                    break;
                case "P":
                    moveIsLegal = Pawn(moveStats, fromTile, toTile);
                    break;
                case "Q":
                    moveIsLegal = Queen(board, fromTile, toTile);
                    break;
                case "R":
                    moveIsLegal = Rook(board, fromTile, toTile);
                    break;
            }
            return moveIsLegal;
        }
        private static bool Bishop(char[,] board, Tile fromTile, Tile toTile)
        {
            bool moveIsLegal = true;
            //A Bishop's move is legal only if it is an equal distance on the x and y axes,
            //as well as being unobstructed to it's target tile

            int yDistance = toTile.y - fromTile.y;
            int yAdvance = 1;
            if (yDistance < 0)
            {
                yDistance *= -1;
                yAdvance = -1;
            }
            int xDistance = toTile.x - fromTile.x;
            int xAdvance = 1;
            if (xDistance < 0)
            {
                xDistance *= -1;
                xAdvance = -1;
            }
            if (yDistance != xDistance)
            {
                moveIsLegal = false;
            }
            else
            {

                //A loop verifying the path to the target tile is unobstructed,
                //The tile itself doesn't have to be checked since it is a SaneMove
                Tile checkEmptyTile = fromTile;
                int tilesInBetween = xDistance - 1;
                for (int i = 0; moveIsLegal && i < tilesInBetween; i++)
                {
                    checkEmptyTile.y += yAdvance;
                    checkEmptyTile.x += xAdvance;
                    if (!Chess.SameTile(checkEmptyTile, toTile))
                    {
                        if (board[checkEmptyTile.x, checkEmptyTile.y] != Chess.EmptyTile)
                        {
                            moveIsLegal = false;
                        }
                    }
                }
            }
            return moveIsLegal;
        }
        private static bool King(MoveStats moveStats, Tile fromTile, Tile toTile)
        {
            CanCastle canPlayerCastle = moveStats.canPlayerCastle;
            char[,] board = moveStats.board;
            bool whiteTurn = moveStats.whiteTurn;
            /* The King can ordinarily move -1/0/1 in both axes independently (except for 0,0)
             * Additionally, the King can Castle under the following conditions:
             * 1. The move is to the player's row (defined by who's turn it is (upper/lower case)), to tiles in either direction
             * 2. Relevant flags are checked (the two castling pieces haven't moved in the game)
             * 3. There are no pieces between the King and relevant Rook and the Rook is in the expected tile
             * 4. The King is not currnetly, nor will he pass or end in Check
             */
            bool moveIsLegal = true;
            int xDistance = toTile.x - fromTile.x;
            if (xDistance < -1 || xDistance > 1)
            {
                moveIsLegal = false;
            }
            int yDistance = toTile.y - fromTile.y;
            if (yDistance < -1 || yDistance > 1)
            {
                moveIsLegal = false;
            }
            //Checking for legal castling.  
            //moveIsLegal will be restored to True in loops and remain so if it passes them
            if (xDistance == 0 && (yDistance == 2 || yDistance == -2))
            {
                //Finding out which Rook the player is trying to castle with, and making sure it has not moved
                bool rookHasMoved = yDistance == -2 ? canPlayerCastle.leftRookHasMoved : canPlayerCastle.rightRookHasMoved;
                if (!canPlayerCastle.kingHasMoved && !rookHasMoved)
                {
                    //A loop making sure there are only empty pieces until the Rook, which is at y == 0 or y == 8
                    //There are 3 blocks to check in a kingside castling and 2 blocks to check in a queenside castling
                    moveIsLegal = true;
                    Tile currentTile;
                    int advance = yDistance == 2 ? 1 : -1;
                    int checkLength = advance == 1 ? 2 : 3;
                    currentTile.x = fromTile.x;
                    currentTile.y = fromTile.y + advance;
                    for (int count = 0; count < checkLength; count++)
                    {
                        if (board[currentTile.x, currentTile.y] != Chess.EmptyTile)
                        {
                            moveIsLegal = false;
                        }
                        currentTile.y += advance;
                    }
                    //Now we check if the Rook is in the expected position
                    int yRook = advance == 1 ? 7 : 0;
                    if (board[fromTile.x, yRook] != Chess.Rook(whiteTurn))
                    {
                        moveIsLegal = false;
                    }
                    //Now we check if the King is in Check, 
                    //or will be in Check in the middle or destination tile
                    //There are always three tiles to check
                    currentTile.x = fromTile.x;
                    currentTile.y = fromTile.y;
                    for (int count = 0; count < 3; count++)
                    {
                        if (IsPlayerInCheck(moveStats, fromTile, currentTile))
                        {
                            moveIsLegal = false;
                        }
                        currentTile.y += advance;
                    }
                }
            }
            return moveIsLegal;
        }
        private static bool Knight(Tile fromTile, Tile toTile)
        {
            //A Knight's move is legal if one of the axes has a distance by 2|-2 and the other has a distance of 1|-1
            bool moveIsLegal = false;
            int yDistance = toTile.y - fromTile.y;
            int xDistance = toTile.x - fromTile.x;
            bool advanceTwo = false;
            bool advanceOne = false;
            if (yDistance == 2 | yDistance == -2 | xDistance == 2 | xDistance == -2)
            {
                advanceTwo = true;
            }
            if (yDistance == 1 | yDistance == -1 | xDistance == 1 | xDistance == -1)
            {
                advanceOne = true;
            }
            if (advanceTwo && advanceOne)
            {
                moveIsLegal = true;
            }
            return moveIsLegal;
        }
        private static bool Pawn(MoveStats moveStats, Tile fromTile, Tile toTile)
        {
            char[,] board = moveStats.board;
            bool whiteTurn = moveStats.whiteTurn;
            Tile enPassant = moveStats.enPassant;
            /* A Pawn can move under the following conditions: 
             * 1. Forward (+1x, y) in it's direction of movement, unless obstructed by anything
             * 2. Twice forward (+2x, y) if it's in the initial position and not obstructed-
             * White's initial position is (x = 1), Black's is (x = 6)
             * 3. Diagonally one step forward (+1x, -1y / +1x, +1y) if there is an foe piece at that position
             * 4. Diagonally one step forward if the enPassant is adjacent to fromTile (x, -1y / x, +1y)
             * *and* the move is to behind the enPassant (+1x, y relative to the enPassant)
             */
            bool moveIsLegal = false;
            int xDistance = toTile.x - fromTile.x;
            int yDistance = toTile.y - fromTile.y;
            int initialPosition = 6;
            int xAdvance = -1;
            if (whiteTurn)
            {
                initialPosition = 1;
                xAdvance = 1;
            }
            if (yDistance == 0 && xDistance == xAdvance)
            {
                if (board[toTile.x, toTile.y] == Chess.EmptyTile)
                {
                    moveIsLegal = true;
                }
            }
            if (yDistance == 0 && xDistance == xAdvance * 2)
            {
                if (fromTile.x == initialPosition)
                {
                    if (board[toTile.x - xAdvance, toTile.y] == Chess.EmptyTile)
                    {
                        if (board[toTile.x, toTile.y] == Chess.EmptyTile)
                        {
                            moveIsLegal = true;
                        }
                    }
                }
            }
            if ((yDistance == -1 || yDistance == 1) && xDistance == xAdvance)
            {
                if (board[toTile.x, toTile.y] != Chess.EmptyTile)
                {
                    moveIsLegal = true;
                }
                //The piece performing an EnPassant must be adjacent to it's target
                //and it's toTile is behind the target
                if (enPassant.y == fromTile.y - 1 || enPassant.y == fromTile.y + 1)
                {
                    if (enPassant.x == fromTile.x)
                    {
                        if (toTile.x == enPassant.x + xAdvance && toTile.y == enPassant.y)
                        {
                            moveIsLegal = true;
                        }
                    }
                }
            }
            return moveIsLegal;
        }
        private static bool Queen(char[,] board, Tile fromTile, Tile toTile)
        {
            //A Queen can operate *either* as a Rook *or* as a Bishop
            //If she makes a move legal as one it will be illegal as the other
            bool moveIsLegal = false;
            if (Bishop(board, fromTile, toTile) | Rook(board, fromTile, toTile))
            {
                moveIsLegal = true;
            }
            return moveIsLegal;
        }
        private static bool Rook(char[,] board, Tile fromTile, Tile toTile)
        {
            //A Rook's move is legal only if one of the axes changes by 0,
            //as well as being unobstructed to it's target tile
            bool moveIsLegal = true;
            int yDistance = toTile.y - fromTile.y;
            int xDistance = toTile.x - fromTile.x;
            if (yDistance != 0 && xDistance != 0)
            {
                moveIsLegal = false;
            }
            //Finding the x and y directionality- +1, -1 or 0
            int yAdvance = 1;
            if (yDistance < 1)
            {
                if (yDistance < 0)
                {
                    yAdvance = -1;
                }
                else
                {
                    yAdvance = 0;
                }
            }
            int xAdvance = 1;
            if (xDistance < 1)
            {
                if (xDistance < 0)
                {
                    xAdvance = -1;
                }
                else
                {
                    xAdvance = 0;
                }
            }
            //A loop verifying the path to the target tile is unobstructed,
            //The tile itself doesn't have to be checked since it is a SaneMove
            //Finding which axis' stats need to be plugged into the for loop
            int tilesInBetween;
            if (xDistance != 0)
            {
                if (xDistance > 0)
                {
                    tilesInBetween = xDistance;
                }
                else
                {
                    tilesInBetween = xDistance * -1;
                }
            }
            else
            {
                if (yDistance > 0)
                {
                    tilesInBetween = yDistance;
                }
                else
                {
                    tilesInBetween = yDistance * -1;
                }
            }
            Tile checkEmptyTile = fromTile;
            //The amount of tiles inBetween is the total amount of tiles (including fromTile & toTile) -2.
            //So inbetween e4 and e6 we only have e4, e5, e6 minus the edges.
            for (int i = 0; moveIsLegal && i < tilesInBetween; i++)
            {
                checkEmptyTile.y += yAdvance;
                checkEmptyTile.x += xAdvance;
                if (!Chess.SameTile(checkEmptyTile, toTile))
                {
                    if (board[checkEmptyTile.x, checkEmptyTile.y] != Chess.EmptyTile)
                    {
                        moveIsLegal = false;
                    }
                }

            }
            return moveIsLegal;
        }
        private static char[,] ExtractBoard(MoveStats moveStats)
        {
            char[,] board = new char[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = moveStats.board[i, j];
                }
            }
            return board;
        }
    }
}
