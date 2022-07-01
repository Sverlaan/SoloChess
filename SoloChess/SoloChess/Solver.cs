using System;
using System.Collections.Generic;
using System.Linq;

namespace SoloChess
{
    public class Solver
    {
        private Puzzle puzzle;        
        private int nBacktracks;      // Number of backtracks so far
        private VisistedSet visited;  // To keep track of already visited states
        private Piece[] places;       // Used for efficient hashing of states to a string
        private IHeuristic heur_fun;  // Heuristic function used in solving

        public Solver(Puzzle puzzle, string heur)
        {
            this.puzzle = puzzle;
            visited = new VisistedSet();
            places = InitPlaces();
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

        public Piece[] InitPlaces()
        {
            // Take over current ordering of pieces 
            Piece[] arr = new Piece[this.puzzle.pieces.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                Piece p = this.puzzle.pieces[i];
                arr[i] = p;
                p.index_places = i;
            }
            return arr;
        }


        public bool GoalState(List<Piece> pieces)
        {
            // Check whether the last piece is a King
            return pieces.Count == 1 && pieces[0].TypeID == 1;
        }

        public void OutputPlan(LinkedList<string> plan)
        {
            // Output the solution found
            Console.WriteLine($"SOLUTION FOUND using {heur_fun.ToString()} heuristic after {nBacktracks} BACKTRACKS");
            foreach (string move in plan)
                Console.WriteLine(move);
            Console.WriteLine();
        }


        public (LinkedList<string>, int) Solve(Puzzle puzzle)
        {
            // Reset
            visited.Clear();
            this.nBacktracks = 0;

            // Solve
            (bool found, LinkedList<string> plan, int nb) = SolveRecursive(new LinkedList<string>());
            if (found)
                return (plan, nb);

            // Should not occur for solvable levels
            Console.WriteLine("NO SOLUTION FOUND");
            throw new Exception();
        }


        public (bool, LinkedList<string>, int) SolveRecursive(LinkedList<string> plan)
        {
            // Solution found
            if (GoalState(this.puzzle.pieces))
                return (true, plan, nBacktracks);

            foreach (Move move in GeneratePossibleMoves(this.puzzle.pieces))
            {
                plan.AddLast(move.ToString());

                Piece p = move.from;
                Piece q = move.to;

                // Store old info that will be overwritten by new move
                Square old_square_p = p.Square;
                int old_index_places_p = p.index_places;

                // Perform move
                ExecuteMove(p, q);
                
                if (!visited.SeenBefore(places))  // Only recurse if resulting state has not been explored before
                {
                    (bool solution_found, LinkedList<string> sol, int nb) = SolveRecursive(plan);

                    if (solution_found)
                        return (true, sol, nb);
                }

                // Restore state and backtrack
                UndoMove(p, q, old_square_p, old_index_places_p);
                plan.RemoveLast();
                nBacktracks++;
            }
            
            // No solution found in this branch
            return (false, null, -9999);
        }

        public void ExecuteMove(Piece p, Piece q)
        {
            // Execute Move
            p.CapturesLeft--;
            p.Square.bids.Remove();
            p.Square = q.Square;
            this.puzzle.pieces.Remove(q);

            // Update places used in string hasing
            places[q.index_places] = p;
            places[p.index_places] = null;
            p.index_places = q.index_places;
        }

        public void UndoMove(Piece p, Piece q, Square old_square_p, int old_index_places_p)
        {
            // Reset places used in string hashing
            places[q.index_places] = q;
            places[old_index_places_p] = p;
            p.index_places = old_index_places_p;

            // Undo move
            this.puzzle.pieces.Add(q);
            p.Square = old_square_p;
            p.CapturesLeft++;
            p.Square.bids.AddBack();
        }


        public List<Move> GeneratePossibleMoves(List<Piece> pieces)
        {
            // Create empty list of valid moves
            List<Move> valid_moves = new List<Move>();

            foreach (Piece p in pieces)
            {
                // Check if King has become fixed (state space reduction)
                // Return zero valid moves, which causes immediate backtracking
                if (p.TypeID == 1 && p.CapturesLeft == 0)
                    return new List<Move>();

                // Reset in and out edges
                p.out_moves = new List<Move>();
                p.in_moves = new List<Move>();
            }

            // Potential knight
            Knight possible_knight = new Knight(null, pieces.Count);

            foreach (Piece p in pieces)
            {
                p.possible_knight_attack = 0;
                foreach (Piece q in pieces)
                {
                    // Check if capture is valid
                    // King cannot be captured (state space reduction)
                    if (q.TypeID != 1 && p.Square != q.Square && p.ValidCapture(q))
                    {
                        // Create move and add to list of possibilities
                        Move m = new Move(p, q);
                        m.Value = heur_fun.CalcValue(m, pieces);
                        valid_moves.Add(m);
                        p.out_moves.Add(m);
                        q.in_moves.Add(m);
                    }

                    // Check whether piece p is in a position to be captured by a potential knight on any of the currently occupied squares
                    possible_knight.Square = q.Square;
                    if (p.Square != q.Square && possible_knight.ValidCapture(p))
                        p.possible_knight_attack++;
                }
            }

            // Return empty list of moves when one of the pieces is completely secluded
            // and not in a position to ever capture or become captured (state space reduction)
            foreach (Piece p in pieces)
                if (p.out_moves.Count == 0 && p.in_moves.Count == 0 && p.Square.bids.Empty() && p.possible_knight_attack == 0)
                    return new List<Move>();

            // Return list of possible moves sorted by heuristic value
            return heur_fun.SortByValue(valid_moves);
        }
    }

}
