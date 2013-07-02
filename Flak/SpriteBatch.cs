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
            public Sprite Sprite { get; set; }
            public Vector2 Position { get; set; }
            public float Rotation { get; set; }
            public Vector2 Scale { get; set; }
            private int frame;
            public int Frame
            {
                get { return frame; }
                set
                {
                    frame = value;

                    if (frame < 0)
                        frame = Sprite.Frames - 1 - (frame % Sprite.Frames);
                    if (frame >= Sprite.Frames)
                        frame = (frame % Sprite.Frames);
                }
            }

            public RenderDetails(Sprite sprite, Vector2 position)
                :this()
            {
                this.frame = 0;
                Sprite = sprite;
                Position = position;
                Rotation = 0;
                Scale = new Vector2(1, 1);
            }

            public RenderDetails(Sprite sprite, Vector2 position, float rotation, Vector2 scale, int frame)
                :this(sprite, position)
            {
                Rotation = rotation;
                Scale = scale;
                Frame = frame;
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

            GL.Enable(EnableCap.DepthTest);

            //enable alpha blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //Define vertex format
            GL.VertexPointer(3, VertexPointerType.Float, 5 * sizeof(float), (IntPtr)(0));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            //enable texturing
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.TextureCoordArray);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

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
                Vector3 cen = new Vector3(instance.Sprite.Center.X, instance.Sprite.Center.Y, 0);
                Vector3 pos = new Vector3(instance.Position.X, instance.Position.Y, 0);
                //create the world matrix
                Matrix4 cent = Matrix4.CreateTranslation(-cen);
                Matrix4 scale = Matrix4.Scale(instance.Scale.X, instance.Scale.Y, 0);
                Matrix4 rotation = Matrix4.CreateRotationZ(instance.Rotation);
                Matrix4 translation = Matrix4.CreateTranslation(pos);

                Matrix4 world = cent * scale * rotation * translation;

                //combine with the view to make modelview
                Matrix4 modelview = world * view;

                //set matrix
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref modelview);

                //set sprite buffers and texture
                instance.Sprite.BindResources();

                //draw the textured quad
                GL.DrawArrays(BeginMode.Quads, instance.Frame * 4, 4);
            }

            GL.PopMatrix();
        }
    }
}
