using System.Collections.Generic;

namespace SoloChess
{
    /*
    This class is used for efficiently checking whether two squares are blocked by another square or not
    This is highly important when checking whether a piece is in a position to capture another piece
    Simply checking whether the two squares are aligned is not enough
    To solve this, there are linked list structures for every occupied row, column and diagonal in the start configuration
    
    Now checking can be done in constant time, instead of linear time for iterating full rows/columns/diagonals

    Each square has a BlockingInfoDS-object with information regarding the next and prev for each linked list the square is part of
    */
    public class BlockingInfoDS
    {
        public Node hor, vert, dig1, dig2;
        public int possible_knight_attacks;
        public BlockingInfoDS()
        {
            vert = new Node();
            hor = new Node();
            dig1 = new Node();
            dig2 = new Node();
            possible_knight_attacks = 0;
        }

        public void Remove()
        {
            // Remove square from all its linked lists structures
            // Happens when the square becomes unoccupied after a move
            foreach(Node node in new List<Node>(){ this.hor, this.vert, this.dig1, this.dig2})
            {
                if (node.prev != null)
                    node.prev.next = node.next;
                if (node.next != null)
                    node.next.prev = node.prev;
            }
        }

        public void AddBack()
        {
            // Add square back to its linked list structures
            // Happens when a move is made undone, after a backtrack
            foreach (Node node in new List<Node>() { this.hor, this.vert, this.dig1, this.dig2})
            {
                if (node.prev != null)
                    node.prev.next = node;
                if (node.next != null)
                    node.next.prev = node;
            }
        }

        public bool Empty()
        {
            // Check whether square is completely secluded, by checking if there are other squares in its linked list structures
            bool empty = true;
            foreach (Node node in new List<Node>() { this.hor, this.vert, this.dig1, this.dig2})
            {
                if (node.prev != null)
                    empty = false;
                if (node.next != null)
                    empty = false;
            }

            return empty;
        }
    }


    public class Node
    {
        public Node next, prev;

        public Node(){}
    }
}
