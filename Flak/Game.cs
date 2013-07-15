
using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

namespace Flak
{
    class Game : GameWindow
    {
        public SpriteBatch Spritebatch { get; set; }

        private GameState GameState { get; set; }

        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "Flak")
        {
            VSync = VSyncMode.On;
            this.WindowBorder = OpenTK.WindowBorder.Fixed;
            GL.Viewport(0, 0, 800, 600);
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            Spritebatch = new SpriteBatch();

            GameState = new TitleState(this);

            AudioManager.Initialize();
            AudioManager.Manager().PlayMusic();
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            //reset the viewport
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            //set up the projection matrix
            Spritebatch.ConfigureProjection();
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            GameState.Update();
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GameState.Draw();


            SwapBuffers();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }

        public void SwitchState(GameState state)
        {
            GameState = state;
        }
    }
}