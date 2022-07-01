
namespace SoloChess
{
    public class Move
    {
        public Piece from, to;   // Move represents capture of piece "to" by piece "from"
        public float Value;      // Heuristic value of move

        private string[] types = new string[] { "", "K", "Q", "R", "B", "N", "P" };  // Used in converting move to string

        public Move(Piece p, Piece q)
        {
            this.from = p;
            this.to = q;
        }

        public override string ToString()
        {
            return types[from.TypeID] + from.Square.X.ToString() + from.Square.Y.ToString()
                + "x" + types[to.TypeID] + to.Square.X.ToString() + to.Square.Y.ToString();
        }
    }
}
