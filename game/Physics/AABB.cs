namespace experiment.Game.Physics;

public class AABB : IIntersectionOperator<AABB>
{
    public float Width { get; set; }

    public float Height { get; set; }

    public float X { get; set; }

    public float Y { get; set; }

    public AABB()
        : this(0.0f, 0.0f, 0.0f, 0.0f)
    {
    }

    public AABB(float width, float height, float x, float y)
    {
        Width = width;
        Height = height;
        X = x;
        Y = y;
    }

    public bool Intersects(AABB other)
    {
        var selfMinX = X;
        var selfMinY = Y;
        var selfMaxX = X + Width;
        var selfMaxY = Y + Height;

        var otherMinX = other.X;
        var otherMinY = other.Y;
        var otherMaxX = other.X + other.Width;
        var otherMaxY = other.Y + other.Height;

        return selfMinX <= otherMaxX &&
               selfMaxX >= otherMinX &&
               selfMinY <= otherMaxY &&
               selfMaxY >= otherMinY;
    }
}
