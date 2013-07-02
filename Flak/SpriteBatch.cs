using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flak
{
    public class SpriteBatch
    {
        public class RenderDetails
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

                    //wrap out of range values
                    if (frame < 0)
                        frame = Sprite.Frames + (frame % Sprite.Frames);
                    if (frame >= Sprite.Frames)
                        frame = (frame % Sprite.Frames);
                }
            }

            public RenderDetails(Sprite sprite, Vector2 position)
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

        public SpriteBatch()
        {
            renderInstances = new Queue<RenderDetails>();
            view = Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
            ConfigureProjection();
        }

        public void ConfigureProjection()
        {
            //create orthographic projection since we're in 2d
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            projection = Matrix4.CreateOrthographicOffCenter(0, viewport[2], viewport[3], 0, -10, 1000);
        }

        public void Begin()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadMatrix(ref projection);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadMatrix(ref view);

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
            GL.MatrixMode(MatrixMode.Modelview);

            while (renderInstances.Count > 0)
            {
                RenderDetails instance = renderInstances.Dequeue();
                //get final position of the quad
                Vector3 cen = new Vector3(instance.Sprite.Center.X, instance.Sprite.Center.Y, 0);
                Vector3 pos = new Vector3((float)Math.Round(instance.Position.X), (float)Math.Round(instance.Position.Y), 0);
                //create the world matrix
                Matrix4 cent = Matrix4.CreateTranslation(-cen);
                Matrix4 scale = Matrix4.Scale(instance.Scale.X, instance.Scale.Y, 0);
                Matrix4 rotation = Matrix4.CreateRotationZ(instance.Rotation);
                Matrix4 translation = Matrix4.CreateTranslation(pos);

                Matrix4 world = cent * scale * rotation * translation;
          
                GL.PushMatrix();
                GL.MultMatrix(ref world);

                //set sprite buffers and texture
                instance.Sprite.BindResources();

                //draw the textured quad
                GL.DrawArrays(BeginMode.Quads, instance.Frame * 4, 4);

                GL.PopMatrix();
            }

            //return matrix state to the state it was at begin       
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
        }
    }
}
