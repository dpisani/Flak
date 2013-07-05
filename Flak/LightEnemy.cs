using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    class LightEnemy : Enemy
    {
        static Sprite SlowEnemySprite { get; set; }

        float targetRadius;

        static LightEnemy()
        {
            Bitmap image = Flak.Properties.Resources.spaceship5;
            image.MakeTransparent(Color.Red);
            SlowEnemySprite = new Sprite(image);
            SlowEnemySprite.Center = new Vector2(13, 6.5f);
        }

        public LightEnemy(Vector2 position, EntityManager manager, Player target, float targetRadius, MainGameState mainGame)
            : base(0.4f, 0.5f, 4, 0.05f, manager, target, mainGame)
        {     
            DrawParams = new SpriteBatch.RenderDetails(SlowEnemySprite, position);

            Health = 1;
            fireRate = 350;
            this.targetRadius = targetRadius;
            KillBonus = 10;

            minDebris = 0;
            maxDebris = 2;
            sparks = 10;
            separationDistance = 45;
        }

        public override void UpdateSteeringForce()
        {
            SteeringForce = GetSeparativeForce();
            if (SteeringForce.LengthFast <= maxForce*0.5f)
                SteeringForce += GetStrafeForce(targetRadius, 5, 0.7f);
        }

        protected override void Fire()
        {
            Manager.Add(new Missile(Position + Orientation * 5, Orientation * missileSpeed, Manager));
        }
    }
}
