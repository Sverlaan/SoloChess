using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SoloChess
{
    public interface IHeuristic
    {
        float CalcValue(Move move, List<Piece> pieces);
        List<Move> SortByValue(List<Move> moves);
    }

    

    public class Rank : IHeuristic
    {
        public float CalcValue(Move move, List<Piece> pieces)
        {
            return move.from.Rank + move.to.Rank * move.to.CapturesLeft;
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            return moves.OrderBy(m => m.Value).ToList();
        }

        public override string ToString(){ return "RANK"; }
    }

    public class Attack : IHeuristic
    {
        public float CalcValue(Move move, List<Piece> pieces)
        {
            int p_out_aftermove = 0;

            Piece p = move.from;
            Piece q = move.to;

            Square p_old_square = p.Square;
            p.Square = q.Square;

            foreach(Piece piece in pieces)
                if (piece != p && piece != q)
                    if (p.ValidCapture(piece))
                        p_out_aftermove++;

            p.Square = p_old_square;

            return p_out_aftermove - p.out_moves.Count - q.out_moves.Count - p.in_moves.Count;
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            return moves.OrderByDescending(m => m.Value).ToList();
        }


        public override string ToString(){ return "ATTACK"; }
    }



    public class Center : IHeuristic
    {
        Square center;
        public Center(Piece[] pieces)
        {
            center = InitCenter(pieces);
        }

        public float CalcValue(Move move, List<Piece> pieces)
        {
            return EuclideanDist(move.from.Square, move.to.Square) + EuclideanDist(move.from.Square, center);
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            return moves.OrderByDescending(m => m.Value).ToList();
        }

        public float EuclideanDist(Square p, Square q)
        {
            return (p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y);
        }

        public Square InitCenter(Piece[] pieces)
        {
            foreach (Piece p in pieces)
                if (p.State == 1)
                    return p.Square;

            return null;
        }

        public override string ToString(){ return "CENTER"; }
    }



    public class Randomized : IHeuristic
    {
        public float CalcValue(Move move, List<Piece> pieces)
        {
            return 0;
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            Shuffle(moves);
            return moves;
        }

        public static void Shuffle(IList<Move> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                Move value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public override string ToString(){ return "RANDOM"; }
    }




    public class New : IHeuristic
    {
        IHeuristic heur1, heur2;

        public New(Piece[] pieces)
        {
            heur1 = new Rank();
            heur2 = new Center(pieces);
        }

        public float CalcValue(Move move, List<Piece> pieces)
        {
            return heur1.CalcValue(move, pieces) / heur2.CalcValue(move, pieces);
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            return moves.OrderBy(m => m.Value).ToList();
        }

        public override string ToString()
        {
            return "COMBINED";
        }
    }
}
