using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SoloChess
{
    public class Instance
    {
        public int n;         // Number of pieces
        public int width;     // Maximum X coordinate, for board width
        public int height;    // Maximum Y coordinate, for board height

        // List of pieces in the instance
        // (type, x, y, max_captures)
        public List<(int, int, int, int)> list_of_piece_information;

        public Instance()
        {
            n = 0;
            width = 8;
            height = 8;
            list_of_piece_information = new List<(int, int, int, int)>();
        }


        public Instance(StreamReader sr)
        {
            // Parse Instance from textfile
            string[] line = sr.ReadLine().Split();
            n = int.Parse(line[0]);
            width = int.Parse(line[1]);
            height = int.Parse(line[2]);

            list_of_piece_information = new List<(int, int, int, int)>();
            for (int i = 0; i < n; i++)
            {
                line = sr.ReadLine().Split();
                int type = int.Parse(line[0]);
                int x = int.Parse(line[1]);
                int y = int.Parse(line[2]);
                int c = int.Parse(line[3]);
                list_of_piece_information.Add((type, x, y, c));
            }
        }

        public void Add(int p, int x, int y, int c)
        {
            // Add new piece to instance
            n++;
            if (x > width)
                width = x + 1;
            if (y > height)
                height = y + 1;

            list_of_piece_information.Add((p, x, y, c));
        }

        public override string ToString()
        {
            // Convert instance to string, for exporting to textfile
            StringBuilder sb = new StringBuilder();

            sb.Append($"{n} {width} {height}");
            foreach ((int p, int x, int y, int c) in list_of_piece_information)
            {
                sb.Append("\n");
                sb.Append($"{p} {x} {y} {c}");
            }
            return sb.ToString();
        }

    }
}
