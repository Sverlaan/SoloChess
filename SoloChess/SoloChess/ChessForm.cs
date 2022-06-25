using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SoloChess
{
    public class ChessForm : Form
    {
        private ChessBoard board;
        private MyButton input_button, generate_button, restart_button, solve_button;
        private MyNumericUpDown nup;
        private MyComboBox cb;

        private Control[] controls;

        private int marge_small = 1;
        private int marge_big = 30;

        public ChessForm()
        {
            // Form settings
            this.Size = new Size(558, 670);
            this.Text = "Solo Chess";
            this.Icon = Properties.Resources.chesslogo;
            this.BackColor = Color.FromArgb(88, 85, 80);
            this.DoubleBuffered = true;

            // Canvas for drawing the board
            board = new ChessBoard(new Puzzle(Generator.GenValidInstance(12)));
            board.Location = new Point(marge_big + marge_small, marge_big + marge_small);
            
            // Controls
            input_button = new MyButton("Input", board.Left, board.Bottom + marge_small + marge_big, true);
            restart_button = new MyButton("Restart", input_button.Right + marge_small, input_button.Top, true);
            generate_button = new MyButton("Generate", restart_button.Right + marge_small, restart_button.Top, false);
            solve_button = new MyButton("Solve", restart_button.Right + marge_small, generate_button.Bottom + marge_small, false);
            nup = new MyNumericUpDown(generate_button.Right, generate_button.Top);
            cb = new MyComboBox(solve_button.Right, solve_button.Top);
            
            // Events
            input_button.Click += ChooseInput;
            restart_button.Click += RestartGame;
            solve_button.Click += SolveGame;
            generate_button.Click += NewGame;

            // Add controls
            controls = new Control[] { board, input_button, restart_button, solve_button, generate_button, nup, cb };
            Controls.AddRange(controls);
        }

        private void ChooseInput(object sender, EventArgs ea)
        {
            // Choose input file of instance
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Tekstfiles|*.txt";
            dialog.Title = "Open input configuration file...";

            if (dialog.ShowDialog() == DialogResult.OK)
                board.NewGame(new StreamReader(dialog.FileName));
        }

        private void NewGame(object sender, EventArgs ea){ board.NewGame((int)nup.Value); }

        private void RestartGame(object sender, EventArgs ea){ board.Restart(); }

        private void SolveGame(object sender, EventArgs ea) { board.Solve((string)cb.SelectedItem); }
    }


    public class ChessBoard : Panel
    {
        private Puzzle game;
        private Piece clicked, arrow_from, arrow_to;
        private Timer animation;
        private LinkedListNode<string> current_move, start_move;
        private int counter;
        private Brush color_black, color_white, color_selected, color_attacked;

        public ChessBoard(Puzzle game)
        {
            Size = new Size(480, 480);
            BackColor = Color.Red;
            ResizeRedraw = true;
            DoubleBuffered = true;

            color_white = new SolidBrush(Color.FromArgb(240, 217, 181));
            color_black = new SolidBrush(Color.FromArgb(181, 136, 99));
            color_selected = Brushes.YellowGreen;
            color_attacked = Brushes.IndianRed;

            animation = new Timer();
            this.game = game;
            this.animation.Tick += animation_step;
        }

        
        protected override void OnPaint(PaintEventArgs pea)
        {
            Graphics gr = pea.Graphics;
            int cellsize = this.Width / game.grid.GetLength(0);

            // Draw board cells and pieces
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

            // Draw arrow used in solution visualisation
            if (counter % 2 == 0 && arrow_from != null && arrow_to != null)
            {
                Pen p = new Pen(color_attacked, 10);
                p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                gr.DrawLine(p, (float)(arrow_from.Square.X * cellsize + 0.5 * cellsize), (float)(arrow_from.Square.Y * cellsize + 0.5 * cellsize), (float)(arrow_to.Square.X * cellsize + 0.5 * cellsize), (float)(arrow_to.Square.Y * cellsize + 0.5 * cellsize));
            }
                
            // Draw cell indices
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

            // Draw string
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
            Piece GetClickedPiece(int click_x, int click_y)
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

        public void Restart() 
        { 
            // Restart the current game
            clicked = null; 
            game.Restart(); 
            this.Invalidate(); 
        }
        public void Solve(string heur)
        {
            // Construct solution and start animation
            Restart();
            Solver solver = new Solver(game, heur);
            (LinkedList<string> sol, int bt) = solver.Solve(game);
            solver.OutputPlan(sol);
            Restart();
            StartAnimation(sol);
        }

        public void StartAnimation(LinkedList<string> solution)
        {
            // Start solution visualisation
            current_move = solution.First;
            start_move = solution.First;
            animation.Interval = 350;
            counter++;
            animation.Start();
        }
        private void animation_step(object sender, EventArgs ea)
        {
            (Piece, Piece) ParseMove(string move)
            {
                // Parse move from solution plan
                string[] line = move.Split('x');
                Piece p = game.grid[int.Parse(line[0][1].ToString()), int.Parse(line[0][2].ToString())];
                Piece q = game.grid[int.Parse(line[1][1].ToString()), int.Parse(line[1][2].ToString())];
                return (p, q);
            }

            if (counter % 2 == 0)  // Intertwine movements with pauses
            {
                // Execute move
                if (current_move != start_move)
                    game.MoveVertex(arrow_from, arrow_to);

                if (current_move != null)
                {
                    // Get next move to animate
                    (arrow_from, arrow_to) = ParseMove(current_move.Value);
                    current_move = current_move.Next;
                }
                else
                {
                    // All moves executed, stop animation
                    counter = 0;
                    animation.Stop();
                }
            }

            this.Invalidate();
            counter++;
        }

        public void NewGame(int difficulty) 
        {
            // Start new generated puzzle
            game = new Puzzle(Generator.GenValidInstance(difficulty));
            clicked = null;
            this.Invalidate(); 
        }
        public void NewGame(StreamReader sr) 
        { 
            // Start new puzzle from input file
            game = new Puzzle(new Instance(sr));
            clicked = null;
            this.Invalidate(); 
        }

        private void SetClicked(Piece p) { clicked = p; }     // Highlight clicked piece
        private void ResetClicked(){ clicked = null; } // Unhighlight
    }



    class MyNumericUpDown : NumericUpDown
    {
        public MyNumericUpDown(int x, int y)
        {
            Location = new Point(x, y);
            Size = new Size(120, 30);
            Value = 12;
            Maximum = 64;
            Minimum = 2;
            Increment = 1;
            Font = new Font("Gorga Grotesque", (float)17.5, FontStyle.Regular);
            BackColor = Color.FromArgb(66, 62, 58);
            ForeColor = Color.FromArgb(156, 148, 144);
            TextAlign = HorizontalAlignment.Center;
            BorderStyle = BorderStyle.None;
            this.Controls[1].Location = new Point(0, 0);
        }
    }


    class MyButton : Button
    {
        public MyButton(string text, int x, int y, bool big)
        {
            this.Location = new Point(x, y);
            if (big)
                this.Size = new Size(119, 61);
            else
                this.Size = new Size(119, 30);
            this.BackColor = Color.FromArgb(66, 62, 58); 
            this.ForeColor = Color.FromArgb(156, 148, 144); 
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Font = new Font("Gorga Grotesque", 11, FontStyle.Regular);
            this.Text = text;
        }
    }


    class MyComboBox : ComboBox
    {
        public MyComboBox(int x, int y)
        {
            Location = new Point(x-1, y-1);
            Size = new Size(121, 30);
            FlatStyle = FlatStyle.Popup;
            Font = new Font("Gorga Grotesque", 12, FontStyle.Regular);
            BackColor = Color.FromArgb(66, 62, 58);
            ForeColor = Color.FromArgb(156, 148, 144);

            this.Items.Add("Random");
            this.Items.Add("Rank");
            this.Items.Add("Attack");
            this.Items.Add("Center");
            this.SelectedItem = "Random";

            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += cbxDesign_DrawItem;
            this.ItemHeight = 26;

            this.Region = new Region(new Rectangle(1, 1, this.Width - 1, this.Height - 2));
        }


        // Containing code from user Martin Braun on StackOverflow.com
        // see: https://stackoverflow.com/questions/11817062/align-text-in-combobox
        private void cbxDesign_DrawItem(object sender, DrawItemEventArgs e)
        {
            // By using Sender, one method could handle multiple ComboBoxes
            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                // Always draw the background
                e.DrawBackground();

                // Drawing one of the items?
                if (e.Index >= 0)
                {
                    // Set the string alignment.  Choices are Center, Near and Far
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    // Set the Brush to ComboBox ForeColor to maintain any ComboBox color settings
                    // Assumes Brush is solid
                    Brush brush = new SolidBrush(cbx.ForeColor);

                    // If drawing highlighted selection, change brush
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        brush = SystemBrushes.HighlightText;

                    // Draw the string
                    e.Graphics.DrawString(cbx.Items[e.Index].ToString(), cbx.Font, brush, e.Bounds, sf);
                }
            }
        }
    }


}
