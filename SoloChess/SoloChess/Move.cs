using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    public class Move
    {
        public Piece from, to;
        public float Value;
        public int arrLoc;
        private string[] states = new string[] { "", "K", "Q", "R", "B", "N", "P" };

        public Move(Piece p, Piece q)
        {
            this.from = p;
            this.to = q;
            arrLoc = -1;
        }

        public override string ToString()
        {
            return states[from.State] + from.Square.X.ToString() + from.Square.Y.ToString()
                + "x" + states[to.State] + to.Square.X.ToString() + to.Square.Y.ToString();
        }
    }
}
