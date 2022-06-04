using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SoloChess
{
    
    internal class Solver
    {
        List<Move> moves;
        Puzzle puzzle;
        Random rdm;
        public int nBacktracks = 0;
        int iter = 0;
        Square center;

        public Solver(Puzzle puzzle)
        {
            rdm = new Random();
            this.puzzle = puzzle;
            moves = GeneratePossibleMoves(puzzle.pieces.ToList());

            center = InitCenter();
        }

        public Square InitCenter()
        {
            foreach (Piece p in puzzle.pieces)
                if (p.State == 1)
                    return p.Square;

            return null;
        }

        public bool GoalState(List<Piece> pieces)
        {
            return pieces.Count == 1 && pieces[0].State == 1;
        }

        public void OutputPlan(Stack<string> plan)
        {
            if (plan.Count == 0)
                return;
            string move = plan.Pop();
            OutputPlan(plan);
            Console.WriteLine(move);
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

        public float Distance(Square p, Square q)
        {
            return (p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y);
        }

        public (bool, int) Solve(List<Piece> pieces, Stack<string> plan, bool heuristic)
        {

            if (GoalState(pieces))
            {
                //Console.WriteLine($"Solution found after {nBacktracks} backtracks");
                //OutputPlan(plan);
                //Console.Write(nBacktracks + ",");
                return (true, nBacktracks);
            }

            List<Move> possible_moves = GeneratePossibleMoves(pieces);
            
            

            if (heuristic)
            {
                //possible_moves = possible_moves.OrderBy(m => m.from.Rank + m.to.Rank * m.to.CapturesLeft).ToList();
                //possible_moves = possible_moves.OrderBy(m => m.from.in_count + m.from.out_count + m.to.out_count).ToList();
                possible_moves = possible_moves.OrderByDescending(m => Distance(m.from.Square, m.to.Square) + Distance(m.from.Square, center)).ToList();
            }
            else
                Shuffle(possible_moves);

            foreach (Move move in possible_moves)
            {
                plan.Push(move.ToString());
                //Console.WriteLine("Try: " + move.ToString());

                Piece p = move.from;
                Piece q = move.to;
                Square old = p.Square;

                // Execute Move
                p.CapturesLeft--;
                p.Square.bids.Remove();
                p.Square = q.Square;
                pieces.Remove(q);

                (bool solution_found, int nb) = Solve(pieces, plan, heuristic);
                if (solution_found)
                    return (true, nb);

                //Console.WriteLine("Backtrack");

                iter++;
                nBacktracks++;
                /*
                if (iter == 10000)
                {
                    Console.WriteLine(nBacktracks);
                    iter = 0;
                }
                */

                // Restore
                pieces.Add(q);
                plan.Pop();
                p.Square = old;
                p.CapturesLeft++;
                p.Square.bids.AddBack();
            }
            return (false, -9999);
        }

        public class Move
        {
            public Piece from, to;
            private string[] states = new string[] { "EMPTYSTATE", "K", "Q", "R", "B", "N", "P" };

            public Move(Piece p, Piece q)
            {
                this.from = p;
                this.to = q;
            }

            public override string ToString()
            {
                return states[from.State] + from.Square.X.ToString() + from.Square.Y.ToString()
                    + "x" + states[to.State] + to.Square.X.ToString() + to.Square.Y.ToString();
            }
        }

        public List<Move> GeneratePossibleMoves(List<Piece> pieces)
        {
            List<Move> valid_moves = new List<Move>();

            foreach(Piece p in pieces)
            {
                p.in_count = 0;
                p.out_count = 0;
            }

            foreach(Piece p in pieces)
            {
                
                foreach(Piece q in pieces)
                {

                    if (q.State != 1 && p.Square != q.Square && p.ValidCapture(q))
                    {
                        valid_moves.Add(new Move(p, q));
                        p.out_count++;
                        q.in_count++;
                    }
                }
            }

            return valid_moves;
        }
    }

}
