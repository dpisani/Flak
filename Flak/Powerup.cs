using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    class Powerup : Entity
    {
        protected int Timer { get; set; }

        protected Powerup(EntityManager manager)
            : base(manager)
        {
            Timer = 200;
        }

        public override void Update()
        {
            Timer--;
            if (Timer == 0)
                Manager.Remove(this);
        }

        public override void HandleCollision(Entity other)
        {
            if (other is Player)
                Manager.Remove(this);
        }
    }
}
