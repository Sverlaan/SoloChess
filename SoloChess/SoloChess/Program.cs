using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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

            //Application.Run(new ChessForm());


            int rand = 0;
            int heur = 0;
            int m = 100;
            for (int j = 0; j < m; j++)
            {
                Console.WriteLine("iter: " + j);
                Instance inst = Generator.GenValidInstance(8);

                int total = 0;
                int n = 50;
                for (int i = 0; i < n; i++)
                {
                    Puzzle puzzle = new Puzzle(inst);
                    Solver sol = new Solver(puzzle);
                    sol.nBacktracks = 0;
                    //sol.InitCenter(puzzle.pieces.ToList());
                    (bool solved, int nback) = sol.Solve(puzzle.pieces.ToList(), new Stack<string>(), false);
                    total += nback;
                }
                rand += total / n;

                Puzzle puzzle2 = new Puzzle(inst);
                Solver sol2 = new Solver(puzzle2);
                //sol2.InitCenter(puzzle2.pieces.ToList());
                sol2.nBacktracks = 0;
                (bool solved2, int nback2) = sol2.Solve(puzzle2.pieces.ToList(), new Stack<string>(), true);
                heur += nback2;
            }
            int rand_gem = rand / m;
            int heur_gem = heur / m;
            Console.WriteLine("RAND BT: " + rand_gem);
            Console.WriteLine("HEUR BT: " + heur_gem);
            Console.WriteLine("ratio: " + (((float) heur_gem) + 1) / (((float)rand_gem) + 1));
            Console.WriteLine("diff: " + (rand_gem - heur_gem));
            Console.ReadLine();

        }
    }
}
