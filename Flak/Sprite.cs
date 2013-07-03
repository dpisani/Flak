using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Runtime.InteropServices;

namespace Flak
{
    public class Sprite : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;          
        }

        //Texture resource id
        private int texID;
        //Vertex buffer id
        uint VBOid;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2 Center { get; set; }

        public int Frames { get; private set; }

        public Sprite(Bitmap image, Vector2 center, int horizontalSegments, int verticalSegments, int frames)
        {
            if (image == null || verticalSegments <= 0 || horizontalSegments <= 0)
                throw new ArgumentException();

            Width = image.Width / horizontalSegments;
            Height = image.Height / verticalSegments;

            Frames = frames;
            Center = center;

            CreateSpriteResources(image, horizontalSegments, verticalSegments);
        }

        public Sprite(Bitmap image)
        {
            if (image == null)
                throw new ArgumentException();

            Width = image.Width;
            Height = image.Height;

            Frames = 1;
            Center = Vector2.Zero;

            CreateSpriteResources(image, 1, 1);
        }
        uint va;
        private void CreateSpriteResources(Bitmap image, int horizontalSegments, int verticalSegments)
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

            //make vertex buffer
            GL.GenVertexArrays(1, out va);
            GL.GenBuffers(1, out VBOid);
            //textured quads, one for each frame
            float uSize = 1 / (float)horizontalSegments;
            float vSize = 1 / (float)verticalSegments;
            Vertex[] vertices = new Vertex[4 * Frames];
            int i = 0;
            for (int v = 0; v < verticalSegments; v++)
                for (int u = 0; u < horizontalSegments && i < Frames; u++, i++)
                {
                    vertices[i * 4].Position = new Vector3(0, 0, 0);
                    vertices[i * 4].TexCoord = new Vector2(uSize * u, vSize * v);

                    vertices[i * 4 + 1].Position = new Vector3(Width, 0, 0);
                    vertices[i * 4 + 1].TexCoord = new Vector2(uSize * (u + 1), vSize * v);

                    vertices[i * 4 + 2].Position = new Vector3(Width, Height, 0);
                    vertices[i * 4 + 2].TexCoord = new Vector2(uSize * (u + 1), vSize * (v + 1));

                    vertices[i * 4 + 3].Position = new Vector3(0, Height, 0);
                    vertices[i * 4 + 3].TexCoord = new Vector2(uSize * u, vSize * (v + 1));
                }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * 5 * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        }

        public void BindTexture()
        {     
            GL.BindTexture(TextureTarget.Texture2D, texID);        
        }

        public void BindBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);
            GL.BindVertexArray(va);
        }

        public void Dispose()
        {
            GL.DeleteTexture(texID);
            GL.DeleteBuffers(1, ref VBOid);
        }
    }
}
