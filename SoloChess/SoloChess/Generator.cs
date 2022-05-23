using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SoloChess
{
    internal static class Generator
    {
        static Random rdm = new Random();

        public static void WriteInputs(int n, int difficulty)
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path.Substring(0, path.LastIndexOf("bin")) + "inputs";

            for (int i = 0; i < n; i++)
            {
                string filename = path2 + $"\\input{i}.txt";
                WriteToFile(GenValidInstance(difficulty).ToString(), filename);
            }
        }

        public static void WriteToFile(string text, string filename)
        {
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


                Console.WriteLine();
                Piece[,] grid = new Piece[8, 8];
                Piece[] pieces = new Piece[rounds];


                int startX = rdm.Next(0, 8);
                int startY = rdm.Next(0, 8);

                Piece piece = new Piece(1, startX, startY, 2);  // always exactly one king /////
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

                        // Find new place for p1 based on type rules
                        List<(int, int)> options_for_piece = new List<(int, int)>();
                        List<(int, int)> temp_options;
                        switch (p.State)
                        {
                            case 1: // king
                                temp_options = new List<(int, int)> { (p.X - 1, p.Y - 1), (p.X - 1, p.Y), (p.X - 1, p.Y + 1), (p.X, p.Y - 1), (p.X, p.Y + 1), (p.X + 1, p.Y - 1), (p.X + 1, p.Y), (p.X + 1, p.Y + 1) };
                                foreach ((int x, int y) in temp_options)
                                    if (x >= 0 && x < 8 && y >= 0 && y < 8 && grid[x, y] == null)
                                        options_for_piece.Add((x, y));
                                break;
                            case 2: // Queen
                                options_for_piece = GetSlidingOptions(p, grid, new List<(int, int)> { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) });
                                break;
                            case 3: // rook
                                options_for_piece = GetSlidingOptions(p, grid, new List<(int, int)> { (-1, 0), (0, -1), (0, 1), (1, 0) });
                                break;
                            case 4: // bishop
                                options_for_piece = GetSlidingOptions(p, grid, new List<(int, int)> { (-1, -1), (-1, 1), (1, -1), (1, 1) });
                                break;
                            case 5: // knight
                                temp_options = new List<(int, int)> { (p.X + 2, p.Y - 1), (p.X + 2, p.Y + 1), (p.X - 2, p.Y - 1), (p.X - 2, p.Y + 1), (p.X - 1, p.Y + 2), (p.X + 1, p.Y + 2), (p.X - 1, p.Y - 2), (p.X + 1, p.Y - 2) };
                                foreach ((int x, int y) in temp_options)
                                    if (x >= 0 && x < 8 && y >= 0 && y < 8 && grid[x, y] == null)
                                        options_for_piece.Add((x, y));
                                break;
                            case 6: // pawn
                                temp_options = new List<(int, int)> { (p.X - 1, p.Y + 1), (p.X + 1, p.Y + 1) };
                                foreach ((int x, int y) in temp_options)
                                    if (x >= 0 && x < 8 && y >= 0 && y < 8 && grid[x, y] == null)
                                        options_for_piece.Add((x, y));
                                break;
                        }

                        if (options_for_piece.Count > 0)
                            all_options.Add((p, options_for_piece));
                    }

                    // handle !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    if (all_options.Count == 0)
                    {
                        found = false;
                        break;
                        //throw new Exception();
                    }

                    (Piece p1, List<(int, int)> options) = all_options[rdm.Next(0, all_options.Count)];
                    (int newX, int newY) = options[rdm.Next(0, options.Count)];
                    p1.nCaptures--; ////

                    if (p1.nCaptures <= 0) ////
                    {
                        nonfixed.Remove(p1);
                    }

                    Piece p2 = new Piece(rdm.Next(2, 7), p1.X, p1.Y, 2); ////
                    pieces[r] = p2;
                    grid[p2.X, p2.Y] = p2;
                    nonfixed.Add(p2);

                    p1.X = newX;
                    p1.Y = newY;
                    grid[newX, newY] = p1;
                }

                if (found)
                {
                    Instance instance = new Instance();
                    IOrderedEnumerable<Piece> sorted_pieces = pieces.OrderBy(p => p.X).ThenBy(p => p.Y);
                    foreach (Piece p in sorted_pieces)
                        instance.Add(p.State, p.X, p.Y, 2);
                    return instance;
                }
            }

            return null;
        }

        public static List<(int, int)> GetSlidingOptions(Piece p1, Piece[,] grid, List<(int, int)> possible_directions)
        {
            List<(int, int)> options = new List<(int, int)>();

            foreach ((int dx, int dy) in possible_directions)
            {
                int x = p1.X + dx;
                int y = p1.Y + dy;

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
