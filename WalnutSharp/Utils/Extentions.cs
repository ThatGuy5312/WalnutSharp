using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WalnutSharp.Utils;

public static class Extentions
{
    public static Vector3 ToVector(this Color color) => new(color.R, color.G, color.B);

    public static OpenTK.Mathematics.Vector3 ToTK(this Vector3 vec) => new(vec.X, vec.Y, vec.Z);

    public static Vector3 ToSys(this OpenTK.Mathematics.Vector3 vec) => new(vec.X, vec.Y, vec.Z);

    public static OpenTK.Mathematics.Quaternion ToTK(this Quaternion quat) => new(quat.X, quat.Y, quat.Z, quat.W);

    public static Quaternion ToSys(this OpenTK.Mathematics.Quaternion quat) => new(quat.X, quat.Y, quat.Z, quat.W);

    public static OpenTK.Mathematics.Vector4 ToTK(this Vector4 vec) => new(vec.X, vec.Y, vec.Z, vec.W);

    public static Vector4 ToSys(this OpenTK.Mathematics.Vector4 vec) => new(vec.X, vec.Y, vec.Z, vec.W);
}
