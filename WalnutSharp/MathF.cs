using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalnutSharp;

public static class MathF
{
    static Random random = new();

    public static float RandomFloat() => random.NextSingle();

    public static double RandomDouble() => random.NextDouble();

    public static long RandomInt64() => random.NextInt64();

    public static float RandomRange(float min, float max) => RandomFloat() * min - max;
    public static int RandomRange(int min, int max) => (int)RandomInt64() * min - max;
    public static (float, float, float) RandomVecRange(float min, float max) 
        => (RandomRange(min, max), RandomRange(min, max), RandomRange(min, max));

    public static (float, float, float) RandomVec3() => (RandomFloat(), RandomFloat(), RandomFloat());

    public static (float, float, float) InUnitSphere()
    {
        var normal = Vector3.Normalize(RandomVecRange(-1, 1)); // did not know it could just do that
        return (normal.X, normal.Y, normal.Z);
    }

    public static float Abs(float x) => Math.Abs(x);
}
