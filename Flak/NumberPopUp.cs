using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Flak
{
    class NumberPopUp : Entity
    {
        int Number { get; set; }
        float DisplayedNumber { get; set; }
        new Vector2 Position { get; set; }
        int countdown = 50;
        float upcountDelta;

        public NumberPopUp(int number, Vector2 position, EntityManager manager)
            : base(manager)
        {
            Number = number;
            Position = position;
            DisplayedNumber = 0;
            upcountDelta = Number / 10.0f;
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            NumberWriter.Print((int)DisplayedNumber, Position, 0.8f, spritebatch);
        }

        public override void Update()
        {
            countdown--;

            if (countdown == 0)
                Manager.Remove(this);

            if (DisplayedNumber < Number)
            {
                DisplayedNumber += upcountDelta;
                if (DisplayedNumber > Number)
                    DisplayedNumber = Number;
            }
        }
    }
}
