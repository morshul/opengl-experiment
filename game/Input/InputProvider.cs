using OpenTK.Windowing.GraphicsLibraryFramework;

namespace experiment.Game.Input;

public static unsafe class InputProvider
{
    private static Window* handle;

    public static void SetHandle(Window* window)
        => handle = window;

    public static bool IsKeyPressed(Keys key)
        => GLFW.GetKey(handle, key) == InputAction.Press;

    public static bool IsKeyReleased(Keys key)
        => GLFW.GetKey(handle, key) == InputAction.Release;
}
