using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    public class MainGameState : GameState
    {
        Player mainPlayer;
        EntityManager manager;

        public MainGameState(Game mainWindow)
            : base(mainWindow)
        {
            manager = new EntityManager();
            manager.Clear();

            mainPlayer = new Player(new Vector2(400, 300), mainGame.Keyboard, mainGame.Mouse, manager);
            manager.Add(mainPlayer);
        }

        public override void Update()
        {
            mainPlayer.Update();
            manager.Update();
        }

        public override void Draw()
        {
            manager.Draw(mainGame.Spritebatch);

            mainGame.Spritebatch.ApplyViewProjection();
            mainPlayer.DrawOverlay();
        }
    }
}
