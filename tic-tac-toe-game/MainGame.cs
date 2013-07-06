using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace tic_tac_toe
{
    public partial class Form1 : Form
    {
        // Initialize playing field ////////////////////////////////////////////////////////////////
        private const float clientSize = 100;                       // size of the world coordinates space
        private const float lineLength = clientSize * 0.8f;         // length of the gridlines
        private const float block = lineLength / 3;                 // width and height of cell
        private const float offset = clientSize * 0.1f;             // position of the upper left corner of the board
        private const float delta = offset / 2;                     // block padding of X or O
        private float scale;                                        // current scale factor
        // Initialize Game Engine and Grid /////////////////////////////////////////////////////////
        private GameEngine GameEngine = new GameEngine();           // Game Engine
        private CellSelection[,] grid = new CellSelection[3,3];     // Game Grid
        private int[] WinningPaths;                                 // Winning Paths
        // Initilialize components /////////////////////////////////////////////////////////////////
        public Form1()
        {
            InitializeComponent();                                  // Initialize Components
            ResizeRedraw = true;                                    // Redraw on resize
            GameEngine.GameOver += GE_GameOver;                     // Attach event handlers
            GameEngine.GridUpdated += GE_GridUpdated;
            GameEngine.InvalidMove += GE_InvalidMove;
        }
        // OnPaint events //////////////////////////////////////////////////////////////////////////
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;            // Create a Graphics object
            ApplyTransform(g);                  // Apply the appropriate transform
            DrawBoard(g);                       // Draw the game board
            for (int i = 0; i < 3; ++i)         // Cycle through the rows
                for (int j = 0; j < 3; ++j)     // cycle through the columns
                {
                    if (grid[i, j] == CellSelection.O) DrawO(i, j, g);      // draw O's
                    else if (grid[i, j] == CellSelection.X) DrawX(i, j, g); // draw X's
                }
            WinningPaths = GameEngine.GameMatrix;                           // Copy GameMatrix into WinningPaths
            for (int i = 0; i < WinningPaths.Length; i++)                   // Iterate through WinningPaths
                if (WinningPaths[i] == 3 | WinningPaths[i] == -3)           // WinningPaths are either 3 or -3
                    DrawWinPath(i, g);                                      // Draw all winning paths
        }
        // Apply transform /////////////////////////////////////////////////////////////////////////
        private void ApplyTransform(Graphics g)
        {
            scale = Math.Min(ClientRectangle.Width / clientSize, ClientRectangle.Height / clientSize);
            if (scale == 0f) return;
            g.ScaleTransform(scale, scale);
            g.TranslateTransform(offset, 5f + offset);      // the Y transform is + 5 to accommodate for the menu strip
        }
        // Draw board //////////////////////////////////////////////////////////////////////////////
        private void DrawBoard(Graphics g)
        {
            g.DrawLine(Pens.Black, block, 0, block, lineLength);            // 1st vertical line
            g.DrawLine(Pens.Black, 2 * block, 0, 2 * block, lineLength);    // 2nd vertical line
            g.DrawLine(Pens.Black, 0, block, lineLength, block);            // 1st horizontal line
            g.DrawLine(Pens.Black, 0, 2 * block, lineLength, 2 * block);    // 2nd horizontal line
        }
        // Draw X //////////////////////////////////////////////////////////////////////////////////
        private void DrawX(int i, int j, Graphics g)
        {
            g.DrawLine(Pens.Black, i * block + delta, j * block + delta, (i * block) + block - delta, (j * block) + block - delta);
            g.DrawLine(Pens.Black, (i * block) + block - delta, j * block + delta, (i * block) + delta, (j * block) + block - delta);
        }
        // Draw O //////////////////////////////////////////////////////////////////////////////////
        private void DrawO(int i, int j, Graphics g)
        {
            g.DrawEllipse(Pens.Black, i * block + delta, j * block + delta, block - 2 * delta, block - 2 * delta);
        }
        // Draw winning path ///////////////////////////////////////////////////////////////////////
        private void DrawWinPath(int i, Graphics g)
        {
            switch (i)
            {
                case 0: g.DrawLine(Pens.Red, 0, 0, lineLength, lineLength);                                         // Diagonal \
                    break;
                case 1: g.DrawLine(Pens.Red, 0, block / 2, lineLength, block / 2);                                  // Top Horizontal
                    break;
                case 2: g.DrawLine(Pens.Red, 0, (block * 2) - (block / 2), lineLength, (block * 2) - (block / 2));  // Mid Horizontal
                    break;
                case 3: g.DrawLine(Pens.Red, 0, (block * 3) - (block / 2), lineLength, (block * 3) - (block / 2));  // Bot Horizontal
                    break;
                case 4: g.DrawLine(Pens.Red, block / 2, 0, block / 2, lineLength);                                  // Left Vertical
                    break;
                case 5: g.DrawLine(Pens.Red, (block * 2) - (block / 2), 0, (block * 2) - (block / 2), lineLength);  // Mid Vertical
                    break;
                case 6: g.DrawLine(Pens.Red, (block * 3) - (block / 2), 0, (block * 3) - (block / 2), lineLength);  // Right Vertical
                    break;
                case 7: g.DrawLine(Pens.Red, 0, lineLength, lineLength, 0);                                         // Diagonal /
                    break;
            }
        }
        // Mousedown event /////////////////////////////////////////////////////////////////////////
        private void Player_Click(object sender, MouseEventArgs e)
        {
            Graphics g = CreateGraphics();                                      // Create a graphics object
            ApplyTransform(g);                                                  // Apply the appropriate transform
            PointF[] p = { new Point(e.X, e.Y) };                               // Create a new PointF variable
            g.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, p);// Transform the point coordinates
            if (p[0].X < 0 || p[0].Y < 0) return;                               // Escape if outside lower bounds
            int i = (int)(p[0].X / block);                                      // Calculate Cell Row (int)
            int j = (int)(p[0].Y / block);                                      // Calculate Cell Column (int)
            if (i > 2 || j > 2) return;                                         // Escape if outside upper bounds
            if (e.Button == MouseButtons.Left)                                  // Left click
                if (GameEngine.ValidMove(i, j))                                 // Proceed if PlayerMove is valid
                {
                    GameEngine.PlayerMove(i, j);                                // User makes a valid move
                    computerStartsToolStripMenuItem.Enabled = false;            // Disable ComputerFirst option
                }
            if (GameEngine.ComputersTurn)                                       // Make sure it's the computer's turn
                GameEngine.ComputerMove();                                      // Then execute computer's move
        }
        // Reset game //////////////////////////////////////////////////////////////////////////////
        private void NewGame(object sender, EventArgs e)
        {
            GameEngine.Reset();                                                 // Reset the game
            computerStartsToolStripMenuItem.Enabled = true;                     // Enable computer start option
        }
        // Computer goes first /////////////////////////////////////////////////////////////////////
        private void SetComputerFirst(object sender, EventArgs e)
        {
            GameEngine.ComputerMove();                                          // Computer goes first
            computerStartsToolStripMenuItem.Enabled = false;                    // Disable computer starts option
        }
        // GridUpdated event handler ///////////////////////////////////////////////////////////////
        private void GE_GridUpdated(object sender, EventArgs e)
        {
            grid = GameEngine.grid;                                             // Update the game grid
            this.Invalidate();                                                  // Redraw
        }
        // GameOver event handler //////////////////////////////////////////////////////////////////
        private void GE_GameOver(object sender, EventArgs e)
        {
            if (GameEngine.Winner == "Player")                                  // Player wins
                MessageBox.Show("You win!!!");                                  // Display winning message
            else if (GameEngine.Winner == "Computer")                           // Computer wins
                MessageBox.Show("You lose...");                                 // Display losing message
            else if (GameEngine.Winner == "Draw")                               // Draw game
                MessageBox.Show("Tied game.");                                  // Display draw message
        }
        // InvalidMove event handler ///////////////////////////////////////////////////////////////
        private void GE_InvalidMove(object sender, InvalidMoveArgs e)
        {
            if (e.Cause == "Game Over")                                         // Click after game over
                MessageBox.Show("You cannot make any more moves after the game is over. \nTry starting a new game.");
            else if (e.Cause == "Occupied")                                     // Click after space occupied
                MessageBox.Show("That space is already occupied. \nTry making another move.");
        }
    }
}
