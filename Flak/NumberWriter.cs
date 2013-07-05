using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;

namespace Flak
{
    class NumberWriter
    {
        static Sprite DigitSprite { get; set; }

        static NumberWriter()
        {
            Bitmap image = Flak.Properties.Resources.digits;
            image.MakeTransparent(Color.Red);
            DigitSprite = new Sprite(image, Vector2.Zero, 10, 1, 10);
        }

        public static void Print(int number, Vector2 position, float scale, SpriteBatch spritebatch)
        {
            string num = number.ToString();         

            Vector2 offset = new Vector2(DigitSprite.Width * scale, 0);
            for (int i = 0; i < num.Length; i++)
            {
                Vector2 pos = position + offset * i;

                int frame = num[i] - '0';

                SpriteBatch.RenderDetails info = new SpriteBatch.RenderDetails(DigitSprite, pos, 0, Vector2.One * scale, frame, 2);
                spritebatch.AddSprite(info);
            }
        }
    }
}
