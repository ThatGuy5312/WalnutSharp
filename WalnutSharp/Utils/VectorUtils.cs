using OpenTK.Mathematics;
using Num = System.Numerics;

namespace WalnutSharp.Utils;

public static class VectorUtils
{
    public static Vector2 FromSystemVec2(Num.Vector2 vector) => new(vector.X, vector.Y);
    public static Num.Vector2 FromTKVec2(Vector2 vector) => new(vector.X, vector.Y);

    public static Vector3 FromSystemVec3(Num.Vector3 vector) => new(vector.X, vector.Y, vector.Z);
    public static Num.Vector3 FromTKVec3(Vector3 vector) => new(vector.X, vector.Y, vector.Z);

    public static Num.Vector3 TupleToSystemVec((float, float, float) tuple) => new(tuple.Item1, tuple.Item2, tuple.Item3);
    public static Vector3 TupleToTKVec((float, float, float) tuple) => new(tuple.Item1, tuple.Item2, tuple.Item3);
}
