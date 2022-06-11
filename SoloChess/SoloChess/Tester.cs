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



        public static void TestRandom(int diff, int from, int to)
        {
            Console.WriteLine($"TESTING LEVEL {diff} from {from} to {to}");
            nfi.NumberDecimalSeparator = ".";

            string dirname = input_folder + $"\\level{diff.ToString("D2")}";
            List<string> fileName = Directory.GetFiles(dirname).ToList();

            //StreamWriter sw = new StreamWriter(path.Substring(0, path.LastIndexOf("bin")) + $"results\\random{diff}_{from}_{to}.csv", true);
            StreamWriter sw = new StreamWriter(path.Substring(0, path.LastIndexOf("bin")) + $"results\\random13-14.csv", true);

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

                sw.WriteLine(Path.GetFileNameWithoutExtension(file) + "," + random_bt + "," + time.ToString(nfi));

                Console.WriteLine((progress / length) * 100 + " %");
                progress++;
            }
            Console.WriteLine("TOTAL TIME (s): \t " + (DateTime.Now - dt).Seconds);
            sw.Close();
        }





        public static void TestHeur(int diff, int from, int to)
        {
            Console.WriteLine($"TESTING LEVEL {diff} from {from} to {to}");
            nfi.NumberDecimalSeparator = ".";

            string dirname = input_folder + $"\\level{diff.ToString("D2")}";
            List<string> fileName = Directory.GetFiles(dirname).ToList();

            StreamWriter sw = new StreamWriter(path.Substring(0, path.LastIndexOf("bin")) + $"results\\heur{diff}_{from}_{to}.csv", true);

            DateTime dt = DateTime.Now;
            float length = to - from + 1;
            int progress = 1;

            for (int i = from -1; i < to; i++)
            {
                string file = fileName[i];
                Instance inst;
                using (StreamReader sr = new StreamReader(file))
                    inst = new Instance(sr);

                (int rank_bt, double rank_time) = SolveIt(inst, 1, "Rank");
                (int attack_bt, double attack_time) = SolveIt(inst, 1, "Attack");
                (int center_bt, double center_time) = SolveIt(inst, 1, "Center");

                string id = Path.GetFileNameWithoutExtension(file);
                sw.WriteLine(id + "," + rank_bt + "," + attack_bt + "," + center_bt + "," + rank_time.ToString(nfi) + "," + attack_time.ToString(nfi) + "," + center_time.ToString(nfi));

                Console.WriteLine((progress / length) * 100 + " %");
                progress++;
            }
            Console.WriteLine("TOTAL TIME (s): \t " + (DateTime.Now - dt).Seconds);
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
