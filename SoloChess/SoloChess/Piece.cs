using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SoloChess
{
    public abstract class Piece
    {
        public int State;
        public int X, Y;
        public bool clicked;
        public int nCapturesLeft;
        public abstract Image FigureW { get; }
        public abstract Image FigureB { get; }

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


        public virtual List<(int, int)> GetOptions(Piece[,] grid)
        {
            List<(int, int)> options_for_piece = new List<(int, int)>();
            List<(int, int)> temp_options = CalcList();

            foreach ((int x, int y) in temp_options)
                if (ValidOption(x, y, grid))
                    options_for_piece.Add((x, y));

            return options_for_piece;
        }

        public virtual bool ValidOption(int x, int y, Piece[,] grid)
        {
            if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1) && grid[x, y] == null)
                return true;
            return false;
        }

        public abstract List<(int, int)> CalcList();

    }



    public class King : Piece
    {
        public override Image FigureW { get => Properties.Resources.kingW; }
        public override Image FigureB { get => Properties.Resources.kingB; }

        public King(int state, int x, int y, int c) : base(state, x, y, c) { }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (this.X - 1, this.Y - 1), (this.X - 1, this.Y), (this.X - 1, this.Y + 1), (this.X, this.Y - 1), (this.X, this.Y + 1), (this.X + 1, this.Y - 1), (this.X + 1, this.Y), (this.X + 1, this.Y + 1) };
        }


    }

    public class Knight : Piece
    {
        public override Image FigureW { get => Properties.Resources.knightW; }
        public override Image FigureB { get => Properties.Resources.knightB; }
        public Knight(int state, int x, int y, int c) : base(state, x, y, c) { }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (this.X + 2, this.Y - 1), (this.X + 2, this.Y + 1), (this.X - 2, this.Y - 1), (this.X - 2, this.Y + 1), (this.X - 1, this.Y + 2), (this.X + 1, this.Y + 2), (this.X - 1, this.Y - 2), (this.X + 1, this.Y - 2) };
        }
    }


    public class Pawn : Piece
    {
        public override Image FigureW { get => Properties.Resources.pawnW; }
        public override Image FigureB { get => Properties.Resources.pawnB; }
        public Pawn(int state, int x, int y, int c) : base(state, x, y, c) { }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (this.X - 1, this.Y + 1), (this.X + 1, this.Y + 1) };
        }
    }



    public abstract class SlidingPiece : Piece
    {
        public SlidingPiece(int p, int x, int y, int c) : base(p, x, y, c) { }

        public override List<(int, int)> GetOptions(Piece[,] grid)
        {
            List<(int, int)> options = new List<(int, int)>();
            List<(int, int)> possible_directions = CalcList();

            foreach ((int dx, int dy) in possible_directions)
            {
                int x = this.X + dx;
                int y = this.Y + dy;

                while (ValidOption(x, y, grid))
                {
                    options.Add((x, y));
                    x += dx;
                    y += dy;
                }
            }
            return options;
        }
    }


    public class Queen : SlidingPiece
    {
        public override Image FigureW { get => Properties.Resources.queenW; }
        public override Image FigureB { get => Properties.Resources.queenB; }
        public Queen(int state, int x, int y, int c) : base(state, x, y, c) { }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
        }
    }


    public class Rook : SlidingPiece
    {
        public override Image FigureW { get => Properties.Resources.rookW; }
        public override Image FigureB { get => Properties.Resources.rookB; }
        public Rook(int state, int x, int y, int c) : base(state, x, y, c) { }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (-1, 0), (0, -1), (0, 1), (1, 0) };
        }
    }


    public class Bishop : SlidingPiece
    {
        public override Image FigureW { get => Properties.Resources.bishopW; }
        public override Image FigureB { get => Properties.Resources.bishopB; }
        public Bishop(int state, int x, int y, int c) : base(state, x, y, c) { }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (-1, -1), (-1, 1), (1, -1), (1, 1) };
        }
    }


    
}
