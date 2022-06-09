using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
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

        public Node()
        {
        }
    }
}
