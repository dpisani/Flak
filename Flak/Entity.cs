﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    public abstract class Entity
    {
        protected SpriteBatch.RenderDetails DrawParams { get; set; }

        public Vector2 Position
        {
            get { return DrawParams.Position; }
            set
            {
                DrawParams.Position = value;
            }
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            spritebatch.AddSprite(DrawParams);
        }

        public abstract void Update();
    }
}