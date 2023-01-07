using experiment.Game.Input;
using experiment.Game.Rendering.Sprites;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace experiment.Game.Elements;

public class PlayerSprite : Sprite
{
    public int BridgeTexture { get; private set; }

    public PlayerSprite(int bridgeTexture)
    {
        BridgeTexture = bridgeTexture;
    }

    public override void WillCollideWith(Body other, bool xAxis)
    {
        if (other is not Sprite e) return;

        if (e.Handle == BridgeTexture && Math.Abs(SpeedY) > 100.0f && !xAxis)
            SpeedY *= -0.75f;
        else
            base.WillCollideWith(other, xAxis);
    }

    public override void Process()
    {
        base.Process();

        var speed = 0.0f;

        if (InputProvider.IsKeyPressed(Keys.A))
            speed -= 300.0f;

        if (InputProvider.IsKeyPressed(Keys.D))
            speed += 300.0f;

        SpeedX = speed;

        if (!(Y < -200.0f)) return;

        X = 50.0f;
        Y = 100.0f;
    }
}
