using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tic_tac_toe
{
    /** Tic-Tac-Toe Algorithm **********************************************************************
     * 
     * Sources: http://www.omnimaga.org/index.php?topic=13070.0
     *          http://en.wikipedia.org/wiki/Tictactoe (strategy used as pseudo code)
     * Explained:
     * There are 9 positions and 8 win paths. Normally, the cells would be arranged like this:
     *      a   b   c
     *      d   e   f
     *      g   h   i
     * But due to a small bug in the original tic-tac-toe example, it is actually arranged like so:
     *      a   d   g
     *      b   e   h
     *      c   f   i
     * Then we assign a matrix of win contributions to each position, essentially constructing a 3D
     * matrix. The array represents the cell's contribution to a win and can be represented like so:
     * {D1,H1,H2,H3,V1,V2,V3,D2} where D's are diagonals, H's are horizontals, and V's are verticals.
     * 
     * [a] = {1,1,0,0,1,0,0,0}
     * [b] = {0,0,1,0,1,0,0,0}
     * [c] = {0,0,0,1,1,0,0,1}
     * ... etc.
     * This matrix will remain constan. The main matrix will be initialized to {0,0,0,0,0,0,0,0} and
     * when a cell is added, we find it's position, get the corresponding array, and either add or
     * subtract it to the main matrix depending on the player. Player1 adds, and Player2 subtracts.
     * To win the game, Player1 and Player2 must score a 3 or -3 in any one of the indices
     * respectively. 
     * 
     * If Player2 is a computer, it's algorithm can be explained simply:
     *      1) Make a -3 (Win condition, high priority)
     *      2) If there is a 2 in the main matrix (Player1 is about to win), select a cell that will
     *         bring it back down to 1 (Block condition, high-med priority)
     *      3) Make as many -2's as possible (Create Forked paths/traps, med priority)
     *      4) Otherwise select a random empty cell (low priority)
     * 
     **********************************************************************************************/

    // Instantiate variables and events ////////////////////////////////////////////////////////////
    public enum CellSelection { N, O, X };                                          // Three possible cell states
    public class InvalidMoveArgs : EventArgs                                        // Invalid move args
    {
        public string Cause { get; set; }                                           // Cause
    }
    public delegate void GridUpdatedEventHandler(object sender, EventArgs e);       // Grid Update Event
    public delegate void InvalidMoveEventHandler(object sender, InvalidMoveArgs e); // Invalid Move Event
    public delegate void GameOverEventHandler(object sender, EventArgs e);          // Game Over Event
    // Game Engine /////////////////////////////////////////////////////////////////////////////////
    public class GameEngine
    {
        // Instantiate events ======================================================================
        public event InvalidMoveEventHandler InvalidMove;       // Create InvalidMove event variable
        public event GridUpdatedEventHandler GridUpdated;       // Create GridUpdated event variable
        public event GameOverEventHandler GameOver;             // Create GameOver event variable
        // Instantiate game variables ==============================================================
        public CellSelection[,] grid = new CellSelection[3, 3]; // Instantiate a 3x3 game grid
        public int[] GameMatrix;                                // Holds the current game matrix
        public static int[, ,] MoveMatrix =                     // Holds the static values assigned to each valid move
        {   {{1,1,0,0,1,0,0,0},{0,0,1,0,1,0,0,0},{0,0,0,1,1,0,0,1}},
            {{0,1,0,0,0,1,0,0},{1,0,1,0,0,1,0,1},{0,0,0,1,0,1,0,0}},
            {{0,1,0,0,0,0,1,1},{0,0,1,0,0,0,1,0},{1,0,0,1,0,0,1,0}}  };
        public bool GameIsOver;                                 // Indicates if the game is over
        public bool ComputersTurn = false;                      // Indicates if it's the computer's turn
        public string Winner { get; private set; }              // Indicates the winner of the game
        // Default Constructor =====================================================================
        public GameEngine()
        {
            Reset();                                            // Creates a new game by resetting the board
        }
        // Resets the game =========================================================================
        public void Reset()
        {
            for (int i = 0; i < 3; i++)                         // Reset the game grid
                for (int j = 0; j < 3; j++)
                    grid[i, j] = CellSelection.N;
            GameMatrix = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };  // Reset the game matrix
            Winner = null;                                      // Reset winner
            ComputersTurn = false;                              // Reset computer's turn
            GameIsOver = false;                                 // Game NOT over
            FireGridUpdated();                                  // Trigger GridUpdated event
        }
        // Checks if move is valid =================================================================
        public bool ValidMove(int row, int col)
        {
            if (GameIsOver)                                     // If the game is over, all moves are invalid
            {
                FireInvalidMove("Game Over");
                return false;
            }
            if (grid[row, col] != CellSelection.N)              // If the cell is not empty, the move is invalid
            {
                FireInvalidMove("Occupied");
                return false;
            }
            return true;                                        // Otherwise the move is valid
        }
        // Player makes a valid move ===============================================================
        public void PlayerMove(int row, int col)
        {
            grid[row, col] = CellSelection.X;                   // Set the selected block
            FireGridUpdated();                                  // Triggers GridUpdated event
            MatrixManipulation("Player", GameMatrix, row, col); // Add MoveMatrix[row,col] to GameMatrix
            UpdateGameState("Player", 3);                       // Update the state of the game
            ComputersTurn = true;                               // Computer's turn
        }
        // Computer AI =============================================================================
        public void ComputerMove()
        {
            if (GameIsOver) { return; }                                             // Return if the the game is already over
            int[] BestMove = {-1,-1,0};                                             // Stores the best move {row, col, priority}
            int[] TestMatrix = { 0, 0, 0, 0, 0, 0, 0, 0 };                          // Hold the values of next potential move
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    if (grid[row, col] == CellSelection.N)                          // Check for empty cells
                    {
                        GameMatrix.CopyTo(TestMatrix, 0);                           // Copy GameMatrix to TextMatrix
                        MatrixManipulation("Computer", TestMatrix, row, col);       // Apply move to TestMatrix
                        if (MatrixMatch(TestMatrix, -3))                            // Check winning conditions
                        { 
                            if (BestMove[2] < 10)                                   // Check priority
                            {
                                BestMove = new int[] { row, col, 10 };              // Assign BestMove = Win
                            }
                        }
                        else if (MatrixMatch(GameMatrix, 2))                        // Check for blocking conditions
                        {
                            int ForkCount = 0;                                      // Player fork count
                            int BlockCount = 0;                                     // Computer fork count
                            for (int i = 0; i < 8; i++)                             // Iterate through Matrices
                            {
                                if (GameMatrix[i].Equals(2)) ForkCount++;           // Calculate Player forked paths (potential wins)
                                if (TestMatrix[i].Equals(2)) BlockCount++;          // Calculate how many computer can block (potentially)
                            }
                            if (ForkCount != BlockCount)                            // If computer can block at least one
                                if (BestMove[2] < 9)                                // Check priority
                                {
                                    BestMove = new int[] { row, col, 9 };           // Assign BestMove = Block
                                }
                        }
                        else if (MatrixMatch(TestMatrix, -2))                       // Check for forking conditions
                        {
                            int ForkCount = 0;
                            for (int i = 0; i < 8; i++)                             // Check for the number of forks
                                if (TestMatrix[i].Equals(-2)) ForkCount++;
                            if (BestMove[2] < ForkCount*2)                          // Check priority
                            {
                                BestMove = new int[] { row, col, ForkCount*2 };     // Assign BestMove = Create Fork
                            }
                        }
                        if (BestMove[0] == -1 | BestMove[1] == -1)                  // If no move was assigned
                            BestMove = new int[] {row, col, 0};                     // Assign BestMove = next available cell
                        else if (BestMove[2] == 0)                                  // Low priority moves
                        {
                            Random rand = new Random();                             // Randomizes BestMove
                            switch (rand.Next() % (row + col))                      // Chance of switching cells increase
                            {                                                       // as you move through the cells
                                case 1: BestMove = new int[] { row, col, 0 };       // Either change it
                                    break;
                                default: break;                                     // Or keep it the same
                            }
                        }
                    }
            grid[BestMove[0], BestMove[1]] = CellSelection.O;                       // Set the selected block
            FireGridUpdated();                                                      // Trigger GridUpdated event
            MatrixManipulation("Computer", GameMatrix, BestMove[0], BestMove[1]);   // Apply move
            UpdateGameState("Computer", -3);                                        // Update the gamestate
            ComputersTurn = false;                                                  // No longer the computer's turn
        }
        // Test Match ==============================================================================
        private bool MatrixMatch(int[] matrix, int match)
        {
            for (int i = 0; i < matrix.Length; i++)                                 // Iterate through the matrix
                if (matrix[i].Equals(match)) return true;                           // Return true if value == match
            return false;
        }
        // Matrix Manipulation =====================================================================
        private void MatrixManipulation(string player, int[] matrix, int row, int col)
        {
            for (int i = 0; i < 8; i++)                                             // Iterate through 8 possible win paths
                if (player == "Player") matrix[i] += MoveMatrix[row, col, i];       // If Player, add
                else matrix[i] -= MoveMatrix[row, col, i];                          // If Computer, subtract
        }
        // Checks and updates the game status ======================================================
        private void UpdateGameState(string player, int match)
        {
            if (MatrixMatch(GameMatrix, match))         // Check winning conditions
            {
                Winner = player;                        // Sets the winner
                GameIsOver = true;                      // Game IS over
                FireGameOver();                         // Trigger GameOver event
                return;                                 // return
            }
            for (int i = 0; i < 3; i++)                 // Check draw conditions
                for (int j = 0; j < 3; j++)
                    if (grid[i, j] == CellSelection.N)  // If an empty cell is found,
                        return;                         // Escape
            Winner = "Draw";                            // Declare a Draw
            GameIsOver = true;                          // Game IS over
            FireGameOver();                             // Trigger GameOverEvent
        }
        // Trigger GameOver Event ==================================================================
        private void FireGameOver()
        {
            if (GameOver != null)
                GameOver(this, new EventArgs());
        }
        // Trigger GridUpdated Event ===============================================================
        private void FireGridUpdated()
        {
            if (GridUpdated != null)
                GridUpdated(this, new EventArgs());
        }
        // Trigger InvalidMove Event ===============================================================
        private void FireInvalidMove(string c)
        {
            if (InvalidMove != null)
            {
                var args = new InvalidMoveArgs();
                args.Cause = c;
                InvalidMove(this, args);
            }
        }
    }
}
