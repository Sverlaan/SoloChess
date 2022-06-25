using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace SoloChess
{
    public class Tester
    {
        private static string path = System.Environment.CurrentDirectory;
        private static string input_folder = path.Substring(0, path.LastIndexOf("bin")) + "inputs";
        private static NumberFormatInfo nfi = new NumberFormatInfo();

        public static void Generate(int from, int to, int n)
        {
            // Generates n puzzles of all difficulty levels between from and to
            for (int i = from; i <= to; i++)
                Generator.WriteInputs(n, i, input_folder);
        }

        public static void GetPieceTypeDistribution()
        {
            Console.WriteLine($"GET PIECE TYPE DISTRIBUTION OF ALL PUZZLES");
            nfi.NumberDecimalSeparator = ".";
            StreamWriter sw = new StreamWriter(path.Substring(0, path.LastIndexOf("bin")) + $"results\\characteristicsNEW.csv", true);

            // For all difficulties
            for (int diff = 2; diff <= 14; diff++)
            {
                string dirname = input_folder + $"\\level{diff.ToString("D2")}";
                List<string> fileName = Directory.GetFiles(dirname).ToList();

                // For all puzzle instances
                for (int i = 0; i < 1000; i++)
                {
                    // Get puzzle instance from input file
                    string file = fileName[i];
                    Instance inst;
                    using (StreamReader sr = new StreamReader(file))
                        inst = new Instance(sr);

                    // Get all piece types on the board, with the exception of the King
                    Puzzle puzzle = new Puzzle(inst);
                    StringBuilder sb = new StringBuilder();
                    foreach (Piece p in puzzle.pieces)
                        if (p.State != 1)
                            sb.Append(p.Rank);

                    // Write to output file
                    sw.WriteLine(Path.GetFileNameWithoutExtension(file) + "," + puzzle.pieces.Length + "," + sb.ToString());
                }
            }
            sw.Close();
        }


        public static void Test(string heur, int tries, int diff, int from, int to)
        {
            Console.WriteLine($"TESTING LEVEL {diff} from {from} to {to} using {heur} HEURISTIC over {tries} TRIES");
            nfi.NumberDecimalSeparator = ".";

            string dirname = input_folder + $"\\level{diff.ToString("D2")}";
            List<string> fileName = Directory.GetFiles(dirname).ToList();

            int length = to - from + 1;
            int progress = 0;

            for (int i = from - 1; i < to; i++)
            {
                // Read puzzle instance from input file
                string file = fileName[i];
                Instance inst;
                using (StreamReader sr = new StreamReader(file))
                    inst = new Instance(sr);

                // Solve puzzle instance {tries} times using {heur} heuristic
                (int nBacktracks, double time) = SolveIt(inst, tries, heur);

                // Write testing results to output file
                // Report puzzle ID, number of backtracks, time
                StreamWriter sw = new StreamWriter(path.Substring(0, path.LastIndexOf("bin")) + $"results\\{heur}_{diff}_{from}_{to}.csv", true);
                sw.WriteLine(Path.GetFileNameWithoutExtension(file) + "," + nBacktracks + "," + time.ToString(nfi));
                sw.Close();

                progress++;
                Console.WriteLine((progress / (float)length) * 100 + " %");
            }
        }

        static (int, double) SolveIt(Instance inst, int n, string heur)
        {
            // Solve given instance n times using heur
            DateTime dt = DateTime.Now;
            int res = 0;
            for (int i = 0; i < n; i++)
            {
                Puzzle puzzle = new Puzzle(inst);
                Solver solver = new Solver(puzzle, heur);
                (LinkedList<string> sol, int nback) = solver.Solve(puzzle);
                res += nback;
            }

            // Report average number of backtracks and average time, over n tries
            int backtracks = res / n;
            double time = (DateTime.Now - dt).TotalMilliseconds / n;
            return (backtracks, time);
        }
















        public static void TestRandom(int diff, int from, int to)
        {
            Console.WriteLine($"TESTING LEVEL {diff} from {from} to {to}");
            nfi.NumberDecimalSeparator = ".";

            string dirname = input_folder + $"\\level{diff.ToString("D2")}";
            List<string> fileName = Directory.GetFiles(dirname).ToList();

            DateTime dt = DateTime.Now;
            float length = to - from + 1;
            int progress = 1;

            for (int i = from - 1; i < to; i++)
            {
                string file = fileName[i];
                Instance inst;
                using (StreamReader sr = new StreamReader(file))
                    inst = new Instance(sr);

                // Solve puzzle instance 100 times using
                (int random_bt, double time) = SolveIt(inst, 100, "Random");

                StreamWriter sw = new StreamWriter(path.Substring(0, path.LastIndexOf("bin")) + $"results\\randomNEW{diff}_{from}_{to}.csv", true);
                sw.WriteLine(Path.GetFileNameWithoutExtension(file) + "," + random_bt + "," + time.ToString(nfi));
                sw.Close();

                Console.WriteLine((progress / length) * 100 + " %");
                progress++;
            }
            Console.WriteLine("TOTAL TIME (s): \t " + (DateTime.Now - dt).Seconds);
        }

        
        public static void TestHeur(int diff, int from, int to, string heurist, string f)
        {
            Console.WriteLine($"TESTING LEVEL {diff} from {from} to {to}");
            nfi.NumberDecimalSeparator = ".";

            StreamWriter sw = new StreamWriter(path.Substring(0, path.LastIndexOf("bin")) + $"results\\{f}", true); // filename

            string dirname = input_folder + $"\\level{diff.ToString("D2")}";
            List<string> fileName = Directory.GetFiles(dirname).ToList();

            for (int i = 0; i < fileName.Count; i++)
            {
                string file = fileName[i];
                Instance inst;
                using (StreamReader sr = new StreamReader(file))
                    inst = new Instance(sr);

                (int bt, double time) = SolveIt(inst, 1, heurist); // 1, heur

                
                sw.WriteLine(Path.GetFileNameWithoutExtension(file) + "," + bt + "," + time.ToString(nfi));
            }

            sw.Close();
        }


        

    }
}
