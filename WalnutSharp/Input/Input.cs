using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WalnutSharp.Utils;

namespace WalnutSharp.Input;

// this is from a game engine i made a while back. this is the only good script i made in it.
public static class Input
{
    static readonly BaseInputModule module = new();

    public static Vector2 MouseDelta { get; private set; }
    public static Vector2 ScrollDelta { get; private set; }
    public static Vector2 Scroll { get; private set; }
    public static Vector2 MousePosition { get; private set; }

    internal static void UpdateInput(MouseState mouse, KeyboardState keyboard)
    {
        module.Mouse = mouse;
        module.Keyboard = keyboard;

        for (int i = 0; i < Enum.GetNames<MouseButton>().Length; i++)
        {
            var btn = (MouseButton)i;

            module.mouseStates[btn] = mouse.IsButtonDown(btn);

            module.mousePressedStates[btn] = mouse.IsButtonPressed(btn);

            module.mouseReleasedStates[btn] = mouse.IsButtonReleased(btn);

        }

        module.mouseDelta = VectorUtils.FromTKVec2(mouse.Delta);
        MouseDelta = module.mouseDelta;

        module.mouseScrollDelta = VectorUtils.FromTKVec2(mouse.ScrollDelta);
        ScrollDelta = module.mouseScrollDelta;

        module.mouseScroll = VectorUtils.FromTKVec2(mouse.Scroll);
        Scroll = module.mouseScroll;

        module.mousePosition = VectorUtils.FromTKVec2(mouse.Position);
        MousePosition = module.mousePosition;

        for (int i = 0; i < Enum.GetNames<Keys>().Length; i++)
        {
            var key = (Keys)i;

            module.keyStates[key] = keyboard.IsKeyDown(key);

            module.keyPressedStates[key] = keyboard.IsKeyPressed(key);

            module.keyReleasedStates[key] = keyboard.IsKeyReleased(key);

        }
    }

    /// <summary>Gets the mouse button that is down.</summary>
    /// <param name="button">The button index</param>
    /// <returns>True if the mouse button is down and false otherwise</returns>
    public static bool IsMouseDown(int button) => module.mouseStates[(MouseButton)button];

    /// <summary>Gets the mouse button that was pressed.</summary>
    /// <param name="button">The button index</param>
    /// <returns>True if the mouse button was pressed and false otherwise</returns>
    public static bool WasMousePressed(int button) => module.mousePressedStates[(MouseButton)button];

    /// <summary>Gets the mouse button that was released.</summary>
    /// <param name="button">The button index</param>
    /// <returns>True if the mouse button was released and false otherwise</returns>
    public static bool WasMouseReleased(int button) => module.mouseReleasedStates[(MouseButton)button];

    /// <summary>Gets the keys that is down.</summary>
    /// <param name="key">The key</param>
    /// <returns>True if the key is down and false otherwise</returns>
    public static bool IsKeyDown(Keys key) => module.keyStates[key];

    /// <summary>Gets the keys that was just pressed.</summary>
    /// <param name="key">The key name</param>
    /// <returns>True if the key was just pressed and false otherwise</returns>
    public static bool WasKeyPressed(Keys key) => module.keyPressedStates[key];

    /// <summary>Gets the keys that was just released.</summary>
    /// <param name="key">The key</param>
    /// <returns>True if the key was just released and false otherwise</returns>
    public static bool WasKeyReleased(Keys key) => module.keyReleasedStates[key];

    /// <summary>Gets the keys that is down.</summary>
    /// <param name="key">The key name</param>
    /// <returns>True if the key is down and false otherwise</returns>
    public static bool IsKeyDown(string key)
    {
        if (Enum.TryParse<Keys>(key, true, out var parsedKey))
            return module.keyStates.TryGetValue(parsedKey, out var state) && state;
        return false;
    }

    /// <summary>Gets the keys that was just pressed.</summary>
    /// <param name="key">The key name</param>
    /// <returns>True if the key was just pressed and false otherwise</returns>
    public static bool WasKeyPressed(string key)
    {
        if (Enum.TryParse<Keys>(key, true, out var parsedKey))
            return module.keyPressedStates.TryGetValue(parsedKey, out var state) && state;
        return false;
    }

    /// <summary>Gets the keys that was just released.</summary>
    /// <param name="key">The key name</param>
    /// <returns>True if the key was just released and false otherwise</returns>
    public static bool WasKeyReleased(string key)
    {
        if (Enum.TryParse<Keys>(key, true, out var parsedKey))
            return module.keyReleasedStates.TryGetValue(parsedKey, out var state) && state;
        return false;
    }

    /// <summary>Gets all the current keys that are being pressed.</summary>
    /// <returns>Keys[] a list of the pressed keys.</returns>
    public static Keys[] AllKeysDown()
    {
        List<Keys> result = [];
        foreach (var states in module.keyStates)
        {
            if (states.Value)
                result.Add(states.Key);
            else result.Remove(states.Key);
        }
        return [.. result];
    }

    /// <summary>Gets the value for the axis you input.</summary>
    /// <param name="axis">Types: 'Horizontal', 'Vertical', 'MouseX', 'MouseY'</param>
    /// <returns>A float value for basic wasd and mouse input.</returns>
    public static float GetAxis(string axis) => axis switch
    {
        "Horizontal" => (IsKeyDown(Keys.A) ? 1f : 0f) - (IsKeyDown(Keys.D) ? 1f : 0f),
        "Vertical" => (IsKeyDown(Keys.W) ? 1f : 0f) - (IsKeyDown(Keys.S) ? 1f : 0f),
        "MouseX" => MousePosition.X,
        "MouseY" => MousePosition.Y,
        _ => 0f
    };

    public static void SetCursorMode(CursorMode state)
    {
        var io = ImGui.GetIO();

        switch (state)
        {
            case CursorMode.Normal:
                io.MouseDrawCursor = true;
                break;

            case CursorMode.Hidden:
                io.MouseDrawCursor = false;
                break;
        };
    }
}

public enum CursorMode
{
    Normal,
    Hidden
}
