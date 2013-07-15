using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    class SpeedupPowerup : Powerup
    {
        static Sprite SpeedupSprite { get; set; }

        static SpeedupPowerup()
        {
            Bitmap image = Flak.Properties.Resources.speedup;
            image.MakeTransparent(Color.Red);
            SpeedupSprite = new Sprite(image);
            SpeedupSprite.Center = new Vector2(16, 16);
        }

        public SpeedupPowerup(Vector2 position, EntityManager manager)
            : base(manager)
        {
            DrawParams = new SpriteBatch.RenderDetails(SpeedupSprite, position);
        }
    }
}
