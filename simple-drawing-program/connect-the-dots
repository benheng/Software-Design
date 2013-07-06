using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace lab03
{
    public partial class Form1 : Form
    {
        // Initialize components ===================================================================
        public Form1()
        {
            InitializeComponent();
            this.ResizeRedraw = true;                           // ResizeRedraw
            this.AutoScrollMinSize = new Size(640, 480);        // Set scrolling area to 640 x 480
        }

        // Initialize variables and objects ========================================================
        const int WIDTH  = 8;                                   // Width of object painted
        const int HEIGHT = 8;                                   // Height of object painted
        private List<Point> coordinates = new List<Point>();    // stores coordinates of left clicks

        // Mouse click program (client area) =======================================================
        private void MouseClick_CaptureCoordinates(object sender, MouseEventArgs e)
        {
            int X = this.AutoScrollPosition.X;          // Declare X-axis autoscroll position
            int Y = this.AutoScrollPosition.Y;          // Declare Y-axis autoscroll position

            if (e.Button == MouseButtons.Left)          // Left mouse click ------------------------
            {
                Point p = new Point(e.X - X, e.Y - Y);  // Store coordinates of each left click
                this.coordinates.Add(p);                // Add coordinates to the List
            }

            if (e.Button == MouseButtons.Right)         // Right mouse click -----------------------
            {
                Point r = new Point(e.X - X, e.Y - Y);  // Stores coordinates of each right click
                for (int i = coordinates.Count - 1; i >= 0; i--)    // Right click test block
                {
                    Point p = coordinates[i];           // Instantiate coordinates to test
                    // Create a bounding box equivalent to the size of the object being drawn
                    RectangleF boundingBox = new RectangleF(p.X - WIDTH / 2, p.Y - HEIGHT / 2, WIDTH, HEIGHT);
                    // Test to see if an object has been right clicked and removes it if true
                    if (boundingBox.Contains(r.X, r.Y)) coordinates.RemoveAt(i);
                }
            }

            this.Invalidate();                          // Refresh paint event
        }

        // Paint coordinates and lines =============================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            int X = this.AutoScrollPosition.X;      // Declare X-axis autoscroll position
            int Y = this.AutoScrollPosition.Y;      // Declare Y-axis autoscroll position
            Graphics g = e.Graphics;                // Initialize graphics object
            g.TranslateTransform(X, Y);             // Moves the graphics objects during scrolling

            // Main control panel is docked at the top during scrolling ----------------------------
            Point SizeOfClient = new Point(this.ClientSize);            // Get size of client area
            this.MainControlPanel.Size = new Size(SizeOfClient.X, 50);  // Set the size of the panel
            this.MainControlPanel.Location = new Point(0, 0);           // "Docks" top

            foreach (Point p in coordinates)        // Paint each coordinate -----------------------
                g.FillEllipse(Brushes.Black, p.X - (WIDTH / 2), p.Y - (HEIGHT / 2), WIDTH, HEIGHT);

            // ONLY ckConnect checked and at least 2 points created --------------------------------
            if ((ckConnect.Checked) & (!ckAll.Checked) & (coordinates.Count > 1))
            {
                // Connects all the points with a line in the order drawn + closing line
                g.DrawPolygon(Pens.Black, coordinates.ToArray());
            }

            // BOTH ckConnect and ckAll checked and at least 2 points created ----------------------
            if ((ckConnect.Checked) & (ckAll.Checked) & (coordinates.Count > 1))
            {
                for (int i = 0; i < coordinates.Count - 1; i++) // Iterate through each point once
                {
                    Point p1 = coordinates[i];          // Declare the start point
                    // Iterate through all points after start point (this reduces duplicate lines)
                    for (int j = i + 1; j < coordinates.Count; j++)
                    {
                        Point p2 = coordinates[j];      // Declare end point
                        g.DrawLine(Pens.Black, p1, p2); // Draw the line connecting them
                    }
                }
            }

            g.Dispose();
        }

        // Refresh paint event when checkboxes change ==============================================
        private void ckChanged(object sender, EventArgs e)
        {
            this.Invalidate();          // Refresh paint event
        }

        // Clear points method =====================================================================
        private void Clear_Points(object sender, EventArgs e)
        {
            this.coordinates.Clear();   // Removes all points in coordinates list
            this.Invalidate();          // Refresh paint event
        }
    }
}
