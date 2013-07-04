using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    class SlowEnemy : Enemy
    {
        static Sprite SlowEnemySprite { get; set; }

        static SlowEnemy()
        {
            Bitmap image = Flak.Properties.Resources.spaceship2;
            image.MakeTransparent(Color.Red);
            SlowEnemySprite = new Sprite(image);
        }

        public SlowEnemy(Vector2 position, EntityManager manager, Player target)
            : base(1, 0.5f, 3, 0.05f, manager, target)
        {     
            DrawParams = new SpriteBatch.RenderDetails(SlowEnemySprite, position);
            DrawParams.Sprite.Center = new Vector2(16, 25);

            Health = 5;
        }

        public override void UpdateSteeringForce()
        {
            SteeringForce = GetSeparativeForce() + GetStrafeForce(200, 5, 0.5f);
        }
    }
}
