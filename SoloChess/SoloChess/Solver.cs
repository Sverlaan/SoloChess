using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    
    internal class Solver
    {
        Puzzle puzzle;

        public int nBacktracks;

        VisistedSet visited;

        Piece[] places;

        IHeuristic heur_fun;

        public Solver(Puzzle puzzle, string heur)
        {
            this.puzzle = puzzle;
            visited = new VisistedSet(puzzle.pieces);

            places = InitPlaces(puzzle.pieces);

            heur_fun = ParseHeuristic(heur);
        }

        private IHeuristic ParseHeuristic(string heur)
        {
            switch(heur)
            {
                case "Rank":
                    return new Rank();
                case "Attack":
                    return new Attack();
                case "Center":
                    return new Center(this.puzzle.pieces);
                default:
                    return new Randomized();
            }
        }

        public Piece[] InitPlaces(Piece[] pieces)
        {
            Piece[] arr = new Piece[pieces.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                Piece p = pieces[i];
                arr[i] = p;
                p.index_places = i;
            }
            return arr;
        }



        public bool GoalState(List<Piece> pieces)
        {
            return pieces.Count == 1 && pieces[0].State == 1;
        }


        public void OutputPlan(LinkedList<string> plan)
        {
            Console.WriteLine($"SOLUTION FOUND using {heur_fun.ToString()} heuristic after {nBacktracks} BACKTRACKS");
            foreach (string move in plan)
                Console.WriteLine(move);
            Console.WriteLine();
        }


        

        public (LinkedList<string>, int) Solve(Puzzle puzzle)
        {
            visited.Clear();
            this.nBacktracks = 0;

            (bool found, LinkedList<string> sol, int nb) = SolveRecursive(puzzle.pieces.ToList(), new LinkedList<string>(), true);
            if (found)
            {
                visited.Clear();
                return (sol, nb);
            }


            ///////////
            Console.WriteLine("NO SOLUTION FOUND");
            throw new Exception();
        }


        public (bool, LinkedList<string>, int) SolveRecursive(List<Piece> pieces, LinkedList<string> plan, bool forward_check)
        {

            if (GoalState(pieces))
                return (true, plan, nBacktracks);


            foreach (Move move in GeneratePossibleMoves(pieces, forward_check))
            {
                plan.AddLast(move.ToString());

                Piece p = move.from;
                Piece q = move.to;
                Square old = p.Square;

                // Execute Move
                p.CapturesLeft--;
                p.Square.bids.Remove();
                p.Square = q.Square;
                pieces.Remove(q);

                places[q.index_places] = p;
                places[p.index_places] = null;
                int old_index_places_p = p.index_places;
                p.index_places = q.index_places;

                if (!visited.SeenBefore(places))
                {
                    (bool solution_found, LinkedList<string> sol, int nb) = SolveRecursive(pieces, plan, forward_check);
                    if (solution_found)
                        return (true, sol, nb);

                }

                nBacktracks++;  // of alleen na recursieve aanroep?

                places[q.index_places] = q;
                places[old_index_places_p] = p;
                p.index_places = old_index_places_p;

                // Restore
                pieces.Add(q);
                plan.RemoveLast();
                p.Square = old;
                p.CapturesLeft++;
                p.Square.bids.AddBack();
            }
            return (false, null, -9999);
        }



        


        public List<Move> GeneratePossibleMoves(List<Piece> pieces, bool forward_check)
        {
            List<Move> valid_moves = new List<Move>();

            foreach (Piece p in pieces)
            {
                p.out_moves = new List<Move>();
                p.in_moves = new List<Move>();
            }

            Knight possible_knight = new Knight(null, pieces.Count);
            foreach (Piece p in pieces)
            {
                p.possible_knight_attack = 0;
                foreach (Piece q in pieces)
                {

                    if (q.State != 1 && p.Square != q.Square && p.ValidCapture(q))
                    {
                        Move m = new Move(p, q);
                        m.Value = heur_fun.CalcValue(m, pieces);
                        valid_moves.Add(m);
                        p.out_moves.Add(m);
                        q.in_moves.Add(m);
                    }

                    possible_knight.Square = q.Square;
                    if (p.Square != q.Square && possible_knight.ValidCapture(p))
                        p.possible_knight_attack++;
                }
            }


            if (forward_check)
                foreach (Piece p in pieces)
                    if (p.out_moves.Count == 0 && p.in_moves.Count == 0 && p.Square.bids.Empty() && p.possible_knight_attack == 0)
                        return new List<Move>();


            return heur_fun.SortByValue(valid_moves);
        }
    }

}
