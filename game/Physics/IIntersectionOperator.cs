namespace experiment.Game.Physics;

public interface IIntersectionOperator<in T>
{
    bool Intersects(T other);
}
