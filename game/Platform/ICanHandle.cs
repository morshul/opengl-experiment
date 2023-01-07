namespace experiment.Game.Platform;

public interface ICanHandle<out T>
    where T : unmanaged
{
    T Handle { get; }
}
