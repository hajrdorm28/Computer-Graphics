using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGLTriangleExample
{
    class Program : GameWindow
    {
        float triangleX = 0.0f;
        float step = 0.1f;

        public Program()
            : base(500, 500, GraphicsMode.Default, "OpenGL Triangle")
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(-1, 1, -1, 1, -1, 1);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Translate(triangleX, 0.0f, 0.0f);

            GL.Color3(1.0f, 0.0f, 0.0f);

            GL.Begin(PrimitiveType.Triangles);

            GL.Vertex2(0.0f, 0.5f);
            GL.Vertex2(-0.5f, -0.5f);
            GL.Vertex2(0.5f, -0.5f);

            GL.End();

            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.A)
            {
                triangleX -= step;
            }
            else if (e.Key == Key.D)
            {
                triangleX += step;
            }
        }

        static void Main(string[] args)
        {
            using (Program program = new Program())
            {
                program.Run(60.0);
            }
        }
    }
}