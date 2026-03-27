using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalnutSharp;

public enum ImageFormat
{
    None = 0,
    RGBA,
    RGBA32F
}

// a bit AI, i just removed and changed things to be more how i like to use things
public class Image
{
    public int Handle { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string FilePath { get; private set; } = "";

    private ImageFormat _format;

    public Image(string path)
    {
        FilePath = path;

        using var stream = File.OpenRead(path);
        var img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        Width = img.Width;
        Height = img.Height;
        _format = ImageFormat.RGBA;

        AllocateTexture();
        SetData(img.Data);
    }

    public Image(int width, int height, ImageFormat format, IntPtr data = default)
    {
        Width = width;
        Height = height;
        _format = format;

        AllocateTexture();

        if (data != IntPtr.Zero)
            SetData(data);
    }

    void AllocateTexture()
    {
        Handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, Handle);

        var internalFormat =
            _format == ImageFormat.RGBA32F ? PixelInternalFormat.Rgba32f : PixelInternalFormat.Rgba;

        GL.TexImage2D( TextureTarget.Texture2D, 0, internalFormat, Width, Height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

        GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureWrapS,
            (int)TextureWrapMode.Repeat);

        GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureWrapT,
            (int)TextureWrapMode.Repeat);
    }

    public void SetData(nint data)
    {
        GL.BindTexture(TextureTarget.Texture2D, Handle);

        GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, PixelFormat.Rgba,
            PixelType.UnsignedByte, data);
    }

    public void SetData(byte[] data)
    {
        GL.BindTexture(TextureTarget.Texture2D, Handle);

        GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, PixelFormat.Rgba,
            PixelType.UnsignedByte, data);
    }

    public void Resize(int width, int height)
    {
        if (Width == width && Height == height)
            return;

        Width = width;
        Height = height;

        GL.DeleteTexture(Handle);
        AllocateTexture();
    }

    public void Dispose() => GL.DeleteTexture(Handle);
}
