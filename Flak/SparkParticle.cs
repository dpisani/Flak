using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    class SparkParticle : Entity
    {
        static Sprite SparkSprite {get;set;}
        static SparkParticle()
        {
            Bitmap image = Flak.Properties.Resources.spark;
            image.MakeTransparent(Color.Red);
            SparkSprite = new Sprite(image);
            SparkSprite.Center = new Vector2(4, 2);
        }

        Vector2 Velocity { get; set; }
        int Lifetime { get; set; }

        public SparkParticle(Vector2 position, Vector2 velocity, EntityManager manager)
            : base(manager)
        {
            Velocity = velocity;
            float angle = (float)Math.Atan2(Velocity.Y, Velocity.X);
            DrawParams = new SpriteBatch.RenderDetails(SparkSprite, position, angle, Vector2.One*0.8f, 1, 0);
            Lifetime = 10;
        }

        public override void Update()
        {
            Position += Velocity;
            Lifetime--;
            if (Lifetime == 0)
                Manager.Remove(this);
        }

        public static void SparkBurst(int num, Vector2 origin, float maxSpeed, EntityManager manager)
        {
            for (int i = 0; i < num; i++)
            {
                Vector2 vel = new Vector2((float)random.NextDouble()-0.5f, (float)random.NextDouble()-0.5f);
                vel = vel * 2 * maxSpeed;
                manager.Add(new SparkParticle(origin, vel, manager));
            }
        }
    }
}
