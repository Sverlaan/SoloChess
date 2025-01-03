﻿using System.Collections.Generic;
using System.Text;

namespace SoloChess
{
    public class VisistedSet
    {
        private HashSet<string> hashset;
        private StringBuilder sb;
        bool seen_before;

        public VisistedSet()
        {
            hashset = new HashSet<string>();
            sb = new StringBuilder();
        }

        public void Add(string hash){ hashset.Add(hash); }
        
        public bool SeenBefore(Piece[] pieces)
        {
            string prehash = PreHash(pieces);
            seen_before = hashset.Contains(prehash);
            if (!seen_before)
                hashset.Add(prehash);
            return seen_before;
        }

        public string PreHash(Piece[] pieces)
        {
            sb.Clear();
            foreach (Piece p in pieces)
            {
                if (p == null)
                    sb.Append("0");
                else
                    sb.Append(p.TypeID + "" + p.CapturesLeft);
            }
            return sb.ToString();
        }

        public void Clear(){ hashset = new HashSet<string>(); }
    }
}
