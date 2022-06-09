using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    
    public class VisistedSet
    {
        private HashSet<string> hashset;


        public VisistedSet(Piece[] pieces)
        {
            hashset = new HashSet<string>();
        }

        public void Add(string hash)
        {
            hashset.Add(hash);
        }
        
        public bool SeenBefore(Piece[] pieces)
        {
            string hash = Hash(pieces);
            bool seen_before = hashset.Contains(hash);
            if (!seen_before)
                hashset.Add(hash);
            return seen_before;
        }

        public string Hash(Piece[] pieces)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Piece p in pieces)
            {
                if (p == null)
                {
                    sb.Append("e");
                }
                else
                {
                    sb.Append(p.State);
                    sb.Append(p.CapturesLeft);
                }
            }
            return sb.ToString();
        }


        public void Clear()
        {
            hashset = new HashSet<string>();
        }
    }
}
