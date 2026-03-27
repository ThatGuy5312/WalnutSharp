using OpenTK.Windowing.Desktop;
using GUI;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using ImGuiNET;
using WalnutSharp.Input;
using System.Diagnostics;

namespace WalnutSharp;

public class Application : GameWindow
{
    public Application(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {
        ui = new UI(this);
    }

    public static string Title = "New Example";
    internal static WindowState WindowState = WindowState.Fullscreen;
    internal static WindowBorder WindowBorder = WindowBorder.Resizable;
    internal static bool Dockspace = false;

    protected override void OnLoad()
    {
        base.OnLoad();

        var io = ImGui.GetIO();
        io.MouseDrawCursor = true;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

        foreach (var layers in RuntimeLayers)
            layers.OnStart();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        Input.Input.UpdateInput(MouseState, KeyboardState);

        foreach (var layers in RuntimeLayers)
            layers.OnUpdate(args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        ui.Update((float)args.Time);

        if (Dockspace)
            RenderDockspace();

        foreach (var layers in RuntimeLayers)
            layers.OnRender(args.Time);

        ui.Render();
        SwapBuffers();
    }

    public static void Start()
    {
        var nativeSettings = new NativeWindowSettings()
        {
            Title = Title,
            Flags = ContextFlags.ForwardCompatible,
            WindowState = WindowState,
            WindowBorder = WindowBorder,
        };

        using var game = new Application(GameWindowSettings.Default, nativeSettings);
        game.Run();
    }

    public static void SetWindowSettings(WindowState state, WindowBorder border, bool dockspace = true)
    {
        WindowState = state;
        WindowBorder = border;
        Dockspace = dockspace;
    }

    public static void AddRuntime(RuntimeLayer layer)
    {
        if (!RuntimeLayers.Contains(layer))
            RuntimeLayers.Add(layer);
        layer.OnInit();
    }

    public static void Exit()
    {
        RuntimeLayers.Clear();
        ui.Dispose();
        Process.GetCurrentProcess().Kill();
    }

    static void RenderDockspace()
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.Pos);
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowViewport(viewport.ID);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);

        var dockFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                        ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus |
                        ImGuiWindowFlags.MenuBar;

        ImGui.Begin("DockSpace", dockFlags);
        var dockspaceID = ImGui.GetID("MainDockSpace");
        ImGui.DockSpace(dockspaceID, new(0, 0), ImGuiDockNodeFlags.PassthruCentralNode);
        ImGui.End();
        ImGui.PopStyleVar(2);
    }

    internal static List<RuntimeLayer> RuntimeLayers = [];

    static UI ui;
}
