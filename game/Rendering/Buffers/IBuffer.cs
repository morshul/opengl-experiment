using experiment.Game.Platform;

namespace experiment.Game.Rendering.Buffers;

public interface IBuffer<out T> : ICanHandle<int>, IDisposable
{
    IEnumerable<T> Data { get; }

    void Bind();

    void Unbind();
}
