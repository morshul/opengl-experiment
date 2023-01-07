using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace experiment.Game.Rendering.Buffers;

public sealed class VertexBuffer : IBuffer<float>
{
    public int Handle { get; private set; }

    private float[]? data;

    public IEnumerable<float> Data => data ?? Array.Empty<float>();

    public VertexBuffer()
    {
        Handle = GL.GenBuffer();
    }

    public void Upload(IEnumerable<float> newData)
    {
        data = newData.ToArray();

        setCapabilities();
    }

    private void setCapabilities()
    {
        Debug.Assert(data != null, nameof(data) + " != null");

        Bind();
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
    }

    public void Bind()
        => GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);

    public void Unbind()
        => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

    private void releaseUnmanagedResources()
    {
        GL.DeleteBuffer(Handle);
    }

    public void Dispose()
    {
        releaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~VertexBuffer()
    {
        releaseUnmanagedResources();
    }
}
