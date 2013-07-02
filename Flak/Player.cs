using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    public class Player : Entity
    {
        KeyboardDevice keyboard;

        const float mass = 1.5f;
        const float maxForce = 3.0f;
        const float maxSpeed = 10.0f;
        const float friction = 0.95f;
        Vector2 Velocity { get; set; }

        Vector2 orientation;
        Vector2 Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                if (orientation.Length > 0)
                    DrawParams.Rotation = (float)Math.Atan2(orientation.Y, orientation.X);
            }
        }

        public Player(Vector2 position, KeyboardDevice keyboard)
        {
            DrawParams = new SpriteBatch.RenderDetails(new Sprite(Flak.Properties.Resources.test_sprite), position);
            DrawParams.Sprite.Center = new Vector2(32, 32);
            this.keyboard = keyboard;
        }

        public override void Update()
        {
            Vector2 moveDir = Vector2.Zero;
            if (keyboard[Key.W])
                moveDir += new Vector2(0, -1);
            if (keyboard[Key.A])
                moveDir += new Vector2(-1, 0);
            if (keyboard[Key.S])
                moveDir += new Vector2(0, 1);
            if (keyboard[Key.D])
                moveDir += new Vector2(1, 0);

            if (moveDir != Vector2.Zero)
                moveDir.Normalize();

            UpdateMovement(moveDir * maxForce);
        }

        private void UpdateMovement(Vector2 steeringForce)
        {
            //deceleration
            Velocity *= friction;
            if (Velocity.LengthFast < 0.2)
                Velocity = Vector2.Zero;

            Vector2 acceleration = steeringForce / mass;
            Velocity += acceleration;
            if (Velocity.Length > maxSpeed)
            {
                Velocity = Vector2.Normalize(Velocity);
                Velocity *= maxSpeed;
            }
            Position += Velocity;

            Orientation = Vector2.Normalize(Velocity);
        }
    }
}
