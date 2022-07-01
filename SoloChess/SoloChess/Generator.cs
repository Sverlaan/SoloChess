using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
            // Try generating until a valid instance has been found
            // Mostly needed for creating instances with a lot of pieces
            bool found = false;
            while (!found)
            {
                found = true;

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
                List<Piece> nonfixed = new List<Piece>();     
                nonfixed.Add(piece);

                // Perform n-1 expansions
                for (int r = 1; r < n; r++)
                {
                    // Keep track of all possible expansions in a list
                    // Each element is a tuple, consisting of a Piece and a list with all its potential expansions
                    List<(Piece, List<(int, int)>)> all_options = new List<(Piece, List<(int, int)>)>();

                    // Get expansion options for all expandable pieces
                    for (int i = 0; i < nonfixed.Count; i++)
                    {
                        Piece p = nonfixed[i];
                        List<(int, int)>  options_for_piece = p.GetOptions(grid);

                        if (options_for_piece.Count > 0)
                            all_options.Add((p, options_for_piece));
                    }

                    if (all_options.Count == 0)
                    {
                        // No expansions possible
                        found = false;
                        break;
                    }

                    // Choose expansion randomly
                    (Piece p1, List<(int, int)> options) = all_options[rdm.Next(0, all_options.Count)];
                    (int newX, int newY) = options[rdm.Next(0, options.Count)];
                    Square new_square = new Square(newX, newY);

                    // Perform expansion
                    p1.CapturesLeft--;
                    if (p1.CapturesLeft <= 0)
                        nonfixed.Remove(p1);

                    Piece p2 = ParseState(rdm.Next(2, 7), p1.Square, 2);  // Generate new piece of random type and 2 captures left
                    pieces[r] = p2;
                    grid[p2.Square.X, p2.Square.Y] = p2;
                    nonfixed.Add(p2);

                    p1.Square = new_square;
                    grid[newX, newY] = p1;
                }

                if (found)
                {
                    // Successsful instance generation accomplished, output instance
                    Instance instance = new Instance();
                    IOrderedEnumerable<Piece> sorted_pieces = pieces.OrderBy(p => p.Square.X).ThenBy(p => p.Square.Y);  // Order pieces by square coordinates to avoid bias in instance
                    foreach (Piece p in sorted_pieces)
                        instance.Add(p.TypeID, p.Square.X, p.Square.Y, 2);
                    return instance;
                }
            }

            return null;
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
