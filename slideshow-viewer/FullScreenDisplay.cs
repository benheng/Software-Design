using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlideShowViewer
{
    public partial class FullScreenDisplay : Form
    {
        // Initialize variables ////////////////////////////////////////////////////////////////////
        private Bitmap CurrentFile;                     // Current loaded bitmap
        private int CurrentIndex = 0;                   // Current index of image list
        private bool isImage;                           // Invalid image
        private float scale;                            // Full screen scaling
        private float x_offset;                         // Centers the image along the x-axis
        private float y_offset;                         // Centers the image along the y-axis
        public int Interval;                            // Slideshow interval in seconds
        public List<string> FileNames;                  // Image list

        // Initialize components ///////////////////////////////////////////////////////////////////
        public FullScreenDisplay()
        {
            InitializeComponent();
        }

        // Start slideshow /////////////////////////////////////////////////////////////////////////
        public void StartSlideshow()
        {
            IntervalTimer.Interval = Interval * 1000;   // Set interval (ms) for timer
            try                                         // Test the file
            {
                CurrentFile = new Bitmap(@FileNames[0]);// Load first image
                isImage = true;                         // Success!
            }
            catch (ArgumentException)                   // Not an image
            {
                isImage = false;                        // Failure!
            }
            finally                                     // Do this anyway
            {
                this.ShowDialog();                      // Show form (modal)
            }
        }

        // Next image //////////////////////////////////////////////////////////////////////////////
        private void TimerTick(object sender, EventArgs e)
        {
            try
            {
                IntervalTimer.Enabled = false;                      // Disable timer while loading
                CurrentIndex++;                                     // Increment CurrentIndex
                CurrentFile = new Bitmap(@FileNames[CurrentIndex]); // Load next image
                isImage = true;                                     // Success!
                this.Invalidate();                                  // Redraw Viewer
            }
            catch (ArgumentOutOfRangeException)                     // End of slideshow
            {
                this.Close();                                       // Close Viewer
            }
            catch (ArgumentException)                               // Not an image!
            {
                isImage = false;                                    // Failure!
                this.Invalidate();                                  // Redraw Viewer
            }
        }

        // Display images //////////////////////////////////////////////////////////////////////////
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;                                // Create a Graphics object
            Font myFont = new Font(FontFamily.GenericSansSerif, 24);// Create a Font object
            if (isImage)                                            // isImage?
            {
                ApplyTransform(g);                                  // Apply the appropriate transform
                                                                    // Draw image with offsets and scaling
                g.DrawImage(CurrentFile, 0f + x_offset, 0f + y_offset, CurrentFile.Width * scale, CurrentFile.Height * scale);
            }
            else                                                    // Not an image
                g.DrawString("Not an image file!", myFont, Brushes.Red, 20, 20);    // Draw note
            IntervalTimer.Enabled = true;                           // Start interval timer after drawing
        }

        // Apply transform /////////////////////////////////////////////////////////////////////////
        private void ApplyTransform(Graphics g)
        {
            float scale_width = (float)ClientSize.Width / CurrentFile.Width;    // Calculate horizontal scale
            float scale_height = (float)ClientSize.Height / CurrentFile.Height; // Calculate vertical scale
            scale = Math.Min(scale_width, scale_height);                        // Calculate image scale
            x_offset = (ClientSize.Width - (CurrentFile.Width * scale)) / 2f;   // Calculate horizontal offset
            y_offset = (ClientSize.Height - (CurrentFile.Height * scale)) / 2f; // Calculate vertical offset
        }

        // Escape fullscreen ///////////////////////////////////////////////////////////////////////
        private void Escape_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)       // If Esc key is pressed
            {
                IntervalTimer.Enabled = false;  // Disable the timer
                this.Close();                   // Close the form
            }
        }
    }
}
