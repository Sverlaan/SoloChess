using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SoloChess
{
    public class ChessForm : Form
    {
        // Internal game state fields
        Puzzle puzzle;
        Generator gen;

        // GUI fields
        Canvas canvas;
        Button input_button, generate_button, restart_button;
        NumericUpDown nup;
        bool show_grid = false;
        Image[] figures;
        Piece clicked;
        int marge_small = 10;
        int marge_big = 20;

        public ChessForm()
        {
            // Form settings
            this.Size = new Size(558, 650);
            this.BackColor = Color.AntiqueWhite;
            this.Text = "Solo Chess";
            //this.Icon = Properties.Resources.logo;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.BackgroundImage = Properties.Resources.wood;
            this.DoubleBuffered = true;

            // Load images from resources
            figures = GetFigures();

            // Initial board configuration
            puzzle = new Puzzle(new Instance());
            gen = new Generator();

            // For blocking / discovered attacks
            /*
            this.verticesX = vertices; 
            this.verticesY = vertices; 
            this.verticesXY = vertices; 
            this.verticesYX = vertices;
            Array.Sort(verticesX, delegate (Piece x, Piece y) { return x.X.CompareTo(y.X); });
            for (int i = 0; i < vertices.Length; i++)
                verticesX[i].ordX = i;
            Array.Sort(verticesY, delegate (Piece x, Piece y) { return x.Y.CompareTo(y.Y); });
            for (int i = 0; i < vertices.Length; i++)
                verticesY[i].ordY = i;
            Array.Sort(verticesXY, delegate (Piece x, Piece y) { return (x.X + x.Y).CompareTo(y.X + y.Y); });
            for (int i = 0; i < vertices.Length; i++)
                verticesXY[i].ordXY = i;
            Array.Sort(verticesYX, delegate (Piece x, Piece y) { return (x.X - x.Y).CompareTo(y.X - y.Y); });
            for (int i = 0; i < vertices.Length; i++)
                verticesYX[i].ordYX = i;
            */

            // Canvas for drawing the board
            canvas = new Canvas { Size = new Size(481, 481), Location = new Point(marge_big + marge_small, marge_big + marge_small), BackColor = Color.SaddleBrown };

            input_button = new Button
            {
                Location = new Point(canvas.Right + marge_small + marge_big, canvas.Top - marge_small),
                Text = "Input"
            };

            restart_button = new Button
            {
                Location = new Point(canvas.Left - marge_small, canvas.Bottom + marge_small + marge_big),
                Text = "Restart"
            };

            generate_button = new Button
            {
                Location = new Point(restart_button.Right + marge_small, restart_button.Top),
                Text = "Generate"
            };

            nup = new NumericUpDown
            {
                Location = new Point(generate_button.Right, generate_button.Top + 2),
                Size = new Size(50, generate_button.Height)
            };

            // Events
            this.Paint += DrawForm;
            canvas.Paint += DrawBoard;
            canvas.MouseClick += ClickBoard;
            input_button.Click += ChooseInput;
            restart_button.Click += RestartPuzzle;
            generate_button.Click += GenerateConfig;

            // Add controls
            Controls.AddRange(new Control[] { canvas, input_button, restart_button, generate_button, nup });
        }

        private void DrawForm(object sender, PaintEventArgs pea)
        {
            Graphics gr = pea.Graphics;

            // Draw the edge of the playboard
            gr.FillRectangle(Brushes.SaddleBrown, canvas.Location.X - marge_small, canvas.Location.Y - marge_small, canvas.Width + marge_small * 2, canvas.Height + marge_small * 2);

        }

        private void DrawBoard(object sender, PaintEventArgs pea)
        {
            Graphics gr = pea.Graphics;
            int cellsize = canvas.Width / puzzle.grid.GetLength(0);


            // Draw stones and hints
            for (int x = 0; x < puzzle.grid.GetLength(0); x++)
                for (int y = 0; y < puzzle.grid.GetLength(1); y++)
                {
                    // Coloring squares
                    if ((x % 2 != 0 && y % 2 == 0) || (x % 2 == 0 && y % 2 != 0))
                        gr.FillRectangle(Brushes.AntiqueWhite, x * cellsize, y * cellsize, cellsize, cellsize);
                    else
                        gr.FillRectangle(Brushes.SandyBrown, x * cellsize, y * cellsize, cellsize, cellsize);

                    // Drawing piece figures
                    Piece piece = puzzle.grid[x, y];
                    if (piece != null)
                    {
                        if (piece.clicked)
                            gr.FillRectangle(Brushes.YellowGreen, x * cellsize, y * cellsize, cellsize, cellsize);

                        if (clicked != null && puzzle.ValidMove(clicked, piece))
                            gr.FillEllipse(Brushes.IndianRed, new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));

                        if (piece.nCaptures > 0)
                            gr.DrawImage(figures[piece.State], new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));
                        else
                            gr.DrawImage(figures[piece.State + 6], new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));

                    }
                }

            // Draw board lines
            if (show_grid)
            {
                Pen pen = new Pen(Color.Gray, 1);
                for (int x = 0; x < canvas.Width; x += cellsize)
                    gr.DrawLine(pen, x, 0, x, canvas.Height);
                for (int y = 0; y < canvas.Height; y += cellsize)
                    gr.DrawLine(pen, 0, y, canvas.Width, y);
            }
        }

        public void ChooseInput(object sender, EventArgs ea)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Tekstfiles|*.txt";
            dialog.Title = "Open input configuration file...";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(dialog.FileName);
                puzzle = new Puzzle(sr);
                canvas.Invalidate();
            }
        }

        public void GenerateConfig(object sender, EventArgs ea)
        {
            Instance i = gen.GenValidInstance((int)nup.Value);
            puzzle = new Puzzle(i);

            canvas.Invalidate();
        }

        public void RestartPuzzle(object sender, EventArgs ea)
        {
            clicked = null;
            puzzle.Restart();
            canvas.Invalidate();
        }


        private void ClickBoard(object sender, MouseEventArgs mea)
        {
            // Get corresponding board cell from mouseclick coordinates
            int Ycell = (int)(mea.Y / (canvas.Width / puzzle.grid.GetLength(0)));
            int Xcell = (int)(mea.X / (canvas.Height / puzzle.grid.GetLength(1)));

            // To prevent index out of bound error when clicking on the east or south edge of the board
            if (Xcell >= puzzle.grid.GetLength(0))
                Xcell -= 1;
            if (Ycell >= puzzle.grid.GetLength(1))
                Ycell -= 1;

            Piece piece = puzzle.grid[Xcell, Ycell];
            if (piece != null)
            {
                if (piece.clicked)
                {
                    piece.clicked = false;
                    clicked = null;
                }
                else if (clicked != null)
                {
                    if (puzzle.ValidMove(clicked, piece))
                    {
                        puzzle.MoveVertex(clicked, piece);
                        clicked.clicked = false;
                        clicked = null;
                    }
                }
                else
                {
                    piece.clicked = true;
                    clicked = piece;
                }
            }
            canvas.Invalidate();
        }

        static Image[] GetFigures()
        {
            Image[] figures = new Image[13];
            figures[1] = Properties.Resources.kingW;
            figures[2] = Properties.Resources.queenW;
            figures[3] = Properties.Resources.rookW;
            figures[4] = Properties.Resources.bishopW;
            figures[5] = Properties.Resources.knightW;
            figures[6] = Properties.Resources.pawnW;
            figures[7] = Properties.Resources.kingB;
            figures[8] = Properties.Resources.queenB;
            figures[9] = Properties.Resources.rookB;
            figures[10] = Properties.Resources.bishopB;
            figures[11] = Properties.Resources.knightB;
            figures[12] = Properties.Resources.pawnB;
            return figures;
        }

    }

    public class Canvas : Panel
    {
        public Canvas()
        {
            ResizeRedraw = true;
            DoubleBuffered = true;
        }
    }
}
