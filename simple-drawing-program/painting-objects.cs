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
        private ArrayList coordinates = new ArrayList();
        // Moust click program (client area)
        private void Form1_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)      // Left mouse click
            {
                Point p = new Point(e.X, e.Y);      // Store coordinates of each left click
                this.coordinates.Add(p);            // Add coordinates to ArrayList
                this.Invalidate();                  // Refresh paint
            }
            if (e.Button == MouseButtons.Right)     // Right mouse click
            {
                this.coordinates.Clear();           // Clear all the previous clicks
                this.Invalidate();                  // Refresh paint
            }
        }
        // Paint Program
        private void Form1_Paint_1(object sender, PaintEventArgs e)
        {
            // Initialize variable size of object to paint
            const int WIDTH = 20;
            const int HEIGHT = 20;
            // Initialize graphics object
            Graphics g = e.Graphics;
            // Paint each point during every paint refresh
            foreach (Point p in this.coordinates)
            {
                // Paint a filled object
                g.FillEllipse(Brushes.Black, p.X-WIDTH/2, p.Y-HEIGHT/2, WIDTH, HEIGHT);
                // Print the coordinates to the right of the object
                // The height is not adjusted because you can use other Fonts of various heights.
                g.DrawString("(" + p.X + ", " + p.Y + ")", Font, Brushes.Black, p.X + WIDTH / 2, p.Y);
            }
        }
        // "Clear" Button
        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.coordinates.Clear();           // Clear all the previous clicks
            this.Invalidate();                  // Refresh paint
        }
        // "Clear" Menu Item
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.coordinates.Clear();           // Clear all the previous clicks
            this.Invalidate();                  // Refresh paint
        }
    }
}
