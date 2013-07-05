using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    class ComboPopup : Entity
    {
        int Number { get; set; }
        new Vector2 Position { get; set; }
        int countdown = 50;

        static Sprite ComboSprite { get; set; }

        static ComboPopup()
        {
            Bitmap image = Flak.Properties.Resources.combo;
            image.MakeTransparent(Color.Red);
            ComboSprite = new Sprite(image);
        }

        public ComboPopup(int number, Vector2 position, EntityManager manager)
            : base(manager)
        {
            Number = number;
            Position = position;
            DrawParams = new SpriteBatch.RenderDetails(ComboSprite, position + new Vector2(number.ToString().Length * 16, -2));
            DrawParams.Depth = 2;
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            NumberWriter.Print((int)Number, Position, 0.8f, spritebatch);
            spritebatch.AddSprite(DrawParams);
        }

        public override void Update()
        {
            countdown--;

            if (countdown == 0)
                Manager.Remove(this);
        }
    }
}
