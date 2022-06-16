using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace SoloChess
{
    public class Tester
    {
        static string path = System.Environment.CurrentDirectory;
        static string input_folder = path.Substring(0, path.LastIndexOf("bin")) + "inputs";
        static NumberFormatInfo nfi = new NumberFormatInfo();

        public static void Generate(int from, int to, int n)
        {
            // Generates n puzzles of all difficulty levels between from and to
            for (int i = from; i <= to; i++)
                Generator.WriteInputs(n, i, input_folder);
        }


        public static void GetChar()
        {
            Console.WriteLine($"CALCULATE ALL PUZZLE Ranks");
            nfi.NumberDecimalSeparator = ".";

            StreamWriter sw = new StreamWriter(path.Substring(0, path.LastIndexOf("bin")) + $"results\\characteristicsNEW.csv", true);


            for (int diff = 2; diff <= 14; diff++)
            {
                string dirname = input_folder + $"\\level{diff.ToString("D2")}";
                List<string> fileName = Directory.GetFiles(dirname).ToList();

                for (int i = 0; i < 1000; i++)
                {
                    string file = fileName[i];
                    Instance inst;
                    using (StreamReader sr = new StreamReader(file))
                        inst = new Instance(sr);

                    Puzzle puzzle = new Puzzle(inst);
                    StringBuilder sb = new StringBuilder();
                    foreach (Piece p in puzzle.pieces)
                        if (p.State != 1)
                            sb.Append(p.Rank);

                    sw.WriteLine(Path.GetFileNameWithoutExtension(file) + "," + puzzle.pieces.Length + "," + sb.ToString());
                }
            }

            sw.Close();
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


        

        static (int, double) SolveIt(Instance inst, int n, string heur_fun)
        {
            DateTime dt = DateTime.Now;

            int res = 0;
            for (int i = 0; i < n; i++)
            {
                Puzzle puzzle = new Puzzle(inst);
                Solver solver = new Solver(puzzle, heur_fun);
                (LinkedList<string> sol, int nback) = solver.Solve(puzzle);
                res += nback;
            }
            int backtracks = res / n;
            double time = (DateTime.Now - dt).TotalMilliseconds / n;
            return (backtracks, time);
        }
    }
}
