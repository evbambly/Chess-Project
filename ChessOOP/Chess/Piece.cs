using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public abstract class Piece
    {
        public readonly bool whitePiece;
        public Piece(bool whitePiece)
        {
            this.whitePiece = whitePiece;
        }
        protected bool moveIsLegal;
        protected int xDistance, yDistance;
        public virtual bool IsMoveLegal(Tile fromTile, Tile toTile)
        {
            moveIsLegal = false;
            yDistance = Math.Abs(toTile.y - fromTile.y);
            xDistance = Math.Abs(toTile.x - fromTile.x);
            return moveIsLegal;
        }
        public virtual bool SecondMove(Piece[,] board, Tile fromTile, Tile toTile)
        {
            return false;
        }
        public abstract override string ToString();
    }
    public class EmptyTile : Piece
    {
        public EmptyTile() : base(false) { }
        public override string ToString()
        {
            return "empty";
        }
    }
    public class Bishop : Piece
    {
        public Bishop(bool whitePiece) : base(whitePiece) { }
        public override bool IsMoveLegal(Tile fromTile, Tile toTile)
        {
            //A Bishop's move is legal only if it is an equal distance on the x and y axes
            base.IsMoveLegal(fromTile, toTile);
            return yDistance == xDistance;
        }
        public override string ToString()
        {
            return "bishop";
        }
    }
    public class King : Piece
    {
        private bool hasMoved;
        public bool HasMoved
        {
            get => hasMoved;
            set => hasMoved = true;
        }
        public King(bool whitePiece) : base(whitePiece)
        {
            hasMoved = false;
        }
        public override bool IsMoveLegal(Tile fromTile, Tile toTile)
        {
            // The King can move -1/0/1 in both axes independently (except for 0,0)
            //A King's special move is castling
            base.IsMoveLegal(fromTile, toTile);
            return xDistance < 2 && yDistance < 2;
        }
        public override bool SecondMove(Piece[,] board, Tile fromTile, Tile toTile)
        {
            /* The King can Castle under the following conditions:
            * 1. The move is to the player's row, two tiles in either direction
            * 2. The two castling pieces haven't moved in the game
            * 3. There are no pieces between the King and relevant Rook and the correct Rook is in the expected tile
            * 4. The King is not currnetly, nor will he pass or end in Check
            */
            base.IsMoveLegal(fromTile, toTile);
            if (xDistance == 0 && yDistance == 2 && !hasMoved)
            {
                //Finding out which Rook the player is trying to castle with, and making sure it has not moved
                Tile rookPos = Chess.CreateTile(fromTile.x, toTile.y > fromTile.y ? 7 : 0);
                if (board[rookPos.x, rookPos.y] is Rook rook && !rook.HasMoved)
                {
                    moveIsLegal = MoveCheck.IsMoveUnobstructed(board, fromTile, rookPos);
                    //Now we check if the King is in Check, 
                    //or will be in Check in the middle or destination tile
                    //There are always three tiles to check
                    Tile currentTile = Chess.CreateTile(fromTile.x, fromTile.y);
                    int yAdvance = toTile.y > fromTile.y ? 1 : -1;
                    for (int count = 0; count < 3; count++)
                    {
                        if (MoveCheck.IsPlayerInCheck(board, fromTile, currentTile))
                        {
                            moveIsLegal = false;
                        }
                        currentTile.y += yAdvance;
                    }
                }
            }
            return moveIsLegal;
        }
        public override string ToString()
        {
            return "king";
        }
    }
    public class Knight : Piece
    {
        public Knight(bool whitePiece) : base(whitePiece) { }
        public override bool IsMoveLegal(Tile fromTile, Tile toTile)
        {
            //A Knight's move is legal if one of the axes has a distance by 2|-2 and the other has a distance of 1|-1
            base.IsMoveLegal(fromTile, toTile);
            bool advanceTwo = yDistance == 2 | xDistance == 2 ? true : false;
            bool advanceOne = yDistance == 1 | xDistance == 1 ? true : false;
            return advanceOne && advanceTwo;
        }
        public override string ToString()
        {
            //In order to convert the Knight to the 'n' symbol on the board
            return "nKnight";
        }
    }
    public class Pawn : Piece
    {
        public bool EnPassant
        {
            get; set;
        }
        private readonly int initialPosition;
        private readonly int xAdvance;
        public Pawn(bool whitePiece) : base(whitePiece)
        {
            EnPassant = false;
            initialPosition = whitePiece ? 1 : 6;
            xAdvance = whitePiece ? 1 : -1;
        }
        public override bool IsMoveLegal(Tile fromTile, Tile toTile)
        {
            //The Pawn is the only piece whose move always depends on the board's situation
            return false;
        }
        public override bool SecondMove(Piece[,] board, Tile fromTile, Tile toTile)
        {
            /* A Pawn's straight move is only under the following conditions: 
             * 1. Forward (+1x, y) in it's direction of movement, unless obstructed by anything
             * 2. Twice forward (+2x, y) if it's in the initial position and not obstructed-
             * White's initial position is (x = 1), Black's is (x = 6)
             * A Pawn's special move is to Capture an enemy piece
             */
            base.IsMoveLegal(fromTile, toTile);
            bool moveIsLegal = false;
            xDistance = toTile.x - fromTile.x;
            if (toTile.y - fromTile.y == 0 && board[toTile.x, toTile.y] is EmptyTile)
            {
                if (xDistance == xAdvance)
                {
                    moveIsLegal = true;
                }
                else if (xDistance == xAdvance * 2 && fromTile.x == initialPosition)
                {
                    moveIsLegal = true;
                }
            }
            /* A Pawn's diagonal move to capture an enemy piece, under the following conditions:
             * 1.Diagonally one step forward(+1x, -1y / +1x, +1y) if there is an foe piece at that position
             * 2.Diagonally one step forward if the Pawn is moving behind the enPassant (-1x, y)
             * And condition 1 otherwise applies
             */
            if (!moveIsLegal && yDistance == 1 && xDistance == xAdvance)
            {
                moveIsLegal = board[toTile.x, toTile.y] is EmptyTile ? false : true;
                if (!moveIsLegal && board[toTile.x - xAdvance, toTile.y] is Pawn pawn)
                {
                    moveIsLegal = pawn.EnPassant && pawn.whitePiece != this.whitePiece ? true : false;
                }
            }
            return moveIsLegal;
        }
        public override string ToString()
        {
            return "pawn";
        }
    }
    public class Queen : Piece
    {
        public Queen(bool whitePiece) : base(whitePiece) { }
        public override bool IsMoveLegal(Tile fromTile, Tile toTile)
        {
            //A Queen's move is legal only if it is an equal distance on the x and y axes,
            //or if one of the axes changes by 0
            base.IsMoveLegal(fromTile, toTile);
            return yDistance == 0 | xDistance == 0 | yDistance == xDistance;
        }
        public override string ToString()
        {
            return "queen";
        }
    }
    public class Rook : Piece
    {
        private bool hasMoved;
        public bool HasMoved
        {
            get => hasMoved;
            set => hasMoved = true;
        }
        public Rook(bool whitePiece) : base(whitePiece)
        {
            hasMoved = false;
        }
        public override bool IsMoveLegal(Tile fromTile, Tile toTile)
        {
            //A Rook's move is legal only if one of the axes changes by 0
            base.IsMoveLegal(fromTile, toTile);
            return yDistance == 0 | xDistance == 0;
        }
        public override string ToString()
        {
            return "rook";
        }
    }
}