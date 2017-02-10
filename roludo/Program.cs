using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace roludo
{
   public class game : GameWindow
    {
        [STAThread]
        public static void Main()
        {

            using (var game = new game())
            {
                game.Run(60.0, 60.0);
            }
        }
       
		public static Vector2 getMouse()
		{
			MouseState Mus = OpenTK.Input.Mouse.GetState ();
			Vector2 center = new Vector2{ X = Mus.X, Y = Mus.Y };
			if (Globals.IsLinux) 
			{
				center.Y = 100/Globals.Height - center.Y;
			}
			return center;

		}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "testo";
            CursorVisible = false;
            WindowBorder = WindowBorder.Hidden;
            WindowState = WindowState.Fullscreen;
            
            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

            //enable 2D textures, layers and alpha
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 100, 0, 100, -1, 1);

            Globals.Height = 100.0f / Height;
            Globals.Width = 100.0f / Width;

            Scenographer.populatScenes();

            Scenographer.activeScene.onLoad();
            //load manager? main menu?

        }

        protected override void OnUnload(EventArgs e)
        {

        }

        protected override void OnResize(EventArgs e)
        {

            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 100, 0, 100, -1, 1);

            Globals.Height = 100.0f / Height;
            Globals.Width = 100.0f / Width;
            
            Scenographer.activeScene.onLoad();
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Scenographer.activeScene.onUpdate(e.Time);
            if (Globals.ExitFlag) { Exit(); }
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            Scenographer.activeScene.onRender();

            SwapBuffers();
        }
    }
}

