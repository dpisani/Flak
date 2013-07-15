using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    class MainGameState : GameState
    {
        Player mainPlayer;
        EntityManager manager;

        Random random;

        int SwarmTimer { get; set; }
        int SwarmDelay { get; set; }
        const int minSwarmDelay = 30;
        const int swarmDelayDelta = 5;
        int SwarmSize { get; set; }
        const int maxSwarmSize = 10;

        public int Score { get; set; }
        int DisplayedScore { get; set; }
        int Multiplier { get; set; }
        int ComboTimer { get; set; }
        const int comboTime = 20;
        int killCount = 0;
        public int Level { get; set; }   
        int killLevelDelta = 3;
        int killsTillNextLevel;

        static Sprite ScoreSprite { get; set; }
        static SpriteBatch.RenderDetails ScoreRenderInfo { get; set; }

        static Sprite LevelSprite { get; set; }
        static SpriteBatch.RenderDetails LevelRenderInfo { get; set; }

        static Sprite ReinforceIconSprite { get; set; }
        static SpriteBatch.RenderDetails ReinforceIconInfo { get; set; }

        static Sprite SpeedupIconSprite { get; set; }
        static SpriteBatch.RenderDetails SpeedupIconInfo { get; set; }

        static MainGameState()
        {
            int[] area = new int[4];
            GL.GetInteger(GetPName.Viewport, area);

            Bitmap image = Flak.Properties.Resources.score;
            image.MakeTransparent(Color.Red);

            ScoreSprite = new Sprite(image);
            ScoreRenderInfo = new SpriteBatch.RenderDetails(ScoreSprite, new Vector2(20, 30));
            ScoreRenderInfo.Depth = 3;

            Bitmap image2 = Flak.Properties.Resources.level;
            image2.MakeTransparent(Color.Red);

            LevelSprite = new Sprite(image2);
            LevelRenderInfo = new SpriteBatch.RenderDetails(LevelSprite, new Vector2(450, 30));
            LevelRenderInfo.Depth = 3;

            Bitmap image3 = Flak.Properties.Resources.reinforceicon;
            image3.MakeTransparent(Color.Red);

            ReinforceIconSprite = new Sprite(image3);
            ReinforceIconInfo = new SpriteBatch.RenderDetails(ReinforceIconSprite, new Vector2(20, 550));
            ReinforceIconInfo.Depth = 3;

            Bitmap image4 = Flak.Properties.Resources.speedupicon;
            image4.MakeTransparent(Color.Red);

            SpeedupIconSprite = new Sprite(image4);
            SpeedupIconInfo = new SpriteBatch.RenderDetails(SpeedupIconSprite, new Vector2(400, 550));
            SpeedupIconInfo.Depth = 3;
        }

        public MainGameState(Game mainWindow)
            : base(mainWindow)
        {
            manager = new EntityManager();

            mainPlayer = new Player(new Vector2(400, 300), mainGame.Keyboard, mainGame.Mouse, manager, this);
            manager.Add(mainPlayer);

            mainGame.Keyboard.KeyDown += Keyboard_KeyDown;
            random = new Random();

            SwarmDelay = 200;
            SwarmSize = 2;

            Score = 0;
            DisplayedScore = 0;
            Level = 0;

            killsTillNextLevel = killLevelDelta;
            Multiplier = 1;
        }

        void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.Escape)
                EndGame();
        }

        public override void Update()
        {
            if (mainPlayer != null)
            {
                SwarmTimer--;
                if (SwarmTimer <= 0)
                {
                    SwarmTimer = SwarmDelay;
                    AddSwarm(SwarmSize);
                }
            }

            if (ComboTimer > 0)
            {
                ComboTimer--;
                if (ComboTimer == 0)
                {
                    if (Multiplier > 2)
                        manager.Add(new ComboPopup(Multiplier-1, lastKillPos - new Vector2(0, 20), manager));
                    Multiplier = 1;
                }
            }

            if (DisplayedScore < Score)
            {
                DisplayedScore += 5;
                if (DisplayedScore > Score)
                    DisplayedScore = Score;
            }

            manager.Update();
        }

        public override void Draw()
        {
            manager.Draw(mainGame.Spritebatch);

            if (mainPlayer != null)
            {
                mainGame.Spritebatch.ApplyViewProjection();
                mainPlayer.DrawOverlay();
            }

            //draw overlay
            if (mainPlayer != null)
            {
                NumberWriter.Print(DisplayedScore, new Vector2(105, 30), 0.9f, mainGame.Spritebatch);
                mainGame.Spritebatch.AddSprite(ScoreRenderInfo);
                mainGame.Spritebatch.Draw();

                NumberWriter.Print(Level, new Vector2(527, 32), 0.9f, mainGame.Spritebatch);
                mainGame.Spritebatch.AddSprite(LevelRenderInfo);
                mainGame.Spritebatch.Draw();

                GL.Disable(EnableCap.Texture2D);
                GL.Begin(BeginMode.Quads);
                GL.Vertex3(60, 550, 2);
                GL.Vertex3(60 + mainPlayer.ReinforceTimer, 550, 2);
                GL.Vertex3(60 + mainPlayer.ReinforceTimer, 582, 2);
                GL.Vertex3(60, 582, 2);
                GL.End();

                GL.Begin(BeginMode.Quads);
                GL.Vertex3(440, 550, 2);
                GL.Vertex3(440 + mainPlayer.SpeedupTimer, 550, 2);
                GL.Vertex3(440 + mainPlayer.SpeedupTimer, 582, 2);
                GL.Vertex3(440, 582, 2);
                GL.End();

                if (mainPlayer.ReinforceTimer > 0)
                {
                    mainGame.Spritebatch.AddSprite(ReinforceIconInfo);
                    mainGame.Spritebatch.Draw();
                }

                if (mainPlayer.SpeedupTimer > 0)
                {
                    mainGame.Spritebatch.AddSprite(SpeedupIconInfo);
                    mainGame.Spritebatch.Draw();
                }
            }
        }

        public void EndGame()
        {
            if (mainPlayer != null)
                mainPlayer.Destroy();
            mainPlayer = null;
            mainGame.Keyboard.KeyDown -= Keyboard_KeyDown;
            mainGame.SwitchState(new TitleState(this));
            HighScoreTable.ScoreTable().SubmitScore(Score);
        }

        public void Dispose()
        {
            manager.Clear();
        }

        const float minRadius = 150;
        const float maxRadius = 300;

        private void AddSwarm(int size)
        {
            float radius = minRadius + (maxRadius - minRadius) * (float)random.NextDouble();
            for (int i = 0; i < size; i++)
            {
                //get position
                double a = random.NextDouble() * MathHelper.TwoPi;

                int[] area = new int[4];
                GL.GetInteger(GetPName.Viewport, area);

                float r = (float)Math.Sqrt(Math.Pow(area[2]/2, 2) + Math.Pow(area[3]/2, 2));

                Vector2 v = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
                v *= r;

                v += new Vector2(area[2] / 2, area[3] / 2);

                //add enemy
                int typeindex = random.Next(Level);
                if (typeindex <= 2)
                    manager.Add(new SlowEnemy(v, manager, mainPlayer, radius, this));    
                else if (typeindex == 3)
                    manager.Add(new HeavyEnemy(v, manager, mainPlayer, radius, this));
                else if (typeindex <= 5)
                {
                    for (int k = 0; k < 5; k++)
                        manager.Add(new LightEnemy(v, manager, mainPlayer, radius, this));
                }
                else
                    manager.Add(new FastEnemy(v, manager, mainPlayer, radius, this));
            }
        }

        Vector2 lastKillPos;

        public void ReportKill(Enemy enemy)
        {
            if (mainPlayer != null)
            {
                int bonus = enemy.KillBonus * Multiplier;
                Score += bonus;
                manager.Add(new NumberPopUp(bonus, enemy.Position, manager));
                ComboTimer = comboTime;
                Multiplier++;
                killCount++;
                lastKillPos = enemy.Position;

                if (killCount == killsTillNextLevel)
                    LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            killsTillNextLevel += killLevelDelta;
            killLevelDelta *= 2;
            SwarmDelay -= swarmDelayDelta;
            if (SwarmDelay < minSwarmDelay)
                SwarmDelay = minSwarmDelay;

            if (Level % 2 == 0)//increase swarm size every 2 levels
            {
                SwarmSize++;
                if (SwarmSize > maxSwarmSize)
                    SwarmSize = maxSwarmSize;
            }
        }
    }
}
