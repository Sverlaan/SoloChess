using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    internal class BlockChecker
    {

    }


    public class Node
    {
        public Piece piece;
        public Node next, prev;

        public Node(Piece piece, Node next, Node prev)
        {
            this.piece = piece;
            this.next = next;
            this.prev = prev;
        }

        public void AddBack()
        {
            this.prev.next = this;
            this.next.prev = this;
        }
    }
}
