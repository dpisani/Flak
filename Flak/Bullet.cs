using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    public class Bullet : Entity
    {
        static Sprite BulletSprite { get; set; }

        Vector2 Velocity { get; set; }
        int lifetime;
        int Lifetime
        {
            get { return lifetime; }
            set
            {
                lifetime = value;
                if (lifetime <= 0)
                    Manager.Remove(this);
            }
        }

        static Bullet()
        {
            BulletSprite = new Sprite(Flak.Properties.Resources.bullet);
            BulletSprite.Center = new Vector2(4, 4);
        }

        public Bullet(Vector2 position, Vector2 velocity, EntityManager manager)
            :base (manager)
        {
            float size = (float)random.NextDouble() + 0.1f;
            if (size < 0.5)
                size = 0.5f;
            DrawParams = new SpriteBatch.RenderDetails(BulletSprite, position, 0, Vector2.One*size, 0);
            Velocity = velocity;
            Lifetime = 100;
        }

        public override void Update()
        {
            Position += Velocity;
            Lifetime--;
        }
    }
}
