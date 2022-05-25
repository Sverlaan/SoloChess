using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace SoloChess
{
    public class ChessForm : Form
    {
        ChessBoard board;
        MyButton input_button, generate_button, restart_button;
        NumericUpDown nup;

        int marge_small = 1;
        int marge_big = 30;

        public ChessForm()
        {
            // Form settings
            this.Size = new Size(558, 650);
            this.Text = "Solo Chess";
            //this.Icon = Properties.Resources.logo;
            //this.BackgroundImageLayout = ImageLayout.Stretch;
            //this.BackgroundImage = Properties.Resources.wood;
            this.BackColor = Color.FromArgb(88, 85, 80);
            this.DoubleBuffered = true;

            // Canvas for drawing the board
            board = new ChessBoard(new Puzzle(Generator.GenValidInstance(12)));
            board.Location = new Point(marge_big + marge_small, marge_big + marge_small);
            //canvas = new Canvas { Size = new Size(481, 481), Location = new Point(marge_big + marge_small, marge_big + marge_small), BackColor = Color.SaddleBrown };


            input_button = new MyButton("Input", board.Left, board.Bottom + marge_small + marge_big);
            restart_button = new MyButton("Restart", input_button.Right + marge_small, input_button.Top);
            generate_button = new MyButton("Generate", restart_button.Right + marge_small, restart_button.Top);
            nup = new NumericUpDown
            {
                Location = new Point(generate_button.Right + marge_small, generate_button.Top),
                Size = new Size(119, 200),
                Value = 12,
                Maximum = 64,
                Minimum = 2,
                Increment = 1,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Gorga Grotesque", (float)17.5, FontStyle.Regular),
                BackColor = Color.FromArgb(66, 62, 58),
                ForeColor = Color.FromArgb(156, 148, 144),
                TextAlign = HorizontalAlignment.Center
            };
            nup.BorderStyle = BorderStyle.None;


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
            //Brush brush = new SolidBrush(Color.FromArgb(120, 120, 120));
            //pea.Graphics.FillRectangle(brush, board.Location.X - marge_small, board.Location.Y - marge_small, board.Width + marge_small * 2, board.Height + marge_small * 2);
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
            Size = new Size(480, 480);
            BackColor = Color.Red;
            ResizeRedraw = true;
            DoubleBuffered = true;
            color_white = new SolidBrush(Color.FromArgb(240, 217, 181));//Brushes.AntiqueWhite;
            color_black = new SolidBrush(Color.FromArgb(181, 136, 99));// Brushes.SandyBrown;
            color_selected = Brushes.YellowGreen;
            color_attacked = Brushes.IndianRed;

            figures = GetFigures();

            this.game = game;
        }

        protected override void OnPaint(PaintEventArgs pea)
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

                        if (piece.nCapturesLeft > 0)
                            gr.DrawImage(figures[piece.State], new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));
                        else
                            gr.DrawImage(figures[piece.State + 6], new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));
                    }
                }

            Font font = new Font("Gorga Grotesque", 10, FontStyle.Bold);
            for (int x = 0; x < game.grid.GetLength(0); x++)
            {
                if (x % 2 == 0)
                    gr.DrawString(x.ToString(), font, color_white, new Point(x * cellsize, 0));
                else
                    gr.DrawString(x.ToString(), font, color_black, new Point(x * cellsize, 0));
            }
            for (int y = 0; y < game.grid.GetLength(1); y++)
            {
                if (y % 2 == 0)
                    gr.DrawString(y.ToString(), font, color_white, new Point(0, y * cellsize));
                else
                    gr.DrawString(y.ToString(), font, color_black, new Point(0, y * cellsize));
            }

            //gr.FillRectangle(color_attacked, new Rectangle(5 * cellsize, 7 * cellsize, cellsize, cellsize));
        }

        protected override void OnMouseClick(MouseEventArgs mea)
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
            clicked = null;
            this.Invalidate(); 
        }
        public void NewGame(StreamReader sr) 
        { 
            game = new Puzzle(new Instance(sr));
            clicked = null;
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

    class MyButton : Button
    {
        public MyButton(string text, int x, int y)
        {
            this.Location = new Point(x, y);
            this.Size = new Size(119, 30);
            this.BackColor = Color.FromArgb(66, 62, 58); 
            this.ForeColor = Color.FromArgb(156, 148, 144); 
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Font = new Font("Gorga Grotesque", 10, FontStyle.Regular);
            this.Text = text;
        }
    }


}
