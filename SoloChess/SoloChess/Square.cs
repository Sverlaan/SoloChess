
namespace SoloChess
{
    public class Square
    {
        public int X { get; set; }
        public int Y { get; set; }

        public BlockingInfoDS bids;

        public Square(int x, int y)
        {
            this.X = x;
            this.Y = y;
            bids = new BlockingInfoDS();
        }

        public bool AlignHorizontal(Square s)
        {
            // Check whether two squares align horizontally on the board
            return this.Y == this.Y 
                && (this.bids.hor.next == s.bids.hor 
                || this.bids.hor.prev == s.bids.hor);
        }
        
        public bool AlignVertical(Square s)
        {
            // Check whether two squares align vertically on the board
            return this.X == s.X 
                && (this.bids.vert.next == s.bids.vert 
                || this.bids.vert.prev == s.bids.vert);
        }

        public bool AlignDiagonal1(Square s)
        {
            // Check whether two squares align diagonally on the board
            return this.X + this.Y == s.X + s.Y 
                && (this.bids.dig1.next == s.bids.dig1 
                || this.bids.dig1.prev == s.bids.dig1);
        }

        public bool AlignDiagonal2(Square s)
        {
            // Check whether two squares align diagonally on the board
            return this.X - this.Y == s.X - s.Y 
                && (this.bids.dig2.next == s.bids.dig2 
                || this.bids.dig2.prev == s.bids.dig2);
        }
    }
}
