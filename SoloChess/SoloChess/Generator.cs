using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoloChess
{
    internal static class Generator
    {
        static Random rdm = new Random();

        public static void WriteInputs(int n,int diff, string path2)
        {
            // Wrapper function for generating and exporting n instances of difficulty diff

            void WriteToFile(string text, string filename)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                StreamWriter writer = new StreamWriter(filename);
                writer.Write(text);
                writer.Close();
            }

            for (int i = 1; i <= n; i++)
            {
                string filename = path2 + $"\\level{diff.ToString("D2")}" + $"\\{diff.ToString("D2")}{i.ToString("D4")}.txt";
                Instance inst = GenValidInstance(diff);
                WriteToFile(inst.ToString(), filename);
            }
        }


        public static Instance GenValidInstance(int n)
        {
            // Call generate instance until successful
            // Multiple tries are mostly needed for extremely large n
            while(true)
            {
                (bool successful, Instance inst) = GenerateInstance(n);
                if (successful)
                    return inst;
            }
        }

        public static (bool, Instance) GenerateInstance(int n)
        {
            // Initialize
            Piece[,] grid = new Piece[8, 8];
            Piece[] pieces = new Piece[n];

            // Place King on random square
            int startX = rdm.Next(0, 8);
            int startY = rdm.Next(0, 8);
            Square square = new Square(startX, startY);
            Piece piece = new King(square, 2);             // King with 2 captures left
            grid[startX, startY] = piece;
            pieces[0] = piece;

            // To keep track of expandable pieces currently on the board
            List<Piece> expandable_pieces = new List<Piece>();
            expandable_pieces.Add(piece);

            // Perform n-1 expansions
            for (int round = 1; round < n; round++)
            {
                // Generate list of all possible expansions
                List<(Piece, List<(int, int)>)> possible_expansions = GetPossibleExpansions(expandable_pieces, grid);
                if (possible_expansions.Count == 0)
                    return (false, null);
                else
                {
                    // Choose random expansion
                    (Piece piece_to_expand, List<(int, int)> square_positions) = possible_expansions[rdm.Next(0, possible_expansions.Count)];
                    (int X, int Y) = square_positions[rdm.Next(0, square_positions.Count)];
                    Square square_to_expand_to = new Square(X, Y);

                    // Perform expansion
                    Expand(piece_to_expand, square_to_expand_to, expandable_pieces, grid, pieces, round);
                }
            }
            return (true, BuildInstance(pieces));
        }


        public static List<(Piece, List<(int, int)>)> GetPossibleExpansions(List<Piece> pieces, Piece[,] grid)
        {
            List<(Piece, List<(int, int)>)> possible_expansions = new List<(Piece, List<(int, int)>)>();

            // Get expansion options for all expandable pieces
            for (int i = 0; i < pieces.Count; i++)
            {
                Piece p = pieces[i];

                // Get all possible square positions p can move to
                List<(int, int)> options_for_piece = p.GetOptions(grid);

                // Add the piece, including a list of all its options for square positions to move to, to the list of possible expansions
                if (options_for_piece.Count > 0)
                    possible_expansions.Add((p, options_for_piece));
            }

            return possible_expansions;
        }

        public static void Expand(Piece p1, Square new_square, List<Piece> expandable, Piece[,] grid, Piece[] pieces, int r)
        {
            // If reached its maximum number of expansions, mark unexpandable
            p1.CapturesLeft--;
            if (p1.CapturesLeft <= 0)
                expandable.Remove(p1);

            // Create new piece with random type and 2 expansions left
            Piece p2 = ParseState(rdm.Next(2, 7), p1.Square, 2); 
            pieces[r] = p2;
            grid[p2.Square.X, p2.Square.Y] = p2;
            expandable.Add(p2);

            // Move original piece
            p1.Square = new_square;
            grid[new_square.X, new_square.Y] = p1;
        }

        public static Instance BuildInstance(Piece[] pieces)
        {
            // Order pieces by square coordinates to avoid bias in instance
            IOrderedEnumerable<Piece> sorted_pieces = pieces.OrderBy(p => p.Square.X).ThenBy(p => p.Square.Y);

            // Create instance from list of pieces
            Instance instance = new Instance();
            foreach (Piece p in sorted_pieces)
                instance.Add(p.TypeID, p.Square.X, p.Square.Y, 2);
            return instance;
        }

        public static Piece ParseState(int p, Square s, int c)
        {
            switch (p)
            {
                case 1:
                    return new King(s, c);
                case 2:
                    return new Queen(s, c);
                case 3:
                    return new Rook(s, c);
                case 4:
                    return new Bishop(s, c);
                case 5:
                    return new Knight(s, c);
                default:
                    return new Pawn(s, c);
            }
        }
    }
}
