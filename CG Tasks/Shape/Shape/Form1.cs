using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Shape
{
    public partial class Form1 : Form
    {
        private const int N = 72;            // number of points that make up the circle
        private const float Radius = 60f;

        // points[0..N-1] = circle outline, points[N] = marker tip, points[N+1] = center
        private readonly PointF[] original = new PointF[N + 2];
        private PointF[] current;

        private const float Step = 20f;          // pixels per move
        private const float ScaleFactor = 1.1f;  // scale per key press
        private const float Angle = 10f;         // degrees per rotate
        private const float ShearAmount = 0.15f;

        private readonly Panel canvas = new DoubleBufferedPanel();
        private readonly Label help = new Label();

        public Form1()
        {
            InitializeComponent();

            Text = "2D Transformations - use the keyboard";
            ClientSize = new Size(900, 600);

            // Build the circle as a polygon so it can be sheared into an ellipse
            for (int i = 0; i < N; i++)
            {
                double a = 2 * Math.PI * i / N;
                original[i] = new PointF((float)(Radius * Math.Cos(a)), (float)(Radius * Math.Sin(a)));
            }
            // Marker: a radius line pointing up-right, so rotation and reflection are visible
            double m = -Math.PI / 4;
            original[N] = new PointF((float)(Radius * Math.Cos(m)), (float)(Radius * Math.Sin(m)));
            original[N + 1] = new PointF(0, 0);

            help.Dock = DockStyle.Top;
            help.Height = 60;
            help.Padding = new Padding(6);
            help.BackColor = Color.WhiteSmoke;
            help.Text =
                "Arrows: move   |   + / - : scale   |   E / Q : rotate right / left (Shift = reverse)\r\n" +
                "X : reflect up/down   |   Y : reflect left/right   |   H : shear horizontal   |   V : shear vertical (Shift = reverse)   |   Space : reset";

            canvas.Dock = DockStyle.Fill;
            canvas.BackColor = Color.White;
            canvas.Paint += Canvas_Paint;
            canvas.Resize += (s, e) => ResetShape();

            Controls.Add(canvas);
            Controls.Add(help);

            ResetShape();
        }

        // Catch every key here so the arrow keys are never swallowed by focus navigation
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            bool shift = (keyData & Keys.Shift) == Keys.Shift;
            bool handled = true;

            switch (key)
            {
                // translate
                case Keys.Up: Translate(0, -Step); break;
                case Keys.Down: Translate(0, Step); break;
                case Keys.Left: Translate(-Step, 0); break;
                case Keys.Right: Translate(Step, 0); break;

                // scale
                case Keys.Oemplus:
                case Keys.Add:
                case Keys.PageUp:
                    ApplyAboutCenter(new Matrix(ScaleFactor, 0, 0, ScaleFactor, 0, 0)); break;
                case Keys.OemMinus:
                case Keys.Subtract:
                case Keys.PageDown:
                    ApplyAboutCenter(new Matrix(1 / ScaleFactor, 0, 0, 1 / ScaleFactor, 0, 0)); break;

                // rotate
                case Keys.E: { var r = new Matrix(); r.Rotate(shift ? -Angle : Angle); ApplyAboutCenter(r); break; }
                case Keys.Q: { var r = new Matrix(); r.Rotate(shift ? Angle : -Angle); ApplyAboutCenter(r); break; }

                // reflect
                case Keys.X: ApplyAboutCenter(new Matrix(1, 0, 0, -1, 0, 0)); break;   // up/down
                case Keys.Y: ApplyAboutCenter(new Matrix(-1, 0, 0, 1, 0, 0)); break;   // left/right

                // shear
                case Keys.H: ApplyAboutCenter(new Matrix(1, 0, shift ? -ShearAmount : ShearAmount, 1, 0, 0)); break;
                case Keys.V: ApplyAboutCenter(new Matrix(1, shift ? -ShearAmount : ShearAmount, 0, 1, 0, 0)); break;

                // reset
                case Keys.Space: ResetShape(); break;

                default: handled = false; break;
            }

            if (handled)
            {
                canvas.Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ResetShape()
        {
            float cx = canvas.ClientSize.Width / 2f;
            float cy = canvas.ClientSize.Height / 2f;
            current = new PointF[original.Length];
            for (int i = 0; i < original.Length; i++)
                current[i] = new PointF(original[i].X + cx, original[i].Y + cy);
            canvas.Invalidate();
        }

        // Translation
        private void Translate(float dx, float dy)
        {
            var m = new Matrix();
            m.Translate(dx, dy);
            m.TransformPoints(current);
        }

        // Scale / rotate / reflect / shear around the circle's own center
        private void ApplyAboutCenter(Matrix op)
        {
            PointF c = current[N + 1];

            var m = new Matrix();
            m.Translate(-c.X, -c.Y, MatrixOrder.Append);   // move center to origin
            m.Multiply(op, MatrixOrder.Append);            // apply the transformation
            m.Translate(c.X, c.Y, MatrixOrder.Append);     // move back
            m.TransformPoints(current);
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float cx = canvas.Width / 2f, cy = canvas.Height / 2f;

            // reference axes
            using (var axis = new Pen(Color.LightGray))
            {
                g.DrawLine(axis, 0, cy, canvas.Width, cy);
                g.DrawLine(axis, cx, 0, cx, canvas.Height);
            }

            // the original circle (dashed) for comparison
            using (var dash = new Pen(Color.Gray) { DashStyle = DashStyle.Dash })
                g.DrawEllipse(dash, cx - Radius, cy - Radius, Radius * 2, Radius * 2);

            // the transformed circle
            var outline = new PointF[N];
            Array.Copy(current, outline, N);

            using (var fill = new SolidBrush(Color.FromArgb(150, Color.SteelBlue)))
            using (var pen = new Pen(Color.DarkBlue, 2))
            using (var markerPen = new Pen(Color.Crimson, 3))
            {
                g.FillPolygon(fill, outline);
                g.DrawPolygon(pen, outline);

                // marker line from the center to the edge, with a dot at the tip
                g.DrawLine(markerPen, current[N + 1], current[N]);
                g.FillEllipse(Brushes.Crimson, current[N].X - 5, current[N].Y - 5, 10, 10);
            }
        }

        private class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel() { DoubleBuffered = true; ResizeRedraw = true; }
        }
    }
}