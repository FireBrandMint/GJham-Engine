using SFML.System;
using SFML.Graphics;

public class RenderMath
{
    public static float Lerp(float a, float b, float t)
    {
        return (1f - t) * a + t * b;
    }

    public static Vector2f Lerp (Vector2f a, Vector2f b, float t)
    {
        return new Vector2f(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
    }

    public static Vector2f Multiply (Vector2f a, Vector2f b)
    {
        return new Vector2f(a.X * b.X, a.Y * b.Y);
    }
}