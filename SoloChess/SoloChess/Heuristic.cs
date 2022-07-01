using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SoloChess
{
    public interface IHeuristic
    {
        float CalcValue(Move move, List<Piece> pieces);  // Function for calculating the heuristic value of a move
        List<Move> SortByValue(List<Move> moves);        // Function for sorting the moves by heuristic value
    }

    

    public class Rank : IHeuristic
    {
        public float CalcValue(Move move, List<Piece> pieces)
        {
            if (move.from.TypeID == 1)
                return 99 + move.to.Rank * move.to.CapturesLeft;  // Rank of King is supposed to be 99 in the context of this heuristic

            // Calculate difference in total rank, before and after move
            return move.from.Rank + move.to.Rank * move.to.CapturesLeft;
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            return moves.OrderBy(x => x.Value).ThenBy(x => Guid.NewGuid()).ToList();
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

            // Get number of possible attacks for p after the potential move
            foreach(Piece piece in pieces)
                if (piece != p && piece != q)
                    if (p.ValidCapture(piece))
                        p_out_aftermove++;

            p.Square = p_old_square;

            // Difference in total number of possible moves, before and after move
            return p_out_aftermove - p.out_moves.Count - q.out_moves.Count - p.in_moves.Count;
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            return moves.OrderByDescending(x => x.Value).ThenBy(x => Guid.NewGuid()).ToList();
        }

        public override string ToString(){ return "ATTACK"; }

    }



    public class Center : IHeuristic
    {
        Square center;
        public Center(List<Piece> pieces)
        {
            center = InitCenter(pieces);
        }

        public float CalcValue(Move move, List<Piece> pieces)
        {
            return EuclideanDist(move.from.Square, move.to.Square) + EuclideanDist(move.from.Square, center);
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            return moves.OrderByDescending(x => x.Value).ThenBy(x => Guid.NewGuid()).ToList();
        }

        public float EuclideanDist(Square p, Square q)
        {
            return (p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y); 
        }

        public Square InitCenter(List<Piece> pieces)
        {
            // Initialize center to be the King's square
            foreach (Piece p in pieces)
                if (p.TypeID == 1)
                    return p.Square;

            return null;
        }

        public override string ToString(){ return "CENTER"; }

    }



    public class Randomized : IHeuristic
    {
        static RNGCryptoServiceProvider provider;

        public Randomized()
        {
            provider = new RNGCryptoServiceProvider();
        }

        public float CalcValue(Move move, List<Piece> pieces)
        {
            return 0;
        }

        public List<Move> SortByValue(List<Move> moves)
        {
            // Shuffle the moves randomly
            Shuffle(moves);
            return moves;
        }

        // Containing code from user Uwe Keim on StackOverflow.com
        // see: https://stackoverflow.com/questions/273313/randomize-a-listt
        public static void Shuffle(IList<Move> list)
        {
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

}
