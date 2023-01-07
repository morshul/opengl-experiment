using experiment.Game.Elements;
using experiment.Game.Platform;
using OpenTK.Graphics.OpenGL;

namespace experiment.Game.Rendering.Sprites;

public class Sprite : Body, ICanHandle<int>
{
    public const float GRAVITY = 800.0f;

    public int Handle { get; private set; } = 0;

    public void SetTexture(int texture)
    {
        if (Handle == texture) return;
        if (texture == 0) return;

        GL.BindTexture(TextureTarget.Texture2D, texture);

        var width = new[] { 0 };
        var height = new[] { 0 };

        GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, width);
        GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight, height);

        Width = width[0];
        Height = height[0];

        Handle = texture;
    }

    public override void Process()
    {
        if (StaticBody) return;

        SpeedY += GRAVITY * Program.ElapsedFrameTime * -1.0f;

        base.Process();
    }

    public override void Draw()
    {
        if (Handle == 0) return;

        // var error = GL.GetError();
        // if (error != ErrorCode.NoError)
        //     Console.WriteLine($"GL ERROR: {error}");

        Program.Renderer?.Draw(Handle,
            (int)Math.Floor(X),
            (int)Math.Floor(Y));
    }
}
