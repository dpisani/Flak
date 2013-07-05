using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    class FastEnemy : Enemy
    {
        static Sprite FastEnemySprite { get; set; }

        float targetRadius;

        int secondFireTimer = 0;
        const int secondFireDelay = 20;

        static FastEnemy()
        {
            Bitmap image = Flak.Properties.Resources.spaceship3;
            image.MakeTransparent(Color.Red);
            FastEnemySprite = new Sprite(image);
            FastEnemySprite.Center = new Vector2(25, 21);
        }

        public FastEnemy(Vector2 position, EntityManager manager, Player target, float targetRadius, MainGameState mainGame)
            : base(1, 1.5f, 7, 0.09f, manager, target, mainGame)
        {     
            DrawParams = new SpriteBatch.RenderDetails(FastEnemySprite, position);

            Health = 5;
            fireRate = 150;
            this.targetRadius = targetRadius;
            KillBonus = 100;
        }

        public override void Update()
        {
            base.Update();

            if (secondFireTimer > 0)
            {
                secondFireTimer--;
                if (secondFireTimer == 0)
                    Fire();
            }
        }

        public override void UpdateSteeringForce()
        {
            SteeringForce = GetSeparativeForce();
            if (SteeringForce.LengthFast <= maxForce * 0.5f)
                SteeringForce += GetStrafeForce(targetRadius, 5, -0.9f);
        }

        bool fireLeft = true;

        protected override void Fire()
        {
            double a = Math.Atan2(Orientation.Y, Orientation.X) + OpenTK.MathHelper.PiOver2;
            Vector2 v = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));

            Vector2 sp = v * 10;

            if (fireLeft)
                sp = -sp;

            if (fireLeft)
                secondFireTimer = secondFireDelay;

            fireLeft = !fireLeft;

            sp += Position + Orientation * 5;

            Manager.Add(new Missile(sp, Orientation * missileSpeed, Manager));
        }
    }
}
