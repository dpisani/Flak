using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    class Debris : Entity
    {
        static Sprite DebrisSprite { get; set; }

        int immunityTimer = 5;

        static Debris()
        {
            Bitmap image = Flak.Properties.Resources.debris;
            image.MakeTransparent(Color.Red);
            DebrisSprite = new Sprite(image, new Vector2(8, 8), 5, 1, 5);
        }

        Vector2 Velocity { get; set; }
        float spin;

        public Debris(Vector2 position, Vector2 velocity, EntityManager manager)
            : base(manager)
        {
            spin = (float)random.NextDouble() * 0.5f;
            DrawParams = new SpriteBatch.RenderDetails(DebrisSprite, position, (float)random.NextDouble(), Vector2.One*2, random.Next(5), 0);
            Velocity = velocity;
        }

        public override void Update()
        {
            DrawParams.Rotation += spin;
            Position += Velocity;

            if (immunityTimer > 0)
                immunityTimer--;

            //check if the debris has left the screen
            int[] area = new int[4];
            GL.GetInteger(GetPName.Viewport, area);

            if (Position.X < area[0] - 8 || Position.Y < area[1] - 8 ||
                Position.X > area[2] + 8 || Position.Y > area[3] + 8)
                Manager.Remove(this);
        }

        public override void HandleCollision(Entity other)
        {
            if (other is Debris)
            {
                if (immunityTimer == 0)
                    Destroy();
                else
                    Position += (Position - other.Position)*0.2f;
            }
            else if (other is Bullet)
                Destroy();
            else if (other is Vehicle)
                Destroy();
        }

        void Destroy()
        {
            SparkParticle.SparkBurst(5, Position, 3, Manager);
            Manager.Remove(this);
        }
    }
}
