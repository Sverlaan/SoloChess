using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SoloChess
{
    public class Puzzle
    {
        public Piece[,] grid;
        public List<Piece> pieces;

        public bool goal_reached;
        private int depth;          // used in checking whether goal state has been reached, for single player game play

        private Instance start_config;


        public Puzzle(Instance instance)
        {
            ParseInstance(instance);
            InitNodes();
            depth = pieces.Count;
            goal_reached = false;
        }


        public void InitNodes()
        {
            // Initialize the linked list structure used in the Blocking datastructure
            // For its use, see BlockingDS class

            void InitNodes2(List<Node> ordered)
            {
                if (ordered.Count < 2)
                    return;

                // Set next, prev pointers between nodes
                for (int i = 0; i < ordered.Count; i++)
                {
                    Node cur = ordered[i];
                    if (i == 0)
                        cur.next = ordered[i + 1];
                    else if (i == ordered.Count - 1)
                        cur.prev = ordered[i - 1];
                    else
                    {
                        cur.next = ordered[i + 1];
                        cur.prev = ordered[i - 1];
                    }
                }
            }

            // Get squares
            Square[] squares2 = pieces.Select(p => p.Square).ToArray();
            List<List<Square>> grouped_squares;

            // vertical
            grouped_squares = squares2.GroupBy(p => p.X).Select(grp => grp.ToList()).ToList();
            foreach(List<Square> l in grouped_squares)
                InitNodes2(l.OrderBy(p => p.Y).Select(n => n.bids.vert).ToList());

            // Horizontal
            grouped_squares = squares2.GroupBy(p => p.Y).Select(grp => grp.ToList()).ToList();
            foreach (List<Square> l in grouped_squares)
                InitNodes2(l.OrderBy(p => p.X).Select(n => n.bids.hor).ToList());

            // Diagonal 1
            grouped_squares = squares2.GroupBy(p => p.X + p.Y).Select(grp => grp.ToList()).ToList();
            foreach (List<Square> l in grouped_squares)
                InitNodes2(l.OrderBy(p => p.X - p.Y).Select(n => n.bids.dig1).ToList());

            // Diagonal 2
            grouped_squares = squares2.GroupBy(p => p.X - p.Y).Select(grp => grp.ToList()).ToList();
            foreach (List<Square> l in grouped_squares)
                InitNodes2(l.OrderBy(p => p.X + p.Y).Select(n => n.bids.dig2).ToList());
        }
         
        public void ParseInstance(Instance instance)
        {
            // Convert Instance-object to Puzzle-object

            // Initialize
            this.start_config = instance;
            this.grid = new Piece[instance.width, instance.height];
            this.pieces = new List<Piece>();

            // Add pieces
            foreach ((int p, int x, int y, int c) in instance.list_of_piece_information)
            {
                Piece piece = ParsePieceType(p, new Square(x,y), c);
                grid[x, y] = piece;
                pieces.Add(piece);
            }
        }

        public Piece ParsePieceType(int p, Square s, int c)
        {
            switch(p)
            {
                case 1:
                    return new King(s, c);
                case 2:
                    return new Queen(s, c);
                case 3:
                    return new Rook(s, c);
                case 4:
                    return new Bishop(s, c);
                case 5:
                    return new Knight(s, c);
                default:
                    return new Pawn(s, c);
            }
        }

        public void MoveVertex(Piece p, Piece q)
        {
            // Update board grid
            grid[p.Square.X, p.Square.Y] = null;
            grid[q.Square.X, q.Square.Y] = p;

            p.CapturesLeft--;

            p.Square.bids.Remove();  // update for BlockingDS
            p.Square = q.Square;

            depth--;

            // Check whether goal state has been reached in single player gameplay
            // This can only happen aften n-1 moves
            if (depth <= 1)          
                goal_reached = true;
        }

        public bool ValidMove(Piece p, Piece q)
        {
            return p.ValidCapture(q);
        }

        public string GetPieceTypes()
        {
            // Gets the type of all pieces currently on the board, with the exception of the King
            // Used for comparing on skewness of piece type distribution 
            StringBuilder sb = new StringBuilder();
            foreach (Piece p in this.pieces)
                if (p.TypeID != 1)
                    sb.Append(p.Rank);

            return sb.ToString();
        }

        public void Restart()
        {
            // Reset puzzle to start configuration
            ParseInstance(start_config);
            InitNodes();
            depth = pieces.Count;
            goal_reached = false;
        }
    }
}
