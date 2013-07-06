using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;   // Required for ArrayList
namespace SimpleDrawingProgram
{
    public partial class Form1 : Form
    {
        // Initialize components ===================================================================
        public Form1()
        {
            InitializeComponent();
            // Set List Boxes to default selection -------------------------------------------------
            PenColorListBox.SelectedIndex = 0;
            FillColorListBox.SelectedIndex = 0;
            PenWidthListBox.SelectedIndex = 0;
        }

        // Initialize variables and objects ========================================================
        private int Count = 0;                              // Click counter
        private ArrayList GraphicsObject = new ArrayList(); // Stores each graphic object
        private PointF click1;                              // Coordinates of the first click
        private PointF click2;                              // COordinates of the second click
        
        // Mouse Click Program (Paintable Panel) ===================================================
        private void MouseClick_CaptureCoordinates(object sender, MouseEventArgs e)
        {   // Note: Left and Right Clicks do the same thing
            if (Count == 0)                     // If first click
            {
                click1 = new PointF(e.X, e.Y);  // Store first click coordinates
                Count = 1;                      // Count first click
            }
            else                                // If not first click
            {
                click2 = new PointF(e.X, e.Y);  // Store second click coordinates
                Count = 0;                      // Reset click count

                // Draw Line -------------------------------------------------------------------
                if (drawLineButton.Checked)
                {   // Convert string to Color
                    Color myPenColor = Color.FromName(PenColorListBox.SelectedItem.ToString());
                    // Instantiate DrawLine
                    DrawLine item = new DrawLine(click1, click2, myPenColor, Convert.ToInt32(PenWidthListBox.SelectedItem));
                    // Add to GraphicsObject
                    this.GraphicsObject.Add(item);
                }

                // Draw Rectangle (at least one checkbox must be checked) ----------------------
                if (drawRectButton.Checked && (FillCheckBox.Checked || OutlineCheckBox.Checked))
                {   // Convert string to Color
                    Color myPenColor = Color.FromName(PenColorListBox.SelectedItem.ToString());
                    Color myFillCOlor = Color.FromName(FillColorListBox.SelectedItem.ToString());
                    // Instantiate DrawRect
                    DrawRect item = new DrawRect(click1, click2, myPenColor, myFillCOlor, Convert.ToInt32(PenWidthListBox.SelectedItem), OutlineCheckBox.Checked, FillCheckBox.Checked);
                    // Add to GraphicsObject
                    this.GraphicsObject.Add(item);
                }

                // Draw Ellipse (at least one checkbox must be selected) -----------------------
                if (drawEllipseButton.Checked && (FillCheckBox.Checked || OutlineCheckBox.Checked))
                {   // Convert string to Color
                    Color myPenColor = Color.FromName(PenColorListBox.SelectedItem.ToString());
                    Color myFillCOlor = Color.FromName(FillColorListBox.SelectedItem.ToString());
                    // Instantiate DrawEllipse
                    DrawEllipse item = new DrawEllipse(click1, click2, myPenColor, myFillCOlor, Convert.ToInt32(PenWidthListBox.SelectedItem), OutlineCheckBox.Checked, FillCheckBox.Checked);
                    // Add to GraphicsObject
                    this.GraphicsObject.Add(item);
                }

                // Draw Text (must have at least 1 character in textbox)------------------------
                if (drawTextButton.Checked && (drawLineTextBox.TextLength > 0))
                {   // Convert string to Color
                    Color myPenColor = Color.FromName(PenColorListBox.SelectedItem.ToString());
                    // Instantiate DrawText
                    DrawText item = new DrawText(click1, click2, myPenColor, drawLineTextBox.Text);
                    // Add to GraphicsObject
                    this.GraphicsObject.Add(item);
                }
                this.paintPanel.Invalidate();   // Refresh paintPanel
            }
        }

        // Paint Panel with GraphicsObject =========================================================
        private void paintPanel_Paint(object sender, PaintEventArgs e)
        {   // Instantiate Graphics Object
            Graphics g = e.Graphics;
            // Draw each Graphics Object
            foreach (DrawBase item in GraphicsObject)
            {
                item.Draw(g);
            }
        }

        // Undo last drawn object ==================================================================
        private void UndoDraw(object sender, EventArgs e)
        {   // Crash Prevention: make sure there is at least 1 item in the GraphicsObject
            if (GraphicsObject.Count > 0)
            {
                GraphicsObject.RemoveAt(GraphicsObject.Count - 1);  // Remove last item
                this.paintPanel.Invalidate();                       // Refresh paintPanel
            }
        }

        // Clear all drawn objects =================================================================
        private void ClearDraw(object sender, EventArgs e)
        {
            GraphicsObject.Clear();         // Clear all Graphics Objects
            this.paintPanel.Invalidate();   // Refresh paintPanel
        }

        // Exit Application ========================================================================
        private void ExitProgram(object sender, EventArgs e)
        {
            Application.Exit();             // Exits Application
        }
    }
    
    // DrawBase: base class ========================================================================
    public class DrawBase
    {
        // Initialize properties -------------------------------------------------------------------
        protected PointF p1, p2;                    // Coordinates of first and second clicks
        protected Color PenColor, FillColor;        // Pen and Fill colors
        protected int PenWidth;                     // Pen width
        protected bool OutlineChecked, FillChecked; // See if Outline or Fill checkboxes are checked
        protected String TypedText;                 // Text from text box
        protected Pen myPen;                        // Pen containing Pen Color and Pen Width
        protected SolidBrush myBrush;               // Brush containing either Pen or Fill Colors
        protected Font myFont = new Font("Microsoft Sans Serif", 33/4); // Default font
        protected RectangleF myRect;                // Rectangle containing points 1 and 2

        // Constructors ----------------------------------------------------------------------------
        public DrawBase()                           // Default
        {
        }
        // Used for DrawLin
        public DrawBase(PointF p1, PointF p2, Color PenColor, int PenWidth)
        {
            this.p1 = p1;                           // Store Point1
            this.p2 = p2;                           // Store Point2
            this.PenColor = PenColor;               // Store PenColor
            this.PenWidth = PenWidth;               // Store PenWidth
            Pen myPen = new Pen(PenColor);          // Instantiate new Pen using PenColor
            myPen.Width = PenWidth;                 // And PenWidth
            this.myPen = myPen;                     // Store myPen
        }
        // Used for DrawRect and DrawEllipse
        public DrawBase(PointF p1, PointF p2, Color PenColor, Color FillColor, int PenWidth, bool OutlineChecked, bool FillChecked)
        {
            this.p1 = p1;                           // Store Point1
            this.p2 = p2;                           // Store Point2
            if (p2.X < p1.X)                        // If X2 < X1
            {
                this.p2.X = p1.X;                   // Swap X1 and X2
                this.p1.X = p2.X;
            }
            if (p2.Y < p1.Y)                        // If Y2 < Y1
            {
                this.p2.Y = p1.Y;                   // Swap Y1 and Y2
                this.p1.Y = p2.Y;
            }
                                                    // Instantiate new Rect using Point1 and Point2
            RectangleF myRect = new RectangleF(this.p1.X, this.p1.Y, this.p2.X - this.p1.X, this.p2.Y - this.p1.Y);
            this.myRect = myRect;                   // Store myRect
            this.OutlineChecked = OutlineChecked;   // Check if Outline checkbox is checked and store
            this.FillChecked = FillChecked;         // Check if Fill checkbox is checked and store
            if (OutlineChecked)                     // If Outline is checked
            {
                this.PenColor = PenColor;           // Store PenColor
                this.PenWidth = PenWidth;           // Store PenWidth
                Pen myPen = new Pen(PenColor);      // Instantiate new Pen using PenColor
                myPen.Width = PenWidth;             // And PenWidth
                this.myPen = myPen;                 // Store myPen
            }
            if (FillChecked)                        // If Fill is checked
            {
                this.FillColor = FillColor;         // Store FillColor
                SolidBrush myBrush = new SolidBrush(FillColor); // Instantiate SolidBrush using FillColor
                this.myBrush = myBrush;             // Store myBrush
            }
        }
        // Used for DrawText
        public DrawBase(PointF p1, PointF p2, Color PenColor, String TypedText)
        {
            this.p1 = p1;                           // Store Point1
            this.p2 = p2;                           // Store Point2
            if (p2.X < p1.X)                        // If X2 < X1
            {
                this.p2.X = p1.X;                   // Swap X1 and X2
                this.p1.X = p2.X;
            }
            if (p2.Y < p1.Y)                        // If Y2 < Y1
            {
                this.p2.Y = p1.Y;                   // Swap Y1 and Y2
                this.p1.Y = p2.Y;
            }
                                                    // Instantiate new Rect using Point1 and Point2
            RectangleF myRect = new RectangleF(this.p1.X, this.p1.Y, this.p2.X - this.p1.X, this.p2.Y - this.p1.Y);
            this.myRect = myRect;                   // Store myRect
            SolidBrush myBrush = new SolidBrush(PenColor);  // Instantiate SolidBrush with PenColor
            this.myBrush = myBrush;                 // Store myBrush
            this.TypedText = TypedText;             // Store TypedText
        }

        // Methods ---------------------------------------------------------------------------------
        public virtual void Draw(Graphics g)        // Default virtual method
        {
        }
    }

    // DrawLine: derived class =====================================================================
    public class DrawLine : DrawBase
    {
        // Derived class constructor
        public DrawLine(PointF p1, PointF p2, Color PenColor, int PenWidth)
            : base(p1, p2, PenColor, PenWidth)
        {
        }
        // Derived class Draw method override
        public override void Draw(Graphics g)
        {
            g.DrawLine(myPen, p1, p2);              // Draw a line
        }
    }

    // DrawRect: derived class =====================================================================
    public class DrawRect : DrawBase
    {
        // Derived class constructor
        public DrawRect(PointF p1, PointF p2, Color PenColor, Color FillColor, int PenWidth, bool OutlineChecked, bool FillChecked)
            : base(p1, p2, PenColor, FillColor, PenWidth, OutlineChecked, FillChecked)
        {
        }
        // Derived class Draw method override
        public override void Draw(Graphics g)
        {   // Note: order drawn is important!
            if (FillChecked)                        // If Fill is checked
            {
                g.FillRectangle(myBrush, myRect);   // Draw a filled rectangle
            }
            if (OutlineChecked)                     // If Outline is checked
            {                                       // Draw an outlined rectangle
                g.DrawRectangle(myPen, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
            }
        }
    }

    // DrawEllipse: derived class ==================================================================
    public class DrawEllipse : DrawBase
    {
        // Derived class constructor
        public DrawEllipse(PointF p1, PointF p2, Color PenColor, Color FillColor, int PenWidth, bool OutlineChecked, bool FillChecked)
            : base(p1, p2, PenColor, FillColor, PenWidth, OutlineChecked, FillChecked)
        {
        }
        // Derived class Draw method override
        public override void Draw(Graphics g)
        {   // Note: order drawn is importatn!
            if (FillChecked)                        // If Fill is checked
            {
                g.FillEllipse(myBrush, myRect);     // Draw a filled ellipse
            }
            if (OutlineChecked)                     // If Outline is checked
            {
                g.DrawEllipse(myPen, myRect);       // Draw an outlined ellipse
            }
        }
    }

    // DrawText: derived class =====================================================================
    public class DrawText : DrawBase
    {
        // Derived class constructor
        public DrawText(PointF p1, PointF p2, Color PenColor, String TypedText) 
            : base(p1, p2, PenColor, TypedText)
        {
        }
        // Derived class Draw method override
        public override void Draw(Graphics g)
        {
            g.DrawString(TypedText, myFont, myBrush, myRect);   // Draw the string in the text box
        }
    }
}
