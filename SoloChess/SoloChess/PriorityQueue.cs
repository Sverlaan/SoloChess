using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    /*
    internal class PriorityQueue
    {
        public Move[] moves;
        public int HeapSize;

        public PriorityQueue(Move[] moves)
        {
            this.moves = moves;
            BuildMinHeap();
        }

        public int Parent(int i) { return (i - 1) / 2; }
        public int Left(int i) { return 2 * i + 1; }
        public int Right(int i) { return 2 * i + 2; }

        public void BuildMinHeap()
        {
            HeapSize = moves.Length;
            for (int i = moves.Length / 2 - 1; i >= 0; i--)
                MinHeapify(i);
        }

        public void MinHeapify(int i)
        {
            int l = Left(i);
            int r = Right(i);

            int smallest;
            if (l < HeapSize && moves[l].d < moves[i].d)
                smallest = l;
            else smallest = i;

            if (r < HeapSize && moves[r].d < moves[smallest].d)
                smallest = r;

            if (smallest != i)
            {
                SwapNodes(i, smallest);
                MinHeapify(smallest);
            }
        }

        public void DecreaseKey(int i, int key)
        {
            if (key < moves[i].d)
            {
                moves[i].d = key;
                while (i > 0 && moves[Parent(i)].d > moves[i].d)
                {
                    SwapNodes(i, Parent(i));
                    i = Parent(i);
                }
            }
            else
                Console.WriteLine("NEW KEY IS NOT SMALLER THAN CURRENT");
        }

        public Move ExtractMin()
        {
            if (HeapSize > 0)
            {
                Move min = moves[0];
                moves[0] = moves[HeapSize - 1];
                HeapSize -= 1;
                MinHeapify(0);
                return min;
            }
            else
            {
                Console.WriteLine("HEAP UNDERFLOW");
                return null;
            }
        }

        public void SwapNodes(int i, int j)
        {
            Move a = moves[i];
            Move b = moves[j];
            moves[i] = b;
            b.HeapIndex = i;
            moves[j] = a;
            a.HeapIndex = j;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("HEAP:\n");

            for (int i = 0; i < HeapSize; i++)
            {
                Move n = moves[i];
                sb.Append(n.ToString() + "\n");
            }
            return sb.ToString();
        }
    }
    */
}
