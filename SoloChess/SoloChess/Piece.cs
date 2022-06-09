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
        public abstract int State { get;}

        public List<Move> in_moves;
        public List<Move> out_moves;

        public int possible_knight_attack;
        public int index_places;

        public abstract int Rank { get; }
        public Square Square {get; set;}
        public int CapturesLeft { get; set; }
        public abstract Image FigureW { get; }
        public abstract Image FigureB { get; }


        public Piece(Square s, int c)
        {
            this.Square = s;
            CapturesLeft = c;
            in_moves = new List<Move>();
            out_moves = new List<Move>();

            possible_knight_attack = 0;
        }


        




        public abstract bool ValidCapture(Piece p);
        public abstract List<(int, int)> CalcList();

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
    }



    public class King : Piece
    {
        public override int Rank { get => 9999; }
        public override int State { get => 1; }

        public King(Square s, int c) : base(s, c) { }

        public override bool ValidCapture(Piece p)
        {
            if (this == p || this.CapturesLeft <= 0)
                return false;

            return Math.Abs(this.Square.X - p.Square.X) <= 1 && Math.Abs(this.Square.Y - p.Square.Y) <= 1;
        }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (this.Square.X - 1, this.Square.Y - 1), (this.Square.X - 1, this.Square.Y), (this.Square.X - 1, this.Square.Y + 1), (this.Square.X, this.Square.Y - 1), (this.Square.X, this.Square.Y + 1), (this.Square.X + 1, this.Square.Y - 1), (this.Square.X + 1, this.Square.Y), (this.Square.X + 1, this.Square.Y + 1) };
        }


        public override Image FigureW { get => Properties.Resources.kingW; }
        public override Image FigureB { get => Properties.Resources.kingB; }
    }

    public class Knight : Piece
    {
        public override int Rank { get => 3; }
        public override int State { get => 5; }
        public Knight(Square s, int c) : base(s, c) { }

        public override bool ValidCapture(Piece p)
        {
            if (this == p || this.CapturesLeft <= 0)
                return false;

            Square s1 = this.Square;
            Square s2 = p.Square;

            if (Math.Abs(s2.X - s1.X) == 1 && Math.Abs(s2.Y - s1.Y) == 2)
                return true;
            else if (Math.Abs(s2.X - s1.X) == 2 && Math.Abs(s2.Y - s1.Y) == 1)
                return true;

            /*
            if (s2.Y == s1.Y - 2 && (s2.X == s1.X - 1 || s2.X == s1.X + 1))
                return true;
            else if (s2.Y == s1.Y + 2 && (s2.X == s1.X - 1 || s2.X == s1.X + 1))
                return true;
            else if (s2.X == s1.X - 2 && (s2.Y == s1.Y - 1 || s2.Y == s1.Y + 1))
                return true;
            else if (s2.X == s1.X + 2 && (s2.Y == s1.Y - 1 || s2.Y == s1.Y + 1))
                return true;
            */
            return false;
        }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (this.Square.X + 2, this.Square.Y - 1), (this.Square.X + 2, this.Square.Y + 1), (this.Square.X - 2, this.Square.Y - 1), (this.Square.X - 2, this.Square.Y + 1), (this.Square.X - 1, this.Square.Y + 2), (this.Square.X + 1, this.Square.Y + 2), (this.Square.X - 1, this.Square.Y - 2), (this.Square.X + 1, this.Square.Y - 2) };
        }

        public override Image FigureW { get => Properties.Resources.knightW; }
        public override Image FigureB { get => Properties.Resources.knightB; }
    }


    public class Pawn : Piece
    {
        public override int Rank { get => 1; }
        public override int State { get => 6; }
        public Pawn(Square s, int c) : base(s, c) { }

        public override bool ValidCapture(Piece p)
        {
            if (this == p || this.CapturesLeft <= 0)
                return false;

            Square s1 = this.Square;
            Square s2 = p.Square;

            if (s2.X == s1.X - 1 && s2.Y == s1.Y - 1)
                return true;
            else if (s2.X == s1.X + 1 && s2.Y == s1.Y - 1)
                return true;

            return false;
        }


        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (this.Square.X - 1, this.Square.Y + 1), (this.Square.X + 1, this.Square.Y + 1) };
        }

        public override Image FigureW { get => Properties.Resources.pawnW; }
        public override Image FigureB { get => Properties.Resources.pawnB; }
    }



    public abstract class SlidingPiece : Piece
    {
        public SlidingPiece(Square s, int c) : base(s, c) { }

        public override List<(int, int)> GetOptions(Piece[,] grid)
        {
            List<(int, int)> options = new List<(int, int)>();
            List<(int, int)> possible_directions = CalcList();

            foreach ((int dx, int dy) in possible_directions)
            {
                int x = this.Square.X + dx;
                int y = this.Square.Y + dy;

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
        public override int Rank { get => 9; }
        public override int State { get => 2; }
        public Queen(Square s, int c) : base(s, c) { }

        public override bool ValidCapture(Piece p)
        {
            if (this == p || this.CapturesLeft <= 0)
                return false;

            Square s1 = this.Square;
            Square s2 = p.Square;

            return s1.AlignVertical(s2) || s1.AlignHorizontal(s2) || s1.AlignDiagonal1(s2) || s1.AlignDiagonal2(s2);
        }


        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
        }

        public override Image FigureW { get => Properties.Resources.queenW; }
        public override Image FigureB { get => Properties.Resources.queenB; }
    }


    public class Rook : SlidingPiece
    {
        public override int Rank { get => 5; }
        public override int State { get => 3; }
        public Rook(Square s, int c) : base(s, c) { }

        public override bool ValidCapture(Piece p)
        {
            if (this == p || this.CapturesLeft <= 0)
                return false;

            Square s1 = this.Square;
            Square s2 = p.Square;

            return s1.AlignVertical(s2) || s1.AlignHorizontal(s2);
        }

        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (-1, 0), (0, -1), (0, 1), (1, 0) };
        }

        public override Image FigureW { get => Properties.Resources.rookW; }
        public override Image FigureB { get => Properties.Resources.rookB; }
    }


    public class Bishop : SlidingPiece
    {
        public override int Rank { get => 5; }
        public override int State { get => 4; }
        public Bishop(Square s, int c) : base(s, c) { }

        public override bool ValidCapture(Piece p)
        {
            if (this == p || this.CapturesLeft <= 0)
                return false;

            Square s1 = this.Square;
            Square s2 = p.Square;

            return s1.AlignDiagonal1(s2) || s1.AlignDiagonal2(s2);
        }


        public override List<(int, int)> CalcList()
        {
            return new List<(int, int)> { (-1, -1), (-1, 1), (1, -1), (1, 1) };
        }

        public override Image FigureW { get => Properties.Resources.bishopW; }
        public override Image FigureB { get => Properties.Resources.bishopB; }
    }

}
