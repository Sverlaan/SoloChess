using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    public class MovesArray
    {
        public List<Move> arr;
        public Random rdm;
        public bool IsEmpty { get => arr.Count == 0; }

        public MovesArray(Piece[] pieces)
        {
            arr = new List<Move>();
            rdm = new Random();
            Initialize(pieces);
        }

        public void Initialize(Piece[] pieces)
        {
            foreach(Piece p in pieces)
                foreach (Piece q in pieces)
                    if (q.State != 1 && p.Square != q.Square && p.ValidCapture(q))
                    {
                        Move new_move = new Move(p, q);
                        Add(new_move);
                        p.out_moves.Add(new_move);
                        q.in_moves.Add(new_move);
                    }
        }

        public Move GetRandom()
        {
            Move move = arr[rdm.Next(0, arr.Count)];
            Remove(move);
            return move;
        }

        public void Add(Move new_move)
        {
            if (new_move.arrLoc < 0)
            {
                arr.Add(new_move);
                new_move.arrLoc = arr.Count - 1;
            }
        }

        public void Remove(Move move)
        {
            if (move.arrLoc >= 0)
            {
                Move last = arr[arr.Count - 1];
                Move moveB = move;
                arr[move.arrLoc] = last;
                last.arrLoc = move.arrLoc;
                arr.RemoveAt(arr.Count - 1);
                move.arrLoc = -1;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Move m in arr)
                sb.Append(m.ToString() + " ");
            return sb.ToString();
        }
    }
}
