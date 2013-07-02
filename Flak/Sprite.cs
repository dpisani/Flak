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


        private int texID;

        uint VBOid;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2 Center { get; set; }

        public Sprite(Bitmap image)
        {
            Width = image.Width;
            Height = image.Height;
   
            //create a new texture
            GL.GenTextures(1, out texID);
            GL.BindTexture(TextureTarget.Texture2D, texID);

            //get raw data from the bitmap
            BitmapData data = image.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //fill the texture with data
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            
            

            //make vertex buffer
            GL.GenBuffers(1, out VBOid);
            //textured quad
            Vertex[] vertices = new Vertex[4];
            vertices[0].Position = new Vector3(0, 0, 0);
            vertices[0].TexCoord = new Vector2(0, 0);

            vertices[1].Position = new Vector3(Width, 0, 0);
            vertices[1].TexCoord = new Vector2(1, 0);

            vertices[2].Position = new Vector3(Width, Height, 0);
            vertices[2].TexCoord = new Vector2(1, 1);

            vertices[3].Position = new Vector3(0, Height, 0);
            vertices[3].TexCoord = new Vector2(0, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * 5 * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
        }

        public void BindResources()
        {
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid);
        }

        public void Dispose()
        {
            GL.DeleteTexture(texID);
            GL.DeleteBuffers(1, ref VBOid);
        }
    }
}
