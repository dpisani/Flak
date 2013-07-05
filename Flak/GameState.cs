using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flak
{
    abstract class GameState
    {
        public Game mainGame;

        public GameState(Game mainWindow)
        {
            mainGame = mainWindow;
        }

        public abstract void Draw();

        public abstract void Update();
    }
}
