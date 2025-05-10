using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;

namespace cg_lab3_rtx
{

    class Program
    {
        static void Main()
        {
            using (GameWindow window = new GameWindow(800, 600, GraphicsMode.Default, "Ray Tracing Lab"))
            {
                View view = new View();

                window.Load += (sender, e) => {
                    view.InitShaders();
                    view.SetupBuffers();
                    GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
                };

                window.Resize += (sender, e) => {
                    GL.Viewport(0, 0, window.Width, window.Height);
                };

                window.RenderFrame += (sender, e) => {
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    view.Draw();
                    window.SwapBuffers();
                };

                window.Run(60);
            }
        }
    }
}
