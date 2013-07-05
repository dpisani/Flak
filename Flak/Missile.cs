using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    class Missile : Entity
    {
        static Sprite MissileSprite { get; set; }

        Vector2 Velocity { get; set; }

        static Missile()
        {
            Bitmap image = Flak.Properties.Resources.missile;
            image.MakeTransparent(Color.Red);
            MissileSprite = new Sprite(image);
            MissileSprite.Center = new Vector2(8, 3);
        }

        public Missile(Vector2 position, Vector2 velocity, EntityManager manager)
            : base(manager)
        {
            float angle = (float)Math.Atan2(velocity.Y, velocity.X);
            DrawParams = new SpriteBatch.RenderDetails(MissileSprite, position, angle, Vector2.One, 0, 0);
            Velocity = velocity;
        }

        public override void Update()
        {
            Position += Velocity;
            //check if the missile has left the screen
            int[] area = new int[4];
            GL.GetInteger(GetPName.Viewport, area);

            if (Position.X < area[0] - 8 || Position.Y < area[1] - 8 ||
                Position.X > area[2] + 8 || Position.Y > area[3] + 8)
                Manager.Remove(this);
        }

        bool destroyed = false;

        public override void HandleCollision(Entity other)
        {
            if (destroyed)
                return;

            if (other is Player || other is Debris || other is Bullet)
            {
                SparkParticle.SparkBurst(6, Position, 6, Manager);
                Manager.Remove(this);
                destroyed = true;
            }
        }
    }
}
