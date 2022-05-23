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
        ChessBoard board;
        Button input_button, generate_button, restart_button;
        NumericUpDown nup;

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

            // Canvas for drawing the board
            board = new ChessBoard(new Puzzle(Generator.GenValidInstance(12)));
            board.Location = new Point(marge_big + marge_small, marge_big + marge_small);
            //canvas = new Canvas { Size = new Size(481, 481), Location = new Point(marge_big + marge_small, marge_big + marge_small), BackColor = Color.SaddleBrown };


            restart_button = new Button
            {
                Location = new Point(board.Left - marge_small, board.Bottom + marge_small + marge_big),
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
                Size = new Size(50, generate_button.Height),
                Value = 12,
                Maximum = 64,
                Minimum = 2,
                Increment = 1
            };

            input_button = new Button
            {
                Location = new Point(nup.Right + marge_big, generate_button.Top),
                Text = "Input"
            };

            // Events
            this.Paint += DrawForm;
            input_button.Click += ChooseInput;
            restart_button.Click += RestartGame;
            generate_button.Click += NewGame;

            // Add controls
            Controls.AddRange(new Control[] { board, input_button, restart_button, generate_button, nup });
        }

        private void DrawForm(object sender, PaintEventArgs pea)
        {
            // Draw the edge of the playboard
            pea.Graphics.FillRectangle(Brushes.SaddleBrown, board.Location.X - marge_small, board.Location.Y - marge_small, board.Width + marge_small * 2, board.Height + marge_small * 2);
        }

        public void ChooseInput(object sender, EventArgs ea)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Tekstfiles|*.txt";
            dialog.Title = "Open input configuration file...";

            if (dialog.ShowDialog() == DialogResult.OK)
                board.NewGame(new StreamReader(dialog.FileName));
        }

        public void NewGame(object sender, EventArgs ea){ board.NewGame((int)nup.Value); }

        public void RestartGame(object sender, EventArgs ea){ board.Restart(); }
    }


    public class ChessBoard : Panel
    {
        private Puzzle game;
        Piece clicked;
        Image[] figures;

        private Brush color_black, color_white, color_selected, color_attacked;

        public ChessBoard(Puzzle game)
        {
            Size = new Size(481, 481);
            BackColor = Color.SaddleBrown;
            ResizeRedraw = true;
            DoubleBuffered = true;
            color_white = Brushes.AntiqueWhite;
            color_black = Brushes.SandyBrown;
            color_selected = Brushes.YellowGreen;
            color_attacked = Brushes.IndianRed;

            figures = GetFigures();

            this.game = game;

            // Events
            this.Paint += DrawBoard;
            this.MouseClick += ClickBoard;
        }

        private void DrawBoard(object sender, PaintEventArgs pea)
        {
            Graphics gr = pea.Graphics;
            int cellsize = this.Width / game.grid.GetLength(0);

            for (int x = 0; x < game.grid.GetLength(0); x++)
                for (int y = 0; y < game.grid.GetLength(1); y++)
                {
                    // Coloring squares
                    if ((x % 2 != 0 && y % 2 == 0) || (x % 2 == 0 && y % 2 != 0))
                        gr.FillRectangle(color_white, x * cellsize, y * cellsize, cellsize, cellsize);
                    else
                        gr.FillRectangle(color_black, x * cellsize, y * cellsize, cellsize, cellsize);

                    // Drawing piece figures
                    Piece piece = game.grid[x, y];
                    if (piece != null)
                    {
                        if (piece.clicked)
                            gr.FillRectangle(color_selected, x * cellsize, y * cellsize, cellsize, cellsize);

                        if (clicked != null && game.ValidMove(clicked, piece))
                            gr.FillRectangle(color_attacked, new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));

                        if (piece.nCaptures > 0)
                            gr.DrawImage(figures[piece.State], new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));
                        else
                            gr.DrawImage(figures[piece.State + 6], new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));
                    }
                }
        }

        private void ClickBoard(object sender, MouseEventArgs mea)
        {
            // Handle movement highlighting
            Piece piece = GetClickedPiece(mea.X, mea.Y);
            if (piece != null)
            {
                if (piece.clicked)                    
                    this.ResetClicked();
                else if (clicked != null)
                {
                    // Perform move if valid
                    if (game.ValidMove(clicked, piece))
                    {
                        game.MoveVertex(clicked, piece);
                        this.ResetClicked();
                    }
                }   
                else
                    SetClicked(piece);
            } 
            this.Invalidate();
        }
        private Piece GetClickedPiece(int click_x, int click_y)
        {
            // Get corresponding grid cell from pixel coordinates
            int Xcell = (int)(click_x / (this.Height / game.grid.GetLength(1)));
            int Ycell = (int)(click_y / (this.Width / game.grid.GetLength(0)));

            // Prevents index out of bounds error when clicking on outer edge
            if (Xcell >= game.grid.GetLength(0))
                Xcell -= 1;
            if (Ycell >= game.grid.GetLength(1))
                Ycell -= 1;

            return game.grid[Xcell, Ycell];
        }

        public void Restart() 
        { 
            clicked = null; 
            game.Restart(); 
            this.Invalidate(); 
        }
        public void NewGame(int difficulty) 
        { 
            game = new Puzzle(Generator.GenValidInstance(difficulty)); 
            this.Invalidate(); 
        }
        public void NewGame(StreamReader sr) 
        { 
            game = new Puzzle(new Instance(sr)); 
            this.Invalidate(); 
        }

        private void SetClicked(Piece p) { p.clicked = true; clicked = p; }     // Highlight clicked piece
        private void ResetClicked(){ clicked.clicked = false; clicked = null; } // Unhighlight

        private static Image[] GetFigures()
        {
            // Gets the figure images
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
}
