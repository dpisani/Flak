using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    class TitleState : GameState
    {
        static Sprite TitleSprite { get; set; }
        static SpriteBatch.RenderDetails titleRender;

        static Sprite HighScoreSprite { get; set; }
        static SpriteBatch.RenderDetails highscoreRender;

        static Sprite LatestSprite { get; set; }
        static SpriteBatch.RenderDetails latestRender;

        MainGameState backgroundGame;

        int startupTimer = 20;

        static TitleState()
        {
            Bitmap image = Flak.Properties.Resources.title;
            image.MakeTransparent(Color.Red);
            TitleSprite = new Sprite(image);
            titleRender = new SpriteBatch.RenderDetails(TitleSprite, new OpenTK.Vector2(200, 200));
            titleRender.Depth = 5;
            
            Bitmap image2 = Flak.Properties.Resources.highscore;
            image2.MakeTransparent(Color.Red);
            HighScoreSprite = new Sprite(image2);
            highscoreRender = new SpriteBatch.RenderDetails(HighScoreSprite, new OpenTK.Vector2(200, 500));
            highscoreRender.Depth = 5;

            Bitmap image3 = Flak.Properties.Resources.latest;
            image3.MakeTransparent(Color.Red);
            LatestSprite = new Sprite(image3);
            latestRender = new SpriteBatch.RenderDetails(LatestSprite, new OpenTK.Vector2(261, 460));
            latestRender.Depth = 5;
        }

        public TitleState(Game mainWindow)
            : base(mainWindow)
        {
            mainGame.Keyboard.KeyDown += Keyboard_KeyDown;
            mainGame.Mouse.ButtonDown += Mouse_ButtonDown;
        }

        void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            if (e.Button == OpenTK.Input.MouseButton.Left && startupTimer == 0)
                StartGame();
        }

        void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.Escape)
                mainGame.Exit();

            if (e.Key == OpenTK.Input.Key.Enter)
            {
                StartGame();
            }
        }

        public TitleState(MainGameState mainGame)
            : this(mainGame.mainGame)
        {
            this.backgroundGame = mainGame;
        }

        public override void Draw()
        {
            if (backgroundGame != null)
                backgroundGame.Draw();

            if (HighScoreTable.ScoreTable().HighScore > 0)
            {           
                mainGame.Spritebatch.AddSprite(highscoreRender);
                mainGame.Spritebatch.Draw();
                NumberWriter.Print(HighScoreTable.ScoreTable().HighScore, new OpenTK.Vector2(365, 507), 1, mainGame.Spritebatch);
            }

            if (HighScoreTable.ScoreTable().LatestScore > 0)
            {
                mainGame.Spritebatch.AddSprite(latestRender);
                mainGame.Spritebatch.Draw();
                NumberWriter.Print(HighScoreTable.ScoreTable().LatestScore, new OpenTK.Vector2(365, 467), 1, mainGame.Spritebatch);
            }

            mainGame.Spritebatch.AddSprite(titleRender);        
            mainGame.Spritebatch.Draw();
        }

        public override void Update()
        {
            if (backgroundGame != null)
                backgroundGame.Update();

            if (startupTimer > 0)
                startupTimer--;
        }

        void StartGame()
        {
            mainGame.Keyboard.KeyDown -= Keyboard_KeyDown;
            mainGame.Mouse.ButtonDown -= Mouse_ButtonDown;
            if (backgroundGame != null)
                backgroundGame.Dispose();
            mainGame.SwitchState(new MainGameState(mainGame));
        }
    }
}
