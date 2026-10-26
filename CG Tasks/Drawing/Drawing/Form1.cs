using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Drawing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Text = "Scene Graphics";
            ClientSize = new Size(700, 500);

            // Reduce flicker and redraw the scene when the window is resized
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // All positions are fractions of the window size,
            // so the scene scales when the window is resized.
            float w = ClientSize.Width;
            float h = ClientSize.Height;

            // The order matters: what is drawn LATER appears ON TOP.
            DrawSky(g, w, h);
            DrawSun(g, w, h);
            DrawMountains(g, w, h);

            // Three pinwheels: x, head y, pole bottom y, size
            DrawPinwheel(g, 0.12f * w, 0.17f * h, 0.52f * h, 0.12f * h);
            DrawPinwheel(g, 0.34f * w, 0.12f * h, 0.47f * h, 0.07f * h);
            DrawPinwheel(g, 0.60f * w, 0.12f * h, 0.54f * h, 0.13f * h);

            DrawRoad(g, w, h);
            DrawGrass(g, w, h);
            DrawCar(g, w, h);
        }

        //Sky
        private void DrawSky(Graphics g, float w, float h)
        {
            using (Brush b = new SolidBrush(Color.SkyBlue))
                g.FillRectangle(b, 0, 0, w, h);
        }

        //Sun
        private void DrawSun(Graphics g, float w, float h)
        {
            float cx = 0.85f * w;
            float cy = 0.14f * h;
            float r = 0.07f * w;

            // Rays
            using (Pen ray = new Pen(Color.Khaki, 3))
            {
                for (int i = 0; i < 12; i++)
                {
                    double a = i * 30 * Math.PI / 180;
                    float x1 = cx + (float)(Math.Cos(a) * r * 1.2);
                    float y1 = cy + (float)(Math.Sin(a) * r * 1.2);
                    float x2 = cx + (float)(Math.Cos(a) * r * 1.7);
                    float y2 = cy + (float)(Math.Sin(a) * r * 1.7);
                    g.DrawLine(ray, x1, y1, x2, y2);
                }
            }

            // Sun body
            using (Brush b = new SolidBrush(Color.Yellow))
                g.FillEllipse(b, cx - r, cy - r, r * 2, r * 2);
        }

        //Mountains
        private void DrawMountains(Graphics g, float w, float h)
        {
            PointF[] mountains =
            {
                new PointF(0.00f * w, 0.53f * h),
                new PointF(0.22f * w, 0.29f * h),   // small peak
                new PointF(0.30f * w, 0.43f * h),   // valley
                new PointF(0.48f * w, 0.05f * h),   // big peak
                new PointF(0.74f * w, 0.45f * h),   // valley
                new PointF(0.86f * w, 0.36f * h),   // small peak
                new PointF(1.00f * w, 0.47f * h),
                new PointF(1.00f * w, 0.60f * h),
                new PointF(0.00f * w, 0.60f * h)
            };

            using (Brush b = new SolidBrush(Color.FromArgb(0, 153, 51)))
                g.FillPolygon(b, mountains);
        }

        //Pinwheel
        private void DrawPinwheel(Graphics g, float x, float headY, float poleBottomY, float size)
        {
            // Pole
            using (Pen pole = new Pen(Color.Beige, 3))
                g.DrawLine(pole, x, headY, x, poleBottomY);

            // One blade shape, drawn 3 times with a different rotation each time
            PointF[] blade =
            {
                new PointF(0, 0),
                new PointF(size * 0.35f, -size * 0.5f),
                new PointF(0, -size),
                new PointF(-size * 0.35f, -size * 0.5f)
            };

            using (Brush red = new SolidBrush(Color.FromArgb(230, 50, 30)))
            {
                for (int i = 0; i < 3; i++)
                {
                    GraphicsState saved = g.Save();        // remember the current transform
                    g.TranslateTransform(x, headY);        // move origin to the pole top
                    g.RotateTransform(i * 90 + 20);        // rotate each blade
                    g.FillPolygon(red, blade);
                    g.Restore(saved);                      // go back to normal
                }
            }
        }

        //Road
        private void DrawRoad(Graphics g, float w, float h)
        {
            using (Brush b = new SolidBrush(Color.Gray))
                g.FillRectangle(b, 0, 0.50f * h, w, 0.25f * h);

            // White middle line
            using (Pen line = new Pen(Color.White, 5))
                g.DrawLine(line, 0, 0.63f * h, w, 0.63f * h);
        }

        //Grass
        private void DrawGrass(Graphics g, float w, float h)
        {
            using (Brush b = new SolidBrush(Color.FromArgb(0, 170, 30)))
                g.FillRectangle(b, 0, 0.75f * h, w, 0.25f * h);
        }

        //Car
        private void DrawCar(Graphics g, float w, float h)
        {
            float wheelR = 0.045f * w;

            // Wheels first, so the body covers their upper half
            DrawWheel(g, 0.45f * w, 0.66f * h, wheelR);
            DrawWheel(g, 0.62f * w, 0.66f * h, wheelR);

            using (Brush red = new SolidBrush(Color.FromArgb(230, 50, 30)))
            {
                // Body (big rectangle)
                g.FillRectangle(red, 0.39f * w, 0.50f * h, 0.30f * w, 0.14f * h);
                // Cabin (small rectangle on top)
                g.FillRectangle(red, 0.43f * w, 0.44f * h, 0.16f * w, 0.07f * h);
            }
        }

        //Wheels
        private void DrawWheel(Graphics g, float cx, float cy, float r)
        {
            g.FillEllipse(Brushes.White, cx - r, cy - r, r * 2, r * 2);

            using (Pen spoke = new Pen(Color.Black, 2))
            {
                for (int i = 0; i < 8; i++)
                {
                    double a = i * 45 * Math.PI / 180;
                    g.DrawLine(spoke, cx, cy,
                        cx + (float)(Math.Cos(a) * r),
                        cy + (float)(Math.Sin(a) * r));
                }
            }

            using (Pen rim = new Pen(Color.Black, 4))
                g.DrawEllipse(rim, cx - r, cy - r, r * 2, r * 2);
        }
    }
}