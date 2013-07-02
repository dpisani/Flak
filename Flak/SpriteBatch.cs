using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    public class SpriteBatch
    {
        public struct RenderDetails
        {
            public Sprite Sprite;
            public Vector2 Position;
            public float Rotation;
            public Vector2 Scale;

            public RenderDetails(Sprite sprite, Vector2 position)
            {
                Sprite = sprite;
                Position = position;
                Rotation = 0;
                Scale = new Vector2(1, 1);
            }
        }

        Queue<RenderDetails> renderInstances;

        Matrix4 view;
        Matrix4 projection;

        public SpriteBatch(System.Drawing.Rectangle viewportSize)
        {
            renderInstances = new Queue<RenderDetails>();
            view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
            ConfigureProjection(viewportSize);
        }

        public void ConfigureProjection (System.Drawing.Rectangle viewportSize)
        {
            //create orthographic projection since we're in 2d
            projection = Matrix4.CreateOrthographicOffCenter(0, viewportSize.Width, viewportSize.Height, 0, -10, 1000);
        }

        public void Begin()
        {
            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            GL.VertexPointer(3, VertexPointerType.Float, 5 * sizeof(float), (IntPtr)(0));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            //enable texturing
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.TextureCoordArray);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

           // GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)All.Modulate);

            GL.Enable(EnableCap.VertexArray);
        }

        public void AddSprite(RenderDetails details)
        {
            renderInstances.Enqueue(details);
        }

        public void End()
        {
            while (renderInstances.Count > 0)
            {
                RenderDetails instance = renderInstances.Dequeue();
                //get final position of the quad
                Vector3 pos = new Vector3(instance.Sprite.Center.X + instance.Position.X, instance.Sprite.Center.Y + instance.Position.Y, 0);
                //create the world matrix
                Matrix4 scale = Matrix4.Scale(instance.Scale.X, instance.Scale.Y, 0);
                Matrix4 rotation = Matrix4.CreateRotationZ(instance.Rotation);
                Matrix4 translation = Matrix4.CreateTranslation(pos);

                Matrix4 world = scale * rotation * translation;

                //combine with the view to make modelview
                Matrix4 modelview = world * view;

                //set matrix
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref modelview);

                //set sprite buffers and texture
                instance.Sprite.BindResources();

                //draw the textured quad
                GL.DrawArrays(BeginMode.Quads, 0, 4);
            }

            GL.PopMatrix();
        }
    }
}
