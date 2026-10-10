using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGLRectangle
{
    //window creation 
    class Program : GameWindow
    {
        float rectangleX = 0.0f;
        float step = 0.1f;

        public Program()
            : base(500, 500, GraphicsMode.Default, "OpenGL Rectangle")
        {
        }
        //prepare environment
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //eraser color
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

            GL.Translate(rectangleX, 0.0f, 0.0f);

            GL.Color3(1.0f, 0.0f, 0.0f);
            //drawing the shape 
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex2(-0.5f, 0.3f);
            GL.Vertex2(0.5f, 0.3f);
            GL.Vertex2(0.5f, -0.3f);
            GL.Vertex2(-0.5f, -0.3f);

            GL.End();

            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.A)
            {
                rectangleX -= step;
            }
            else if (e.Key == Key.D)
            {
                rectangleX += step;
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