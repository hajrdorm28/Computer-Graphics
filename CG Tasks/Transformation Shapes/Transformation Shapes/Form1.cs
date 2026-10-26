using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Transformation_Shapes
{
    public partial class Form1 : Form
    {
        // Transformation variables
        int x = 0;              // translation in X
        int y = 0;              // translation in Y
        int speed = 10;         // translation step

        float scale = 1f;       // scale factor
        float angle = 0f;       // rotation angle (degrees)
        float flipX = 1f;       // 1 = normal, -1 = reflected left/right
        float flipY = 1f;       // 1 = normal, -1 = reflected up/down
        float shearX = 0f;      // horizontal shear
        float shearY = 0f;      // vertical shear

        // Rectangle size
        const int RectWidth = 160;
        const int RectHeight = 100;

        public Form1()
        {
            InitializeComponent();

            // Allow the form to receive keyboard input
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            // Reduce flicker when redrawing
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Center of the window
            float cx = ClientSize.Width / 2f;
            float cy = ClientSize.Height / 2f;


            // Build the transformation matrix.
            // Operations are applied to the shape in reverse order of these calls:
            // shear -> scale/reflect -> rotate -> translate
            Matrix m = new Matrix();
            m.Translate(cx + x, cy + y);                           // move
            m.Rotate(angle);                                       // rotate
            m.Scale(scale * flipX, scale * flipY);                 // scale + reflect
            m.Multiply(new Matrix(1, shearY, shearX, 1, 0, 0));    // shear

            g.Transform = m;

            // Draw the rectangle around (0,0); the matrix moves/changes it
            g.FillRectangle(Brushes.Green, -RectWidth / 2, -RectHeight / 2, RectWidth, RectHeight);


            g.ResetTransform();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // ---- Translate ----
                case Keys.Right:
                    x += speed;
                    break;

                case Keys.Left:
                    x -= speed;
                    break;

                case Keys.Up:
                    y -= speed;
                    break;

                case Keys.Down:
                    y += speed;
                    break;

                // ---- Scale ----
                case Keys.Oemplus:
                case Keys.Add:
                    scale *= 1.1f;
                    break;

                case Keys.OemMinus:
                case Keys.Subtract:
                    scale /= 1.1f;
                    if (scale < 0.1f) scale = 0.1f;
                    break;

                // ---- Rotate ----
                case Keys.E:
                    angle += 10;
                    break;

                case Keys.Q:
                    angle -= 10;
                    break;

                // ---- Reflect ----
                case Keys.X:            // up/down
                    flipY = -flipY;
                    break;

                case Keys.Y:            // left/right
                    flipX = -flipX;
                    break;

                // ---- Shear (Shift = opposite direction) ----
                case Keys.H:
                    shearX += e.Shift ? -0.1f : 0.1f;
                    break;

                case Keys.V:
                    shearY += e.Shift ? -0.1f : 0.1f;
                    break;

                // ---- Reset ----
                case Keys.Space:
                    x = 0; y = 0;
                    scale = 1f; angle = 0f;
                    flipX = 1f; flipY = 1f;
                    shearX = 0f; shearY = 0f;
                    break;
            }

            // Prevent arrow keys / space from doing anything else
            e.Handled = true;

            // Redraw the shapes
            Invalidate();
        }
    }
}