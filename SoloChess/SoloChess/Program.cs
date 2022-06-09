using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Timers;

namespace SoloChess
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Generator.WriteInputs(5, 4, 20);

            Application.Run(new ChessForm());

            //Test(nPuzzles: 1000, nPerPuzzle: 100, diff: 8);
            //Console.ReadLine();
        }

        


        static void Test(int nPuzzles, int nPerPuzzle, int diff, string heur1, string heur2)
        {
            DateTime dt = DateTime.Now;

            int heur1_backtracks = 0;
            int heur2_backtracks = 0;

            for (int j = 1; j <= nPuzzles; j++)
            {
                Instance inst = Generator.GenValidInstance(diff);

                heur1_backtracks += SolveIt(inst, nPerPuzzle, heur1);
                heur2_backtracks += SolveIt(inst, 1, heur2);

                Console.WriteLine((j / (float)nPuzzles)*100 + " %");
            }

            int heur1_gem = heur1_backtracks / nPuzzles;
            int heur2_gem = heur2_backtracks / nPuzzles;

            Console.WriteLine("\nRAND BT: " + heur1_gem + "\n" + "HEUR BT: " + heur2_gem);
            Console.WriteLine("ratio: " + (((float)heur2_gem) + 1) / (((float)heur1_gem) + 1));
            Console.WriteLine((DateTime.Now - dt).TotalSeconds);
        }

        static int SolveIt(Instance inst, int n, string heur_fun)
        {
            int res = 0;
            for (int i = 0; i < n; i++)
            {
                Puzzle puzzle = new Puzzle(inst);
                Solver solver = new Solver(puzzle, heur_fun);
                (LinkedList<string> sol, int nback) = solver.Solve(puzzle);
                res += nback;
            }
            return res / n;
        }
    }
}
