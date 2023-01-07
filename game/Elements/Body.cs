using experiment.Game.Physics;
using experiment.Game.Rendering;

namespace experiment.Game.Elements;

public class Body : AABB, IDrawable, IUpdatable
{
    public float SpeedX { get; set; }

    public float SpeedY { get; set; }

    public bool StaticBody { get; set; }

    public void Move()
    {
        if (StaticBody) return;

        X += SpeedX * Program.ElapsedFrameTime;
        Y += SpeedY * Program.ElapsedFrameTime;
    }

    public virtual void WillCollideWith(Body other, bool xAxis)
    {
        if (StaticBody) return;

        if (xAxis) SpeedX = 0.0f;
        else SpeedY = 0.0f;
    }

    public virtual void Process()
    {
    }

    public virtual void Draw()
    {
    }
}
