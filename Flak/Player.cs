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
        //input devices
        KeyboardDevice keyboard;
        MouseDevice mouse;

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
                {
                    DrawParams.Rotation = (float)Math.Atan2(orientation.Y, orientation.X);
                    orientation = Vector2.Normalize(orientation);
                }
            }
        }

        bool IsMousePressed { get; set; }
        bool IsMouseReleased { get; set; }

        int weaponCooloff;
        int WeaponCooloff
        {
            get { return weaponCooloff; }
            set
            {
                weaponCooloff = value;
                if (weaponCooloff < 0)
                    weaponCooloff = 0;
            }
        }
        const int cooloffPeriod = 20;

        float angleCharge;
        float AngleCharge
        {
            get { return angleCharge; }
            set
            {
                angleCharge = value;
                if (angleCharge > 1)
                    angleCharge = 1;
                if (angleCharge < 0)
                    angleCharge = 0;
            }
        }
        const float maxAngle = (float)Math.PI * 0.5f;
        const float minAngle = (float)Math.PI * 0.05f;


        public Player(Vector2 position, KeyboardDevice keyboard, MouseDevice mouse, EntityManager manager)
            :base(manager)
        {
            DrawParams = new SpriteBatch.RenderDetails(new Sprite(Flak.Properties.Resources.test_sprite), position);
            DrawParams.Sprite.Center = new Vector2(32, 32);
            this.keyboard = keyboard;
            this.mouse = mouse;

            mouse.ButtonDown += mouse_ButtonDown;
            mouse.ButtonUp += mouse_ButtonUp;
        }

        void mouse_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                IsMouseReleased = true;
                IsMousePressed = false;
            }
        }

        void mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                IsMousePressed = true;
        }

        public override void Update()
        {
            //take keyboard input
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

            Orientation = Vector2.Normalize(new Vector2(mouse.X, mouse.Y) - Position);

            UpdateMovement(moveDir * maxForce);

            //update shooting
            UpdateWeapons();
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
        }

        private void UpdateWeapons()
        {
            //if weapon fired cause a delay
            WeaponCooloff--;

            //check if the left button is pressed
            if (IsMousePressed)
            {
                //charge the weapon
                AngleCharge += 0.1f;
            }

            //check if the mouse button is released
            if (IsMouseReleased)
            {
                //handle mouse press
                IsMouseReleased = false; //we recieved this message

                if (WeaponCooloff == 0)
                {
                    //FIRE!
                    Burst();
                    AngleCharge = 0;
                    WeaponCooloff = cooloffPeriod;
                }
            }
        }

        const float maxBulletSpeed = 17;
        const float minBulletSpeed = 13;

        const float shipLength = 32;

        private void Burst()
        {
            float angleRange = maxAngle - (maxAngle - minAngle) * AngleCharge;
            int numBullets = random.Next(5, 10);
            for (int i = 0; i < numBullets; i++)
            {
                double angle = random.NextDouble() * angleRange;
                angle -= angleRange / 2.0f;

                angle += Math.Atan2(Orientation.Y, Orientation.X);


                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                float speed = minBulletSpeed + (maxBulletSpeed - minBulletSpeed) * (float)random.NextDouble();

                //make bullet
                Manager.Add(new Bullet(Position + Orientation*shipLength, dir*speed, Manager));
            }
        }
    }
}
