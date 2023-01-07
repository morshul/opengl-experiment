using System.Diagnostics;
using experiment.Game.Platform;
using experiment.Game.Resources;
using OpenTK.Graphics.OpenGL;

namespace experiment.Game.Rendering;

public sealed class Shader : ICanHandle<int>, IDisposable
{
    public int Handle { get; private set; }

    public readonly string VertexShaderSource;
    public readonly string FragmentShaderSource;

    public Shader(string vertexShaderSource, string fragmentShaderSource)
    {
        VertexShaderSource = vertexShaderSource;
        FragmentShaderSource = VertexShaderSource;
    }

    public Shader(string? name = "shader")
    {
        {
            using var manifestStream = ExperimentResources.Assembly.GetManifestResourceStream($"experiment.Game.Resources.Shaders.{name}.vertex.glsl");
            Debug.Assert(manifestStream != null, nameof(manifestStream) + " != null");

            using var streamReader = new StreamReader(manifestStream);
            VertexShaderSource = streamReader.ReadToEnd();
        }

        {
            using var manifestStream = ExperimentResources.Assembly.GetManifestResourceStream($"experiment.Game.Resources.Shaders.{name}.fragment.glsl");
            Debug.Assert(manifestStream != null, nameof(manifestStream) + " != null");

            using var streamReader = new StreamReader(manifestStream);
            FragmentShaderSource = streamReader.ReadToEnd();
        }

        compileShaders();
    }

    public void Bind()
        => GL.UseProgram(Handle);

    public void Unbind()
        => GL.UseProgram(0);

    private void compileShaders()
    {
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, VertexShaderSource);
        GL.CompileShader(vertexShader);
        throwShaderErrors(vertexShader);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, FragmentShaderSource);
        GL.CompileShader(fragmentShader);
        throwShaderErrors(fragmentShader);

        linkProgram(vertexShader, fragmentShader);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    private void linkProgram(int vertex, int fragment)
    {
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertex);
        GL.AttachShader(Handle, fragment);

        GL.LinkProgram(Handle);
        throwProgramErrors();
    }

    private void throwShaderErrors(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);

        if (status == 1) return;

        GL.GetShaderInfoLog(shader, out var message);
        throw new InvalidProgramException($"Couldn't compile shader: {message}");
    }

    private void throwProgramErrors()
    {
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var status);

        if (status == 1) return;

        GL.GetProgramInfoLog(Handle, out var message);
        throw new InvalidProgramException($"Couldn't link program: {message}");
    }

    private void releaseUnmanagedResources()
    {
        GL.DeleteProgram(Handle);
    }

    public void Dispose()
    {
        releaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Shader()
    {
        releaseUnmanagedResources();
    }
}
