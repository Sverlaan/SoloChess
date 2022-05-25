using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    internal class Square
    {
        int x, y;
        Piece piece;

        public Square(int x, int y, Piece piece)
        {
            this.x = x;
            this.y = y;
            this.piece = piece;
        }
    }
}
