using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SoloChess
{
    internal static class Generator
    {
        static Random rdm = new Random();

        public static void WriteInputs(int n,int diff, string path2)
        {

            for (int i = 1; i <= n; i++)
            {
                string filename = path2 + $"\\level{diff.ToString("D2")}" + $"\\{diff.ToString("D2")}{i.ToString("D4")}.txt";
                Instance inst = GenValidInstance(diff);
                WriteToFile(inst.ToString(), filename);
            }
        }

        public static void WriteToFile(string text, string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            StreamWriter writer = new StreamWriter(filename);
            writer.Write(text);
            writer.Close();
        }

        public static Instance GenValidInstance(int rounds)
        {
            bool found = false;
            while (!found)
            {
                found = true;

                Piece[,] grid = new Piece[8, 8];
                Piece[] pieces = new Piece[rounds];

                int startX = rdm.Next(0, 8);
                int startY = rdm.Next(0, 8);
                Square square = new Square(startX, startY);

                Piece piece = new King(square, 2);  // always exactly one king /////
                grid[startX, startY] = piece;
                pieces[0] = piece;
                List<Piece> nonfixed = new List<Piece>();
                nonfixed.Add(piece);

                for (int r = 1; r < rounds; r++)
                {
                    List<(Piece, List<(int, int)>)> all_options = new List<(Piece, List<(int, int)>)>();

                    for (int i = 0; i < nonfixed.Count; i++)
                    {
                        Piece p = nonfixed[i];
                        List<(int, int)>  options_for_piece = p.GetOptions(grid);

                        if (options_for_piece.Count > 0)
                            all_options.Add((p, options_for_piece));
                    }

                    if (all_options.Count == 0)
                    {
                        found = false;
                        break;
                        //throw new Exception();
                    }

                    (Piece p1, List<(int, int)> options) = all_options[rdm.Next(0, all_options.Count)];
                    (int newX, int newY) = options[rdm.Next(0, options.Count)];
                    Square new_square = new Square(newX, newY);

                    p1.CapturesLeft--;
                    if (p1.CapturesLeft <= 0)
                        nonfixed.Remove(p1);

                    Piece p2 = ParseState(rdm.Next(2, 7), p1.Square, 2);
                    pieces[r] = p2;
                    grid[p2.Square.X, p2.Square.Y] = p2;
                    nonfixed.Add(p2);

                    p1.Square = new_square;
                    grid[newX, newY] = p1;
                }

                if (found)
                {
                    Instance instance = new Instance();
                    IOrderedEnumerable<Piece> sorted_pieces = pieces.OrderBy(p => p.Square.X).ThenBy(p => p.Square.Y);
                    foreach (Piece p in sorted_pieces)
                        instance.Add(p.State, p.Square.X, p.Square.Y, 2);
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

        public static List<(int, int)> GetSlidingOptions(Piece p1, Piece[,] grid, List<(int, int)> possible_directions)
        {
            List<(int, int)> options = new List<(int, int)>();

            foreach ((int dx, int dy) in possible_directions)
            {
                int x = p1.Square.X + dx;
                int y = p1.Square.Y + dy;

                while (x >= 0 && x < 8 && y >= 0 & y < 8 && grid[x, y] == null)
                {
                    options.Add((x, y));
                    x += dx;
                    y += dy;
                }
            }
            return options;
        }

        public static string GenRandomInstance()
        {
            int nPieces = rdm.Next(2, 10);
            int width = 8;
            int height = 8;

            StringBuilder sb = new StringBuilder();

            sb.Append($"{nPieces} {width} {height}");
            for (int i = 0; i < nPieces; i++)
            {
                int piece = rdm.Next(1, 7);
                int xpos = rdm.Next(0, width);
                int ypos = rdm.Next(0, height);

                sb.Append("\n");
                sb.Append($"{piece} {xpos} {ypos}");
            }
            return sb.ToString();
        }
    }
}
