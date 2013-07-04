using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Flak
{
    abstract class Vehicle : Entity
    {
        protected float mass;
        protected float maxForce;
        protected float maxSpeed;
        protected float friction;
        public Vector2 Velocity { get; set; }
        protected Vector2 SteeringForce { get; set; }

        Vector2 orientation;
        protected Vector2 Orientation
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

        protected Vehicle(float mass, float maxForce, float maxSpeed, float friction, EntityManager manager)
            :base (manager)
        {
            this.mass = mass;
            this.maxForce = maxForce;
            this.maxSpeed = maxSpeed;
            this.friction = friction;
        }

        protected void UpdateVelocity()
        {
            float steeringpower = SteeringForce.Length;
            if (steeringpower > maxForce)
                SteeringForce = (SteeringForce / steeringpower) * maxForce;

            Vector2 acceleration = SteeringForce / mass;


            //deceleration
            Vector2 decel = Velocity * friction;

            Velocity += acceleration;
            Velocity -= decel;

            if (Velocity.LengthFast < 0.2)
                Velocity = Vector2.Zero;
            
            if (Velocity.Length > maxSpeed)
            {
                Velocity = Vector2.Normalize(Velocity);
                Velocity *= maxSpeed;
            }
        }

        protected virtual void UpdateMovement()
        {
            UpdateVelocity();
            Position += Velocity;
        }

        const float minDebrisSpeed = 5;
        const float maxDebrisSpeed = 8;

        protected void DebrisBurst(int num)
        {
            for (int i = 0; i < num; i++)
            {
                double angle = OpenTK.MathHelper.TwoPi * random.NextDouble();
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                float speed = minDebrisSpeed + (maxDebrisSpeed - minDebrisSpeed) * (float)random.NextDouble();
                Manager.Add(new Debris(Position, dir * speed, Manager));
            }
        }
    }
}
