using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;

namespace Flak
{
    abstract class Enemy : Vehicle
    {
        public int Health { get; set; }

        static List<Enemy> Bretheren { get; set; }

        public Player Target { get; set; }

        static Enemy()
        {
            Bretheren = new List<Enemy>();
        }

        protected Enemy(float mass, float maxForce, float maxSpeed, float friction, EntityManager manager, Player target)
            : base(mass, maxForce, maxSpeed, friction, manager)
        {
            Bretheren.Add(this);
            Target = target;
        }

        public override void HandleCollision(Entity other)
        {
            if (other is Bullet)
                Health -= 1;
            if (other is Debris)
                Health -= 3;
        }

        const int sparks = 20;
        const int minDebris = 4;
        const int maxDebris = 6;

        public virtual void Explode()
        {
            Manager.Remove(this);

            SparkParticle.SparkBurst(sparks, Position, 5, Manager);
            DebrisBurst(random.Next(minDebris, maxDebris));
        }

        public override void Update()
        {
            if (Health <= 0)
            {
                Explode();
                return;
            }

            Orientation = Target.Position - Position;

            UpdateSteeringForce();
            UpdateVelocity();
            UpdateMovement();
        }

        public virtual void UpdateSteeringForce()
        {
            SteeringForce = GetSeparativeForce();
        }

        public override void Dispose()
        {
            Bretheren.Remove(this);
        }

        protected float separationDistance = 100;

        protected Vector2 GetSeparativeForce()
        {
            Vector2 force = Vector2.Zero;

            foreach (Enemy b in Bretheren)
            {
                if (b == this)
                    continue;

                Vector2 v = Position - b.Position;

                if (v == Vector2.Zero)
                    continue;

                if (v.LengthFast > separationDistance)
                    continue;

                Vector2 sep = Vector2.Normalize(v) * separationDistance / v.LengthFast;
                force += sep;
            }

            return force;
        }

        const float slowingDistance = 100.0f;

        protected Vector2 GetStrafeForce(float offset, float t, float strafeSpeed)
        {
            //arrive
            Vector2 predicted = Target.Position + Target.Velocity * t;

            Vector2 ring = predicted - Orientation * offset;

            Vector2 targetOffset = ring - Position;
            float dis = targetOffset.Length;

            float grad = dis / slowingDistance;
            Vector2 desired = Vector2.Normalize(Position - ring) * maxSpeed;


            Vector2 force = (Velocity - desired) * Math.Min(grad, 1);


            //strafe
            double a = Math.Atan2(Orientation.Y, Orientation.X) + OpenTK.MathHelper.PiOver2;
            Vector2 v = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));


            return force + v*strafeSpeed;
        }
    }
}
