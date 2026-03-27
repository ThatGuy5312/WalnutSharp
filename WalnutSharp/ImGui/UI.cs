using ImGuiNET;
using OpenTK.Compute.OpenCL;
using OpenTK.Core.Native;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection.Metadata;
using System;
using System.Runtime.InteropServices;

namespace GUI;

// this was from my other project and this class is from chat gpt.
internal class UI : IDisposable
{
    private GameWindow _window;

    private int _vertexArray;
    private int _vertexBuffer;
    private int _indexBuffer;
    private int _shaderProgram;
    private int _fontTexture;

    public UI(GameWindow window)
    {
        _window = window;

        ImGui.CreateContext();
        ImGui.StyleColorsDark();
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.Fonts.Clear();
        io.Fonts.AddFontFromFileTTF("C:/Windows/Fonts/arial.ttf", 18f);

        _window.MouseMove += Window_MouseMove;
        _window.MouseDown += Window_MouseDown;
        _window.MouseUp += Window_MouseUp;
        _window.MouseWheel += Window_MouseWheel;
        _window.KeyDown += Window_KeyDown;
        _window.KeyUp += Window_KeyUp;
        _window.TextInput += Window_TextInput;

        BuildFontAtlas();
        SetupBuffers();
        SetupShaders();
    }

    #region Input
    private void Window_MouseMove(MouseMoveEventArgs e)
    {
        ImGui.GetIO().MousePos = new System.Numerics.Vector2(e.X, e.Y);
    }

    private void Window_MouseDown(MouseButtonEventArgs e)
    {
        var io = ImGui.GetIO();
        if (e.Button == MouseButton.Left) io.MouseDown[0] = true;
        if (e.Button == MouseButton.Right) io.MouseDown[1] = true;
        if (e.Button == MouseButton.Middle) io.MouseDown[2] = true;
    }

    private void Window_MouseUp(MouseButtonEventArgs e)
    {
        var io = ImGui.GetIO();
        if (e.Button == MouseButton.Left) io.MouseDown[0] = false;
        if (e.Button == MouseButton.Right) io.MouseDown[1] = false;
        if (e.Button == MouseButton.Middle) io.MouseDown[2] = false;
    }

    private void Window_MouseWheel(MouseWheelEventArgs e)
    {
        ImGui.GetIO().MouseWheel = e.OffsetY;
        ImGui.GetIO().MouseWheelH = e.OffsetX;
    }

    private void Window_KeyDown(KeyboardKeyEventArgs e)
    {
        ImGuiKey key = MapKey(e.Key);
        if (key != ImGuiKey.None)
            ImGui.GetIO().AddKeyEvent(key, true);
    }

    private void Window_KeyUp(KeyboardKeyEventArgs e)
    {
        ImGuiKey key = MapKey(e.Key);
        if (key != ImGuiKey.None)
            ImGui.GetIO().AddKeyEvent(key, false);
    }

    private ImGuiKey MapKey(Keys key)
    {
        return key switch
        {
            Keys.Tab => ImGuiKey.Tab,
            Keys.Left => ImGuiKey.LeftArrow,
            Keys.Right => ImGuiKey.RightArrow,
            Keys.Up => ImGuiKey.UpArrow,
            Keys.Down => ImGuiKey.DownArrow,
            Keys.PageUp => ImGuiKey.PageUp,
            Keys.PageDown => ImGuiKey.PageDown,
            Keys.Home => ImGuiKey.Home,
            Keys.End => ImGuiKey.End,
            Keys.Insert => ImGuiKey.Insert,
            Keys.Delete => ImGuiKey.Delete,
            Keys.Backspace => ImGuiKey.Backspace,
            Keys.Space => ImGuiKey.Space,
            Keys.Enter => ImGuiKey.Enter,
            Keys.Escape => ImGuiKey.Escape,
            Keys.A => ImGuiKey.A,
            Keys.C => ImGuiKey.C,
            Keys.V => ImGuiKey.V,
            Keys.X => ImGuiKey.X,
            Keys.Y => ImGuiKey.Y,
            Keys.Z => ImGuiKey.Z,
            _ => ImGuiKey.None
        };
    }

    private void Window_TextInput(TextInputEventArgs e)
    {
        ImGui.GetIO().AddInputCharactersUTF8(e.AsString);
    }
    #endregion

    #region Setup

    void BuildFontAtlas()
    {
        var io = ImGui.GetIO();
        io.Fonts.AddFontDefault();
        IntPtr pixels;
        int width, height, bytesPerPixel;
        io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);

        _fontTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        io.Fonts.SetTexID((IntPtr)_fontTexture);
        io.Fonts.ClearTexData();
    }

    private void SetupBuffers()
    {
        _vertexArray = GL.GenVertexArray();
        _vertexBuffer = GL.GenBuffer();
        _indexBuffer = GL.GenBuffer();

        GL.BindVertexArray(_vertexArray);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);

        int stride = Marshal.SizeOf<ImDrawVert>();
        GL.EnableVertexAttribArray(0); // pos
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, Marshal.OffsetOf<ImDrawVert>("pos"));
        GL.EnableVertexAttribArray(1); // uv
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, Marshal.OffsetOf<ImDrawVert>("uv"));
        GL.EnableVertexAttribArray(2); // col
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, Marshal.OffsetOf<ImDrawVert>("col"));

        GL.BindVertexArray(0);
    }

    private void SetupShaders()
    {
        string vertexSrc = @"
#version 330 core
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aUV;
layout(location = 2) in vec4 aColor;

uniform mat4 projection;
out vec2 vUV;
out vec4 vColor;

void main()
{
    vUV = aUV;
    vColor = aColor;
    gl_Position = projection * vec4(aPos, 0.0, 1.0);
}";

        string fragmentSrc = @"
#version 330 core
in vec2 vUV;
in vec4 vColor;
uniform sampler2D texture0;
out vec4 FragColor;
void main()
{
    FragColor = vColor * texture(texture0, vUV);
}";

        int vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, vertexSrc);
        GL.CompileShader(vs);
        GL.GetShader(vs, ShaderParameter.CompileStatus, out int vStatus);
        if (vStatus != 1) throw new Exception(GL.GetShaderInfoLog(vs));

        int fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, fragmentSrc);
        GL.CompileShader(fs);
        GL.GetShader(fs, ShaderParameter.CompileStatus, out int fStatus);
        if (fStatus != 1) throw new Exception(GL.GetShaderInfoLog(fs));

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vs);
        GL.AttachShader(_shaderProgram, fs);
        GL.LinkProgram(_shaderProgram);
        GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out int linkStatus);
        if (linkStatus != 1) throw new Exception(GL.GetProgramInfoLog(_shaderProgram));

        GL.DeleteShader(vs);
        GL.DeleteShader(fs);
    }
    #endregion

    #region Render
    public void Update(float deltaSeconds)
    {
        var io = ImGui.GetIO();
        io.DisplaySize = new System.Numerics.Vector2(_window.Size.X, _window.Size.Y);
        ImGui.NewFrame();
    }

    public void Render()
    {
        ImGui.Render();
        RenderImDrawData(ImGui.GetDrawData());
    }

    private unsafe void RenderImDrawData(ImDrawDataPtr drawData)
    {
        if (drawData.CmdListsCount == 0) return;

        GL.Enable(EnableCap.Blend);
        GL.BlendEquation(BlendEquationMode.FuncAdd);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.ScissorTest);

        GL.UseProgram(_shaderProgram);
        var ortho = Matrix4.CreateOrthographicOffCenter(0f, _window.Size.X, _window.Size.Y, 0f, -1f, 1f);
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref ortho);
        GL.BindVertexArray(_vertexArray);

        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            var cmdList = drawData.CmdListsRange[n];
            int vertexSize = Marshal.SizeOf<ImDrawVert>();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, cmdList.VtxBuffer.Size * vertexSize, (nint)cmdList.VtxBuffer.Data, BufferUsageHint.StreamDraw);

            int indexSize = sizeof(ushort);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, cmdList.IdxBuffer.Size * indexSize, (nint)cmdList.IdxBuffer.Data, BufferUsageHint.StreamDraw);

            int idxOffset = 0;
            for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
            {
                var pcmd = cmdList.CmdBuffer[cmdi];
                GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.GetTexID());
                GL.Scissor(
                    (int)pcmd.ClipRect.X,
                    (int)(_window.Size.Y - pcmd.ClipRect.W),
                    (int)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                    (int)(pcmd.ClipRect.W - pcmd.ClipRect.Y)
                );
                GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount,
                    DrawElementsType.UnsignedShort, (idxOffset * indexSize), 0);
                idxOffset += (int)pcmd.ElemCount;
            }
        }

        GL.Disable(EnableCap.ScissorTest);
    }
    #endregion


    public void Dispose()
    {
        GL.DeleteVertexArray(_vertexArray);
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteBuffer(_indexBuffer);
        GL.DeleteTexture(_fontTexture);
        GL.DeleteProgram(_shaderProgram);
    }
}