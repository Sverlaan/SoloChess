using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace SoloChess
{
    public class Puzzle
    {
        public Piece[,] grid;
        public Piece[] pieces;

        public int depth;
        public bool goal;

        public Instance start_config;


        public Puzzle(Instance instance)
        {
            ParseInstance(instance);
            InitNodes();
            depth = pieces.Length;
            goal = false;
        }

        public void InitNodes()
        {
            void InitNodes2(List<Node> ordered)
            {
                if (ordered.Count < 2)
                    return;

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

            Square[] squares2 = pieces.Select(p => p.Square).ToArray();
            List<List<Square>> grouped_squares;

            // vertical
            grouped_squares = squares2.GroupBy(p => p.X).Select(grp => grp.ToList()).ToList();
            foreach(List<Square> l in grouped_squares)
                InitNodes2(l.OrderBy(p => p.Y).Select(n => n.bids.vert).ToList());

            grouped_squares = squares2.GroupBy(p => p.Y).Select(grp => grp.ToList()).ToList();
            foreach (List<Square> l in grouped_squares)
                InitNodes2(l.OrderBy(p => p.X).Select(n => n.bids.hor).ToList());

            grouped_squares = squares2.GroupBy(p => p.X + p.Y).Select(grp => grp.ToList()).ToList();
            foreach (List<Square> l in grouped_squares)
                InitNodes2(l.OrderBy(p => p.X - p.Y).Select(n => n.bids.dig1).ToList());

            grouped_squares = squares2.GroupBy(p => p.X - p.Y).Select(grp => grp.ToList()).ToList();
            foreach (List<Square> l in grouped_squares)
                InitNodes2(l.OrderBy(p => p.X + p.Y).Select(n => n.bids.dig2).ToList());

            
        }
         

        public void ParseInstance(Instance instance)
        {
            this.start_config = instance;
            this.grid = new Piece[instance.width, instance.height];
            this.pieces = new Piece[instance.n];

            int i = 0;
            foreach ((int p, int x, int y, int c) in instance.p_list)
            {
                Piece piece = ParseState(p, new Square(x,y), c);

                grid[x, y] = piece;
                pieces[i] = piece;
                i++;
            }
        }

        public Piece ParseState(int p, Square s, int c)
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
            grid[p.Square.X, p.Square.Y] = null;
            grid[q.Square.X, q.Square.Y] = p;

            p.CapturesLeft--;

            p.Square.bids.Remove();
            p.Square = q.Square;

            depth--;
            if (depth <= 1)
                goal = true;
        }

        public bool ValidMove(Piece p, Piece q)
        {
            return p.ValidCapture(q);
        }

        public void Restart()
        {
            ParseInstance(start_config);
            InitNodes();
            depth = pieces.Length;
            goal = false;
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
