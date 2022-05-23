using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SoloChess
{
    public class Puzzle
    {
        public Piece[,] grid;
        public Piece[] pieces;

        public Instance start_config;


        public Puzzle(Instance instance)
        {
            ParseInstance(instance);
            InitNodes();
        }

        public void InitNodes()
        {
            Piece[] arr = new Piece[pieces.Length];
            arr = (Piece[])pieces.Clone();

            // horizontal
            Array.Sort(arr, delegate (Piece x, Piece y) { return x.X.CompareTo(y.X); });

            var groupedlist = arr.GroupBy<Piece, int>(p => p.X).Select(grp => grp.ToList()).ToList();
            foreach(List<Piece> l in groupedlist)
            {
                List<Piece> ordered = l.OrderBy<Piece, int>(p => p.Y).ToList<Piece>();

                if (ordered.Count < 2)
                    continue;

                for (int i = 0; i < ordered.Count; i++)
                {
                    Piece cur = ordered[i];
                    if (i == 0)
                        cur.vert.next = ordered[i + 1].vert;
                    else if (i == ordered.Count - 1)
                        cur.vert.prev = ordered[i - 1].vert;
                    else
                    {
                        cur.vert.next = ordered[i + 1].vert;
                        cur.vert.prev = ordered[i - 1].vert;
                    }
                }
            }

            // vertical
            Piece[] arr2 = new Piece[pieces.Length];
            arr2 = (Piece[])pieces.Clone();

            Array.Sort(arr2, delegate (Piece x, Piece y) { return x.Y.CompareTo(y.Y); });

            var groupedlist2 = arr2.GroupBy<Piece, int>(p => p.Y).Select(grp => grp.ToList()).ToList();
            foreach (List<Piece> l in groupedlist2)
            {
                List<Piece> ordered = l.OrderBy<Piece, int>(p => p.X).ToList<Piece>();

                if (ordered.Count < 2)
                    continue;

                for (int i = 0; i < ordered.Count; i++)
                {
                    
                    Piece cur = ordered[i];
                    if (i == 0)
                        cur.hor.next = ordered[i + 1].hor;
                    else if (i == ordered.Count - 1)
                        cur.hor.prev = ordered[i - 1].hor;
                    else
                    {
                        cur.hor.next = ordered[i + 1].hor;
                        cur.hor.prev = ordered[i - 1].hor;
                    }
                }
            }

            
            // diagonal1
            Array.Sort(arr, delegate (Piece x, Piece y) { return (x.X + x.Y).CompareTo(y.X + y.Y); });

            groupedlist = arr.GroupBy<Piece, int>(p => (p.X + p.Y)).Select(grp => grp.ToList()).ToList();
            foreach (List<Piece> l in groupedlist)
            {
                List<Piece> ordered = l.OrderBy<Piece, int>(p => (p.X - p.Y)).ToList<Piece>();

                if (ordered.Count < 2)
                    continue;

                for (int i = 0; i < ordered.Count; i++)
                {
                    Piece cur = ordered[i];
                    if (i == 0)
                        cur.dig1.next = ordered[i + 1].dig1;
                    else if (i == ordered.Count - 1)
                        cur.dig1.prev = ordered[i - 1].dig1;
                    else
                    {
                        cur.dig1.next = ordered[i + 1].dig1;
                        cur.dig1.prev = ordered[i - 1].dig1;
                    }
                }
            }

            // diagonal2
            Array.Sort(arr, delegate (Piece x, Piece y) { return (x.X - x.Y).CompareTo(y.X - y.Y); });

            groupedlist = arr.GroupBy<Piece, int>(p => (p.X - p.Y)).Select(grp => grp.ToList()).ToList();
            foreach (List<Piece> l in groupedlist)
            {
                List<Piece> ordered = l.OrderBy<Piece, int>(p => (p.X + p.Y)).ToList<Piece>();

                if (ordered.Count < 2)
                    continue;

                for (int i = 0; i < ordered.Count; i++)
                {
                    Piece cur = ordered[i];
                    if (i == 0)
                        cur.dig2.next = ordered[i + 1].dig2;
                    else if (i == ordered.Count - 1)
                        cur.dig2.prev = ordered[i - 1].dig2;
                    else
                    {
                        cur.dig2.next = ordered[i + 1].dig2;
                        cur.dig2.prev = ordered[i - 1].dig2;
                    }
                }
            }
        }

        public void InitNodesPointers(List<Piece> l)
        {
            List<Piece> ordered = l.OrderBy<Piece, int>(p => p.Y).ToList<Piece>();

            if (ordered.Count < 2)
                return;

            for(int i = 0; i < ordered.Count; i++)
            {
                Piece cur = ordered[i];
                if (i == 0)
                    cur.vert.next = ordered[i+1].vert;
                else if (i == ordered.Count - 1)
                    cur.vert.prev = ordered[i - 1].vert;
                else
                {
                    cur.vert.next = ordered[i + 1].vert;
                    cur.vert.prev = ordered[i - 1].vert;
                }

            }
        }

        
        

        public void ParseInstance(Instance instance)
        {
            this.start_config = instance;
            this.grid = new Piece[instance.width, instance.height];
            this.pieces = new Piece[instance.n];

            int i = 0;
            foreach ((int p, int x, int y, int c) in instance.p_list)
            {
                Piece piece = new Piece(p, x, y, c);

                grid[x, y] = piece;
                pieces[i] = piece;
                i++;
            }
        }

        

        


        public void MoveVertex(Piece v, Piece w)
        {
            grid[v.X, v.Y] = null;
            w.State = v.State;
            w.nCaptures = v.nCaptures - 1;

            if (v.vert.prev != null)
            {
                v.vert.prev.next = v.vert.next;
            }
            if(v.vert.next != null)
            {
                v.vert.next.prev = v.vert.prev;
            }


            if (v.hor.prev != null)
            {
                v.hor.prev.next = v.hor.next;
            }
            if (v.hor.next != null)
            {
                v.hor.next.prev = v.hor.prev;
            }


            if (v.dig2.prev != null)
            {
                v.dig2.prev.next = v.dig2.next;
            }
            if (v.dig2.next != null)
            {
                v.dig2.next.prev = v.dig2.prev;
            }


            if (v.dig1.prev != null)
            {
                v.dig1.prev.next = v.dig1.next;
            }
            if (v.dig1.next != null)
            {
                v.dig1.next.prev = v.dig1.prev;
            }

        }

        public bool ValidMove(Piece p1, Piece p2)
        {
            if (p1 == p2)
                return false;
            if (p1.nCaptures <= 0)
                return false;

            switch (p1.State)
            {
                case 1: // king
                    if (Math.Abs(p1.X - p2.X) <= 1 && Math.Abs(p1.Y - p2.Y) <= 1)
                        return true;
                    break;
                case 2: // Queen
                    if (p1.X == p2.X && (p1.vert.next == p2.vert || p1.vert.prev == p2.vert))
                        return true;
                    else if (p1.Y == p2.Y && (p1.hor.next == p2.hor || p1.hor.prev == p2.hor))
                        return true;
                    else if (p1.X - p1.Y == p2.X - p2.Y && (p1.dig2.next == p2.dig2 || p1.dig2.prev == p2.dig2))
                        return true;
                    else if (p1.X + p1.Y == p2.X + p2.Y && (p1.dig1.next == p2.dig1 || p1.dig1.prev == p2.dig1))
                        return true;
                    break;
                case 3: // rook
                    if (p1.X == p2.X && (p1.vert.next == p2.vert || p1.vert.prev == p2.vert))
                        return true;
                    else if (p1.Y == p2.Y && (p1.hor.next == p2.hor || p1.hor.prev == p2.hor))
                        return true;
                    break;
                case 4: // bishop
                    if (p1.X - p1.Y == p2.X - p2.Y && (p1.dig2.next == p2.dig2 || p1.dig2.prev == p2.dig2))
                        return true;
                    else if (p1.X + p1.Y == p2.X + p2.Y && (p1.dig1.next == p2.dig1 || p1.dig1.prev == p2.dig1))
                        return true;
                    break;
                case 5: // knight
                    if (p2.Y == p1.Y - 2 && (p2.X == p1.X - 1 || p2.X == p1.X + 1))
                        return true;
                    else if (p2.Y == p1.Y + 2 && (p2.X == p1.X - 1 || p2.X == p1.X + 1))
                        return true;
                    else if (p2.X == p1.X - 2 && (p2.Y == p1.Y - 1 || p2.Y == p1.Y + 1))
                        return true;
                    else if (p2.X == p1.X + 2 && (p2.Y == p1.Y - 1 || p2.Y == p1.Y + 1))
                        return true;
                    break;
                case 6: // pawn
                    if (p2.X == p1.X - 1 && p2.Y == p1.Y - 1)
                        return true;
                    else if (p2.X == p1.X + 1 && p2.Y == p1.Y - 1)
                        return true;
                    break;
            }
            return false;
        }

        public void Restart()
        {
            ParseInstance(start_config);
            InitNodes();
        }
    }


    public class Piece
    {
        public int State;
        public int X, Y;
        public bool clicked;
        public int nCaptures;

        public Node hor, vert, dig1, dig2;

        public Piece(int state, int x, int y, int c)
        {
            this.State = state;
            this.X = x;
            this.Y = y;
            nCaptures = c;

            vert = new Node(this, null, null);
            hor = new Node(this, null, null);
            dig1 = new Node(this, null, null);
            dig2 = new Node(this, null, null);
        }
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

    public class Instance
    {
        public int n;
        public int width;
        public int height;

        public List<(int, int, int, int)> p_list;

        public Instance()
        {
            n = 0;
            width = 8;
            height = 8;
            p_list = new List<(int, int, int, int)>();
        }

        public Instance(StreamReader sr)
        {
            string[] line = sr.ReadLine().Split();
            n = int.Parse(line[0]);
            width = int.Parse(line[1]);
            height = int.Parse(line[2]);

            p_list = new List<(int, int, int, int)>();
            for (int i = 0; i < n; i++)
            {
                line = sr.ReadLine().Split();
                int type = int.Parse(line[0]);
                int x = int.Parse(line[1]);
                int y = int.Parse(line[2]);
                int c = int.Parse(line[3]);
                p_list.Add((type, x, y, c));
            }
        }

        public void Add(int p, int x, int y, int c)
        {
            n++;
            if (x > width)
                width = x + 1;
            if (y > height)
                height = y + 1;

            p_list.Add((p, x, y, c));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{n} {width} {height}");
            foreach((int p, int x, int y, int c) in p_list)
            {
                sb.Append("\n");
                sb.Append($"{p} {x} {y} {c}");
            }
            return sb.ToString();
        }
    }
}
