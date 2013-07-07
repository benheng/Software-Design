using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SlideShowViewer
{
    public partial class Form1 : Form
    {
        // Initialize Components ///////////////////////////////////////////////////////////////////
        public Form1()
        {
            InitializeComponent();
        }

        // Add Files ///////////////////////////////////////////////////////////////////////////////
        private void OpenAddFileDialog(object sender, EventArgs e)
        {
            if (AddFileDialog.ShowDialog(this) == DialogResult.OK)              // Open AddFileDialog >> Click OK
                foreach (string filename in AddFileDialog.FileNames)            // Iterate through .Filename array
                    ImageListBox.Items.Add(filename);                           // Copy each filepath to ImageListBox items
        }

        // Delete Files ////////////////////////////////////////////////////////////////////////////
        private void DeleteSelectedFiles(object sender, EventArgs e)
        {
            for (int i = ImageListBox.SelectedIndices.Count - 1; i >= 0; i--)   // Iterate through selected items
                ImageListBox.Items.RemoveAt(ImageListBox.SelectedIndices[i]);   // Remove items
        }

        // Show Images /////////////////////////////////////////////////////////////////////////////
        private void StartSlideShow(object sender, EventArgs e)
        {
            try
            {
                if (ImageListBox.Items.Count < 1)                               // Check if any filepaths are loaded
                    throw new Exception("No images to show.");                  // Throw a generic modified exception
                int interval = Convert.ToInt32(IntervalTextBox.Text);           // Convert interval from string to int32
                if (interval < 1) throw new FormatException();                  // Check if interval is > 0
                FullScreenDisplay Viewer = new FullScreenDisplay();             // Instantiate form (Viewer)
                Viewer.Interval = interval;                                     // Pass intervals to Viewer
                Viewer.FileNames = ImageListBox.Items.Cast<string>().ToList();  // Pass filenames to Viewer as a list of strings
                Viewer.StartSlideshow();                                        // Start slideshow
            }
            catch (FormatException)                                             // Format exception handler
            {
                MessageBox.Show("Please enter an integer time interval > 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)                                                // Generic exception hanlder
            {
                MessageBox.Show(ex.Message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Open Pix ////////////////////////////////////////////////////////////////////////////////
        private void OpenOpenPixDialog(object sender, EventArgs e)
        {
            if (OpenPixDialog.ShowDialog(this) == DialogResult.OK)              // Open OpenPixDialog >> Click OK
            {
                ImageListBox.Items.Clear();                                     // Clear ImageListBox
                using (StreamReader sr = new StreamReader(OpenPixDialog.FileName))  // Open StreamReader
                {
                    string line;                                                // Create a sting to hold each line
                    while ((line = sr.ReadLine()) != null)                      // Read each line, if it's not empty
                        ImageListBox.Items.Add(line);                           // Add each line to the ImageListBox
                }
            }
        }

        // Save Pix ////////////////////////////////////////////////////////////////////////////////
        private void OpenSavePixDialog(object sender, EventArgs e)
        {
            if (ImageListBox.Items.Count == 0)                                  // Check if there are files to save
                MessageBox.Show("No file names to save.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else                                                                // If yes
            {
                SavePixDialog.ShowDialog(this);                                 // Open SavePixDialog
                if (SavePixDialog.FileName != "")                               // >> Save (will skip if Cancel)
                {
                    List<string> list = ImageListBox.Items.Cast<string>().ToList(); // Convert ListBox.Items to a list of stings
                    StreamWriter sw = File.CreateText(SavePixDialog.FileName);  // Open StreamWriter
                    foreach (string line in list)                               // Iterate through list
                        sw.WriteLine(line);                                     // Write each line to the save file
                    sw.Close();                                                 // Close the StreamWriter
                }
            }
        }

        // Exit Program ////////////////////////////////////////////////////////////////////////////
        private void ExitProgram(object sender, EventArgs e)
        {
            Application.Exit(); // Exit program
        }
    }
}
