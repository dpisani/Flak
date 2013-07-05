using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    abstract class Entity
    {
        protected EntityManager Manager { get; set; }

        protected static Random random { get; set; }

        static Entity()
        {
            random = new Random();
        }

        public Entity(EntityManager manager)
        {
            Manager = manager;
        }

        protected SpriteBatch.RenderDetails DrawParams { get; set; }

        public Vector2 Position
        {
            get { return DrawParams.Position; }
            set
            {
                DrawParams.Position = value;
            }
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            spritebatch.AddSprite(DrawParams);
        }

        public abstract void Update();

        public bool DoesCollide(Entity other)
        {
            if (DrawParams == null || other.DrawParams == null)
                return false;

            float rad = Math.Max(DrawParams.Sprite.Height, DrawParams.Sprite.Width) / 2.0f;
            float orad = Math.Max(other.DrawParams.Sprite.Height, other.DrawParams.Sprite.Width) / 2.0f;

            float distance = (Position - other.Position).Length;

            return distance < rad + orad;
        }

        public virtual void HandleCollision(Entity other) { }

        public virtual void Dispose()
        {
            Manager = null;
        }
    }
}
