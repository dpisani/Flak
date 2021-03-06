﻿using System;
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

        float targetRadius;

        static SlowEnemy()
        {
            Bitmap image = Flak.Properties.Resources.spaceship2;
            image.MakeTransparent(Color.Red);
            SlowEnemySprite = new Sprite(image);
            SlowEnemySprite.Center = new Vector2(16, 25);
        }

        public SlowEnemy(Vector2 position, EntityManager manager, Player target, float targetRadius, MainGameState mainGame)
            : base(1, 0.5f, 3, 0.05f, manager, target, mainGame)
        {     
            DrawParams = new SpriteBatch.RenderDetails(SlowEnemySprite, position);

            Health = 5;
            fireRate = 200;
            this.targetRadius = targetRadius;
            KillBonus = 50;

            minDebris = 3;
            maxDebris = 5;
            sparks = 25;
        }

        public override void UpdateSteeringForce()
        {
            SteeringForce = GetSeparativeForce();
            if (SteeringForce.LengthFast <= maxForce*0.5f)
                SteeringForce += GetStrafeForce(targetRadius, 5, 0.5f);
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
