using ImGuiNET;
using ImGuizmoNET;
// Twizzle.Net ily
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using WalnutSharp;

// project has ImGuizmos if you ever want to make something with that

namespace Example;

public unsafe class Program : RuntimeLayer
{
    // this runtime player program
    public static Program? Instance;

    Image Image;
    // -- CONSTRUCTORS
    // new Image(string) - Creates a new image from a file
    // new Image(int width, int height, ImageFormat format, IntPtr data = default)
    // ^^ Creates a new image with a specified width, height, format, and image data.
    // -- FUNCTIONS
    // SetData(nint) - I recommend to make a byte* with unsafe context enabled and casting to nint
    // SetData(byte[]) - Don't use this unless you uploading a image
    // Resize(int width, int height) - Resized the image
    // Dispose() - Deletes the image texture from OpenGL
    // -- FIELDS
    // Handle - Gets the handle / ID used by this image. Use when setting texture to ImGui image.
    // Width / Height - The width and or height of this image
    // FilePath - The file path that this image was loaded from. Only exist if this was loaded from a file.

    // C# console app entry point
    static void Main()
    {
        // needs a instance of class
        // will find a way to change it in the future
        if (Instance == null)
        {
            Instance = new Program();
            // adds the runtime layer
            // works with multiple layers
            Application.AddRuntime(Instance);
        }

        // already set to this but here just so you know that this method exist
        //
        Application.SetWindowSettings(WindowState.Fullscreen, WindowBorder.Resizable, true);

        // starts the opentk game window
        Application.Start();
    }

    public override void OnUpdate(double time)
    {
        base.OnUpdate(time);

        // remember to add any windows up here
        // or you can put all your windows into on function
        // i recommend making multiple functions inside a different class and then calling them here
        UIWindow();
    }

    public override void OnRender(double time)
    {
        base.OnRender(time);
        // render opengl objects here
    }

    // basic imgui example window
    void UIWindow()
    {
        ImGui.Begin("Example window");

        ImGui.Text("This is a example text!");

        //ImGui.Image(Image.Handle, new(Image.Width, Image.Handle));
        //Creats a image using a image field

        ImGui.End();

        ImGui.ShowDemoWindow();
    }
}