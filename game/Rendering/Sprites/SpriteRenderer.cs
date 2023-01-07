using experiment.Game.Rendering.Buffers;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace experiment.Game.Rendering.Sprites;

public sealed class SpriteRenderer : IDisposable
{
    public readonly Shader Shader;

    public int VertexArrayHandle { get; private set; }

    private VertexBuffer? buffer;

    public SpriteRenderer()
    {
        Shader = new Shader();
        VertexArrayHandle = createVertexArrays();
    }

    private int createVertexArrays()
    {
        var vao = GL.GenVertexArray();

        GL.BindVertexArray(vao);
        {
            buffer = new VertexBuffer();
            buffer.Upload(new[]
            {
                -1.0f, 1.0f,
                -1.0f, -1.0f,
                1.0f, -1.0f,

                -1.0f, 1.0f,
                1.0f, -1.0f,
                1.0f, 1.0f
            });

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
            buffer.Unbind();
        }
        GL.BindVertexArray(0);

        return vao;
    }

    public void Draw(int spriteTexture, int x, int y)
    {
        Shader.Bind();

        var viewportLocation = GL.GetUniformLocation(Shader.Handle, "u_Viewport");
        var positionLocation = GL.GetUniformLocation(Shader.Handle, "u_Position");
        var spriteLocation = GL.GetUniformLocation(Shader.Handle, "u_Sprite");

        GL.Uniform2(viewportLocation, new Vector2(Program.Width, Program.Height));
        GL.Uniform2(positionLocation, new Vector2(x, y));
        GL.Uniform1(spriteLocation, 0);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, spriteTexture);

        GL.BindVertexArray(VertexArrayHandle);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

        Shader.Unbind();
        GL.BindVertexArray(0);
    }

    private void releaseUnmanagedResources()
    {
        GL.DeleteVertexArray(VertexArrayHandle);
        buffer?.Dispose();
        Shader.Dispose();
    }

    private void dispose(bool disposing)
    {
        releaseUnmanagedResources();

        if (disposing)
        {
            Shader.Dispose();
        }
    }

    public void Dispose()
    {
        dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SpriteRenderer()
    {
        dispose(false);
    }
}
