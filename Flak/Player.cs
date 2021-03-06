﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Flak
{
    class Player : Vehicle
    {
        //input devices
        KeyboardDevice keyboard;
        MouseDevice mouse;

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
        const float maxAngle = (float)Math.PI * 0.8f;
        const float minAngle = (float)Math.PI * 0.05f;
        const float chargeSpeed = 0.08f;

        float moveForce;

        const int powerupTime = 300;
        public int ReinforceTimer { get; set; }
        const int reinforceAmount = 12;
        public int SpeedupTimer { get; set; }
        const float speedupAmount = 0.15f;
        const int cooloffReduce = 10;

        static Sprite PlayerSprite { get; set; }

        MainGameState game;

        static Player()
        {
            Bitmap image = Flak.Properties.Resources.spaceship1;
            image.MakeTransparent(Color.Red);
            PlayerSprite = new Sprite(image);
            PlayerSprite.Center = new Vector2(20, 14);
        }     

        public Player(Vector2 position, KeyboardDevice keyboard, MouseDevice mouse, EntityManager manager, MainGameState game)
            :base(1.5f, 5.0f, 10.0f, 0.09f, manager)
        {    
            DrawParams = new SpriteBatch.RenderDetails(PlayerSprite, position);
            DrawParams.Depth = 1;
            this.keyboard = keyboard;
            this.mouse = mouse;

            mouse.ButtonDown += mouse_ButtonDown;
            mouse.ButtonUp += mouse_ButtonUp;

            moveForce = maxForce - 2;

            WeaponCooloff = 20;

            this.game = game;
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

            SteeringForce = moveDir * maxForce;
            //update shooting
            UpdateWeapons();

            UpdateMovement();

            if (ReinforceTimer > 0)
                ReinforceTimer--;

            if (SpeedupTimer > 0)
                SpeedupTimer--;
        }

        protected override void UpdateMovement()
        {
            UpdateVelocity();

            int[] area = new int[4];
            GL.GetInteger(GetPName.Viewport, area);
            Vector2 pos = Position + Velocity;
            Vector2 mov = Velocity;
            if (pos.X < area[0] || pos.X > area[2])
                mov.X = 0;
            if (pos.Y < area[1] || pos.Y > area[3])
                mov.Y = 0;

            Position += mov;
        }

        const float recoil = 5.0f;

        private void UpdateWeapons()
        {
            //if weapon fired cause a delay
            WeaponCooloff--;

            //check if the mouse button is released
            if (IsMouseReleased)
            {
                //handle mouse press
                IsMouseReleased = false; //we recieved this message

                if (WeaponCooloff == 0)
                {
                    //FIRE!
                    Burst();
                    SteeringForce += -Orientation * recoil * AngleCharge;
                    AngleCharge = 0;
                    WeaponCooloff = cooloffPeriod;
                    if (SpeedupTimer > 0)
                        WeaponCooloff -= cooloffReduce;
                }
            }

            //check if the left button is pressed
            if (IsMousePressed && WeaponCooloff == 0)
            {
                //charge the weapon
                AngleCharge += chargeSpeed;
                if (SpeedupTimer > 0)
                    AngleCharge += speedupAmount;
            }
            else
                AngleCharge = 0;
        }

        const float maxBulletSpeed = 17;
        const float minBulletSpeed = 13;

        const float shipLength = 13;

        const int minBullets = 7;
        const int maxBullets = 11;

        private void Burst()
        {
            float angleRange = maxAngle - (maxAngle - minAngle) * AngleCharge;
            int numBullets = random.Next(minBullets, maxBullets);

            if (ReinforceTimer > 0)
                numBullets += reinforceAmount;

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

            AudioManager.Manager().PlayPhaser();
        }

        static float arcDrawRad = 100.0f;
        static int arcDrawSegments = 10;

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);          
        }

        public void DrawOverlay()
        {
            if (AngleCharge == 0)
                return;

            GL.Disable(EnableCap.Texture2D);
            GL.AlphaFunc(AlphaFunction.Greater, 0.3f);

            //draw the weapon angle         
            Vector2 center = Position + Orientation * shipLength;
            GL.Begin(BeginMode.TriangleFan);
            GL.Color4(1.0f, 1.0f, 1.0f, AngleCharge * 0.3f);
            GL.Vertex3(center.X, center.Y, 1);

            float angleRange = maxAngle - (maxAngle - minAngle) * AngleCharge;
            float angledif = angleRange / (float)arcDrawSegments;
            float angleStart = (float)Math.Atan2(Orientation.Y, Orientation.X) - angleRange / 2.0f;
            GL.Color4(1.0f, 1.0f, 1.0f, AngleCharge * 0.1f);
            for (int i = 0; i < arcDrawSegments; i++)
            {
                float angle = angleStart + angledif * i;
                Vector2 v = center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle))*arcDrawRad;
                GL.Vertex3(v.X, v.Y, 1);
            }
            GL.End();

            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
        }

        public override void HandleCollision(Entity other)
        {
            if (other is Missile || other is Debris || other is Enemy)
            {
                Destroy();
                game.EndGame();
            }
            else if (other is ReinforcePowerup)
                ReinforceTimer = powerupTime;
            else if (other is SpeedupPowerup)
                SpeedupTimer = powerupTime;
        }

        public void Destroy()
        {
            Manager.Remove(this);
            SparkParticle.SparkBurst(20, Position, 10, Manager);
            AudioManager.Manager().PlayBoom();
        }
    }
}
