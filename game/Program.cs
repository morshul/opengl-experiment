using System.Runtime.InteropServices;
using experiment.Game.Elements;
using experiment.Game.Input;
using experiment.Game.Physics;
using experiment.Game.Rendering;
using experiment.Game.Rendering.Sprites;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace experiment.Game;

public sealed unsafe class Program : IDisposable
{
    public static int Width = 1366;
    public static int Height = 768;

    public static float ElapsedFrameTime { get; internal set; } = 1.0f / 60.0f;

    private Window* glfwWindow;

    public static SpriteRenderer? Renderer { get; private set; }
    public SpriteLoader? Loader { get; private set; }

    private List<Sprite>? bodies;

    private GCHandle debugCallbackHandle;

    public static void Main()
    {
        using var program = new Program();
        program.run();
    }

    private void run()
    {
        if (!createCapabilities()) return;

        Renderer = new SpriteRenderer();
        Loader = new SpriteLoader();
        InputProvider.SetHandle(glfwWindow);

        bodies = new List<Sprite>();

        GLFW.SetFramebufferSizeCallback(glfwWindow, (window, width, height) =>
        {
            Width = width;
            Height = height;
        });

        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);

        GL.Disable(EnableCap.DepthTest);

        gameSetup();

        while (!GLFW.WindowShouldClose(glfwWindow))
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);

            eventLoop();

            GLFW.PollEvents();
            GLFW.SwapBuffers(glfwWindow);
        }
    }

    private bool createCapabilities()
    {
        GLFW.SetErrorCallback((error, description) =>
            throw new InvalidOperationException("An error occurred while running GLFW.",
                new GLFWException(description, error)));

        if (!GLFW.Init()) return false;

        GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 4);
        GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 6);
        GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

        glfwWindow = GLFW.CreateWindow(1366, 768, "OPENGL EXPERIMENT", null, null);

        GLFW.MakeContextCurrent(glfwWindow);
        GL.LoadBindings(new GLFWBindingsContext());

        debugCallbackHandle = GCHandle.Alloc(debugCallback);
        GL.DebugMessageCallback(debugCallback, nint.Zero);

        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);

        return glfwWindow != null;
    }

    private void gameSetup()
    {
        var grassTexture = Loader!.CreateTexture("grass_mid");
        var playerTexture = Loader!.CreateTexture("p1_front");
        var brickTexture = Loader!.CreateTexture("brick_wall");
        var bridgeTexture = Loader!.CreateTexture("bridge");

        for (var i = 0; i < 24; i++)
        {
            var grass = new Sprite();

            grass.SetTexture(grassTexture);
            grass.StaticBody = true;
            grass.X = grass.Width * i;
            bodies?.Add(grass);
        }

        var player = new PlayerSprite(bridgeTexture)
        {
            X = 50.0f,
            Y = 100.0f,
            StaticBody = false
        };
        player.SetTexture(playerTexture);

        bodies?.Add(player);

        GLFW.SetKeyCallback(glfwWindow, (_, key, code, action, _) =>
        {
            if (key == Keys.Space && action == InputAction.Press)
                player.SpeedY += 300.0f;
        });

        {
            var brick = new Sprite
            {
                X = 150.0f,
                Y = 80.0f,
                StaticBody = true
            };
            brick.SetTexture(brickTexture);

            bodies?.Add(brick);
        }

        {
            var brick = new Sprite
            {
                X = 250.0f,
                Y = 150.0f,
                StaticBody = true
            };
            brick.SetTexture(brickTexture);

            bodies?.Add(brick);
        }

        {
            var brick = new Sprite
            {
                X = 350.0f,
                Y = 250.0f,
                StaticBody = true
            };
            brick.SetTexture(brickTexture);

            bodies?.Add(brick);
        }

        var bridge = new Sprite
        {
            X = 480.0f,
            Y = 100.0f,
            StaticBody = true
        };
        bridge.SetTexture(bridgeTexture);

        bodies?.Add(bridge);
    }

    private void eventLoop()
    {
        var b = bodies!.ToArray();

        foreach (IUpdatable updatable in b)
            updatable.Process();

        var toTest = new AABB();

        foreach (var bodyA in bodies.Where(bodyA => !bodyA.StaticBody))
        {
            toTest.Width = bodyA.Width;
            toTest.Height = bodyA.Height;

            foreach (var bodyB in bodies.Where(bodyB => bodyA != bodyB))
            {
                toTest.X = bodyA.X + bodyA.SpeedX * ElapsedFrameTime;
                toTest.Y = bodyA.Y;

                if (toTest.Intersects(bodyB))
                    bodyA.WillCollideWith(bodyB, true);

                toTest.X = bodyA.X;
                toTest.Y = bodyA.Y * bodyA.SpeedY * ElapsedFrameTime;
                if (toTest.Intersects(bodyB))
                    bodyA.WillCollideWith(bodyB, false);
            }
        }

        foreach (var body in bodies)
            body.Move();

        foreach (IDrawable drawable in bodies)
            drawable.Draw();
    }

    private void debugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, nint message, nint param)
    {
        var messageString = Marshal.PtrToStringAnsi(message, length);

        Console.WriteLine($"{severity} {type} | {messageString}");

        if (type == DebugType.DebugTypeError)
            throw new InvalidOperationException(messageString);
    }

    private void releaseUnmanagedResources()
    {
        GLFW.DestroyWindow(glfwWindow);
        GLFW.Terminate();
    }

    public void Dispose()
    {
        releaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Program()
    {
        releaseUnmanagedResources();
    }
}
