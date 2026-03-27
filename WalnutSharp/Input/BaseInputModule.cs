using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WalnutSharp.Input;

public class BaseInputModule
{
    public BaseInputModule() { }

    public MouseState? Mouse { get; internal set; }
    public KeyboardState? Keyboard { get; internal set; }

    public Dictionary<MouseButton, bool> mouseStates = [];
    public Dictionary<MouseButton, bool> mousePressedStates = [];
    public Dictionary<MouseButton, bool> mouseReleasedStates = [];

    public Vector2 mouseDelta;
    public Vector2 mouseScrollDelta;
    public Vector2 mouseScroll;
    public Vector2 mousePosition;

    public Dictionary<Keys, bool> keyStates = [];
    public Dictionary<Keys, bool> keyPressedStates = [];
    public Dictionary<Keys, bool> keyReleasedStates = [];
}
