using System.Diagnostics;
using experiment.Game.Resources;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using static StbImageSharp.StbImage;

namespace experiment.Game.Rendering.Sprites;

public class SpriteLoader
{
    static SpriteLoader()
    {
        stbi_set_flip_vertically_on_load(1);
    }

    public int CreateTexture(string name)
    {
        using var manifestStream = ExperimentResources.Assembly.GetManifestResourceStream($"experiment.Game.Resources.Textures.{name}.png");
        var e = ExperimentResources.Assembly.GetManifestResourceNames();
        Debug.Assert(manifestStream != null, nameof(manifestStream) + " != null");

        var image = ImageResult.FromStream(manifestStream, ColorComponents.RedGreenBlueAlpha);

        var texture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
            image.Width, image.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            image.Data);

        var error = GL.GetError();
        if (error != 0)
            throw new InvalidOperationException($"GL ERROR: {error}");

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);

        return texture;
    }
}
