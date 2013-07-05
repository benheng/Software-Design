using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;  // Required for ArrayList
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // Create an ArrayList to place coordinates of left mouse clicks
        private ArrayList coordinates = new ArrayList();    // Array for coordinates of left clicks
        private ArrayList objectState = new ArrayList();    // Array for the graphics object state
                                                            //  "0" implies no right click
                                                            //  "1" implies a right click
        private Point r;                                    // Declare right click coordinates variable
        // Moust click program (client area)
        private void Form1_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)      // Left mouse click
            {
                Point p = new Point(e.X, e.Y);      // Store coordinates of each left click
                this.coordinates.Add(p);            // Add coordinates to ArrayList
                this.objectState.Add("0");          // Initialize the graphics state with the same
                                                    //  index as the points (tracking each graphic)
                this.Invalidate();                  // Refresh paint
            }
            if (e.Button == MouseButtons.Right)     // Right mouse click
            {
                this.r.X = e.X;                     // Store coordinates of each right click
                this.r.Y = e.Y;
                this.Invalidate();                  // Refresh paint
            }
        }
        // Paint Program
        private void Form1_Paint_1(object sender, PaintEventArgs e)
        {
            // Initialize/Reset index
            int i = 0;
            // Initialize variable size of object to paint
            const int WIDTH = 20;
            const int HEIGHT = 20;
            // Initialize graphics object
            Graphics g = e.Graphics;
            // Paint each point during every paint refresh
            foreach (Point p in this.coordinates)
            {
                // Create the bounding box to be drawn in
                RectangleF boundingBox = new RectangleF(p.X-WIDTH/2, p.Y-HEIGHT/2, WIDTH, HEIGHT);
                // Test if right clicked in the bounding box and set states
                if (boundingBox.Contains(r.X, r.Y))
                {
                    if (objectState[i] == "0")     // If no previous click:
                    {
                        g.FillEllipse(Brushes.Red, boundingBox);    // Paint red immediately
                        // Print the coordinates to the right of the object
                        // The height is not adjusted because you can use other Fonts of various heights.
                        g.DrawString("(" + p.X + ", " + p.Y + ")", Font, Brushes.Black, p.X + WIDTH / 2, p.Y);
                        objectState[i] = "1";      // Set state to "1"
                    }
                    else if (objectState[i] == "1")// Else if clicked once already:
                    {
                        objectState[i] = "2";      // Set state to "2" and do not paint
                    }
                }
                else
                {

                    // Paint each point depending on their current state
                    if (objectState[i] == "0")
                    {
                        g.FillEllipse(Brushes.Black, boundingBox);  // Black ellipse for initial click
                        // Print the coordinates to the right of the object
                        // The height is not adjusted because you can use other Fonts of various heights.
                        g.DrawString("(" + p.X + ", " + p.Y + ")", Font, Brushes.Black, p.X + WIDTH / 2, p.Y);
                    }
                    else if (objectState[i] == "1")
                    {
                        g.FillEllipse(Brushes.Red, boundingBox);    // Red ellipse for 1st click after
                        // Print the coordinates to the right of the object
                        // The height is not adjusted because you can use other Fonts of various heights.
                        g.DrawString("(" + p.X + ", " + p.Y + ")", Font, Brushes.Black, p.X + WIDTH / 2, p.Y);
                    }
                    // Do not paint object after 2nd click
                }
                
                ++i; // Increment index
            }
            this.r.X = -1;      // Reset the r values
            this.r.Y = -1;
        }
        // "Clear" Button
        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.coordinates.Clear();           // Clear all the previous clicks
            this.objectState.Clear();           // Clear all object states
            this.Invalidate();                  // Refresh paint
        }
        // "Clear" Menu Item
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.coordinates.Clear();           // Clear all the previous clicks
            this.objectState.Clear();           // Clear all object states
            this.Invalidate();                  // Refresh paint
        }
    }
}
