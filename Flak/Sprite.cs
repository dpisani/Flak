using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Runtime.InteropServices;

namespace Flak
{
    class Sprite : IDisposable
    {
        //Texture resource id
        private int texID;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2 Center { get; set; }

        public int Frames { get; private set; }

        int VerticalSegments { get; set; }
        int HorizontalSegments { get; set; }
        float USize { get; set; }
        float VSize { get; set; }

        public Sprite(Bitmap image, Vector2 center, int horizontalSegments, int verticalSegments, int frames)
        {
            if (image == null || verticalSegments <= 0 || horizontalSegments <= 0)
                throw new ArgumentException();

            HorizontalSegments = horizontalSegments;
            VerticalSegments = verticalSegments;

            Width = image.Width / horizontalSegments;
            Height = image.Height / verticalSegments;

            Frames = frames;
            Center = center;

            CreateSpriteResources(image);
        }

        public Sprite(Bitmap image)
        {
            if (image == null)
                throw new ArgumentException();

            Width = image.Width;
            Height = image.Height;

            Frames = 1;
            Center = Vector2.Zero;

            HorizontalSegments = 1;
            VerticalSegments = 1;

            CreateSpriteResources(image);           
        }

        private void CreateSpriteResources(Bitmap image)
        {
            //create a new texture
            GL.GenTextures(1, out texID);
            GL.BindTexture(TextureTarget.Texture2D, texID);

            //get raw data from the bitmap
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //fill the texture with data
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            USize = 1 / (float)HorizontalSegments;
            VSize = 1 / (float)VerticalSegments;

            GL.Flush();
        }

        public void BindTexture()
        {     
            GL.BindTexture(TextureTarget.Texture2D, texID);        
        }

        public void AddVertices(int frame)
        {
            int v = frame / HorizontalSegments;
            int u = frame % HorizontalSegments;

            GL.TexCoord2(new Vector2(USize * u, VSize * v));
            GL.Vertex3(new Vector3(0, 0, 0));

            GL.TexCoord2(new Vector2(USize * (u + 1), VSize * v));
            GL.Vertex3(new Vector3(Width, 0, 0));

            GL.TexCoord2(new Vector2(USize * (u + 1), VSize * (v + 1)));
            GL.Vertex3(new Vector3(Width, Height, 0));

            GL.TexCoord2(new Vector2(USize * u, VSize * (v + 1)));
            GL.Vertex3(new Vector3(0, Height, 0));
            

        }

        public void Dispose()
        {
            GL.DeleteTexture(texID);
        }
    }
}
