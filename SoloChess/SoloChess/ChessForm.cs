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
        MyButton input_button, generate_button, restart_button, solve_button;
        NumericUpDown nup;

        int marge_small = 1;
        int marge_big = 30;


        public ChessForm()
        {
            // Form settings
            this.Size = new Size(558, 650);
            this.Text = "Solo Chess";
            //this.Icon = Properties.Resources.logo;
            this.BackColor = Color.FromArgb(88, 85, 80);
            this.DoubleBuffered = true;

            // Canvas for drawing the board
            board = new ChessBoard(new Puzzle(Generator.GenValidInstance(5)));
            board.Location = new Point(marge_big + marge_small, marge_big + marge_small);
            
            input_button = new MyButton("Input", board.Left, board.Bottom + marge_small + marge_big);
            restart_button = new MyButton("Restart", input_button.Right + marge_small, input_button.Top);
            solve_button = new MyButton("Solve", restart_button.Right + marge_small, restart_button.Top);
            generate_button = new MyButton("Generate", solve_button.Right + marge_small, solve_button.Top);
            nup = new NumericUpDown
            {
                Location = new Point(generate_button.Right + marge_small, generate_button.Top),
                Size = new Size(96, 200),
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
            solve_button.Click += SolveGame;
            generate_button.Click += NewGame;

            // Add controls
            Controls.AddRange(new Control[] { board, input_button, restart_button, solve_button, generate_button, nup });
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

        public void SolveGame(object sender, EventArgs ea) { board.Solve(); }
    }


    public class ChessBoard : Panel
    {
        private Puzzle game;
        Piece clicked;
        Piece sol_from, sol_to;
        Timer timer;
        LinkedListNode<string> current_move, start_move;
        int counter;

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
            timer = new Timer();
            this.game = game;
            this.timer.Tick += timer_tick;
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
                        if (piece == clicked)
                            gr.FillRectangle(color_selected, x * cellsize, y * cellsize, cellsize, cellsize);

                        if ((clicked != null && game.ValidMove(clicked, piece)))
                            gr.FillRectangle(color_attacked, new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));

                        if (piece.CapturesLeft > 0)
                            gr.DrawImage(piece.FigureW, new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));
                        else
                            gr.DrawImage(piece.FigureB, new Rectangle(x * cellsize, y * cellsize, cellsize, cellsize));
                    }
                }

            // DrawLine From To solution visualisation
            if (counter % 2 == 0 && sol_from != null && sol_to != null)
            {
                Pen p = new Pen(color_attacked, 10);
                p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                gr.DrawLine(p, (float)(sol_from.Square.X * cellsize + 0.5 * cellsize), (float)(sol_from.Square.Y * cellsize + 0.5 * cellsize), (float)(sol_to.Square.X * cellsize + 0.5 * cellsize), (float)(sol_to.Square.Y * cellsize + 0.5 * cellsize));
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

            if (game.goal)
            {
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                gr.DrawString("Solved", new Font("Gorga Grotesque", 50, FontStyle.Regular), Brushes.Green, this.ClientRectangle, format);
            }
                
        }

        protected override void OnMouseClick(MouseEventArgs mea)
        {
            // Handle movement highlighting
            Piece piece = GetClickedPiece(mea.X, mea.Y);
            if (piece != null)
            {
                if (piece == clicked)                    
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
        public void Solve()
        {
            Solver solver = new Solver(game);
            solver.Solve(game, 3, true);
            Restart();
            StartSolutionVisualisation(solver.solution);
        }


        public void StartSolutionVisualisation(LinkedList<string> solution)
        {
            current_move = solution.First;
            start_move = solution.First;
            timer.Interval = 350;
            counter++;
            timer.Start();
        }

        (Piece, Piece) ParseMove(string move)
        {
            string[] line = move.Split('x');
            Piece p = game.grid[int.Parse(line[0][1].ToString()), int.Parse(line[0][2].ToString())];
            Piece q = game.grid[int.Parse(line[1][1].ToString()), int.Parse(line[1][2].ToString())];
            return (p, q);
        }

        private void timer_tick(object sender, EventArgs ea)
        {
            if (counter % 2 == 0)
            {
                Piece p = sol_from;
                Piece q = sol_to;

                if (current_move != start_move)
                {
                    game.MoveVertex(p, q);
                }

                if (current_move != null)
                {
                    (p, q) = ParseMove(current_move.Value);
                    sol_from = p;
                    sol_to = q;

                    current_move = current_move.Next;
                    this.Invalidate();
                }
                else
                {
                    this.Invalidate();
                    counter = 0;
                    timer.Stop();
                }
            }
            else
            {
                this.Invalidate();
            }
            counter++;

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

        private void SetClicked(Piece p) { clicked = p; }     // Highlight clicked piece
        private void ResetClicked(){ clicked = null; } // Unhighlight
    }

    class MyButton : Button
    {
        public MyButton(string text, int x, int y)
        {
            this.Location = new Point(x, y);
            this.Size = new Size(95, 30);
            this.BackColor = Color.FromArgb(66, 62, 58); 
            this.ForeColor = Color.FromArgb(156, 148, 144); 
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Font = new Font("Gorga Grotesque", 10, FontStyle.Regular);
            this.Text = text;
        }
    }


    









}
