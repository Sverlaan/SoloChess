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
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ChessForm());

            //Tester.GetChar();

            for(int i = 2; i <= 14; i++)
            {
                Tester.TestRandom(i, 0, 1000);
            }

            Tester.TestRandom(12, 0, 1000);

        }

    }
}
