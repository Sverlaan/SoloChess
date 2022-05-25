using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    public class Piece
    {
        public int State;
        public int X, Y;
        public bool clicked;
        public int nCapturesLeft;

        public Node hor, vert, dig1, dig2;

        public Piece(int state, int x, int y, int c)
        {
            this.State = state;
            this.X = x;
            this.Y = y;
            nCapturesLeft = c;

            vert = new Node(this, null, null);
            hor = new Node(this, null, null);
            dig1 = new Node(this, null, null);
            dig2 = new Node(this, null, null);
        }
    }


    public class King : Piece
    {
        public King(int state, int x, int y, int c) : base(state, x, y, c) { }
    }


    public class Queen : Piece
    {
        public Queen(int state, int x, int y, int c) : base(state, x, y, c) { }
    }


    public class Rook : Piece
    {
        public Rook(int state, int x, int y, int c) : base(state, x, y, c) { }
    }


    public class Bishop : Piece
    {
        public Bishop(int state, int x, int y, int c) : base(state, x, y, c) { }
    }


    public class Knight : Piece
    {
        public Knight(int state, int x, int y, int c) : base(state, x, y, c) { }
    }


    public class Pawn : Piece
    {
        public Pawn(int state, int x, int y, int c) : base(state, x, y, c) { }
    }
}
