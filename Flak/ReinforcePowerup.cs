using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    class ReinforcePowerup : Powerup
    {
        static Sprite ReinforceSprite { get; set; }

        static ReinforcePowerup()
        {
            Bitmap image = Flak.Properties.Resources.reinforce;
            image.MakeTransparent(Color.Red);
            ReinforceSprite = new Sprite(image);
            ReinforceSprite.Center = new Vector2(16, 16);
        }

        public ReinforcePowerup(Vector2 position, EntityManager manager)
            : base(manager)
        {
            DrawParams = new SpriteBatch.RenderDetails(ReinforceSprite, position);
        }
    }
}
