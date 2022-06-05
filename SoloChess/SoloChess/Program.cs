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
             
            //Test(nPuzzles: 100, nPerPuzzle: 50, diff: 6, heuristic: 1);
            

        }


        static void Test(int nPuzzles, int nPerPuzzle, int diff, int heuristic)
        {
            DateTime dt = DateTime.Now;

            int rand = 0;
            int heur = 0;
            for (int j = 1; j <= nPuzzles; j++)
            {
                Instance inst = Generator.GenValidInstance(diff);

                rand += SolveIt(inst, nPerPuzzle, 0, false);
                heur += SolveIt(inst, 1, heuristic, true);


                Console.WriteLine((j / (float)nPuzzles)*100 + " %");
            }
            int rand_gem = rand / nPuzzles;
            int heur_gem = heur / nPuzzles;
            Console.WriteLine("\nRAND BT: " + rand_gem + "\n" + "HEUR BT: " + heur_gem);
            Console.WriteLine("ratio: " + (((float)heur_gem) + 1) / (((float)rand_gem) + 1) + "\n" + "diff: " + (rand_gem - heur_gem));
            Console.WriteLine((DateTime.Now - dt).TotalSeconds);
            Console.ReadLine();
        }

        static int SolveIt(Instance inst, int n, int heuristic, bool forward_check)
        {
            int res = 0;
            for (int i = 0; i < n; i++)
            {
                Puzzle puzzle = new Puzzle(inst);
                Solver sol = new Solver(puzzle);
                int nback = sol.Solve(puzzle, heuristic, forward_check);
                res += nback;
            }
            return res / n;
        }
    }
}
