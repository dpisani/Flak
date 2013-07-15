using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;

namespace Flak
{
    class HeavyEnemy : Enemy
    {

        static Sprite HeavyEnemySprite { get; set; }

        float targetRadius;

        static HeavyEnemy()
        {
            Bitmap image = Flak.Properties.Resources.spaceship4;
            image.MakeTransparent(Color.Red);
            HeavyEnemySprite = new Sprite(image);
            HeavyEnemySprite.Center = new Vector2(21, 27.5f);
        }

        public HeavyEnemy(Vector2 position, EntityManager manager, Player target, float targetRadius, MainGameState mainGame)
            : base(5, 1.0f, 4, 0.07f, manager, target, mainGame)
        {
            DrawParams = new SpriteBatch.RenderDetails(HeavyEnemySprite, position);

            Health = 10;
            fireRate = 220;
            this.targetRadius = targetRadius;
            KillBonus = 75;

            minDebris = 5;
            maxDebris = 11;
            sparks = 35;
            separationDistance = 75;
        }

        public override void UpdateSteeringForce()
        {
            SteeringForce = GetSeparativeForce();
            if (SteeringForce.LengthFast <= maxForce*0.5f)
                SteeringForce += GetStrafeForce(targetRadius, 5, -0.9f);
        }

        bool fireLeft = true;

        protected override void Fire()
        {
            double a = Math.Atan2(Orientation.Y, Orientation.X) + OpenTK.MathHelper.PiOver2;
            Vector2 v = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));

            Vector2 sp = v * 16;

            if (fireLeft)
                sp = -sp;

            fireLeft = !fireLeft;

            sp += Position;

            Manager.Add(new Missile(sp, Orientation * missileSpeed, Manager));
        }
    }
}
