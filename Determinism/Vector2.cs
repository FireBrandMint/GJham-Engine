using System.Drawing;
using SFML.Graphics;

public readonly struct Vector2
{
    public static readonly Vector2 ZERO;

    static Vector2 ()
    {
        ZERO = new Vector2();
    }

    public readonly FInt x,y;

    public Vector2 ()
    {
        x = (FInt) 0;
        y = (FInt) 0;
    }

    public Vector2(FInt x_, FInt y_)
    {
        x=x_;
        y=y_;
    }

    public Vector2(int x_, int y_)
    {
        x = (FInt)x_;
        y = (FInt)y_;
    }

    public Vector2 Create()
    {
        return new Vector2(FInt.Create(0), FInt.Create(0));
    }

    public static Vector2 Lerp (Vector2 v1, Vector2 v2, FInt t)
    {
        FInt x = DeterministicMath.Lerp(v1.x, v2.x, t);
        FInt y = DeterministicMath.Lerp(v1.y, v2.y, t);

        return new Vector2(x, y);
    }

    public Vector2 Lerp (Vector2 v2, FInt t)
    {
        FInt x = DeterministicMath.Lerp(this.x, v2.x, t);
        FInt y = DeterministicMath.Lerp(this.y, v2.y, t);

        return new Vector2(x, y);
    }

    //The direction of angle of v1 pointing at v2
    public Vector2 AngleVector (Vector2 v1, Vector2 v2)
    {
        return (v2 - v1).Normalized();
    }

    public bool InRangeSquared(Vector2 v1, Vector2 v2, FInt range)
    {
        FInt dx = v1.x - v1.x;
        FInt dy = v1.y - v1.y;

        return dx*dx + dy*dy <=  range * range;
    }

    public FInt Length ()
    {
        return DeterministicMath.Sqrt(x*x + y*y);
    }

    public Vector2 Normalized()
    {
        FInt length = Length();
        if (length == 0)
        {
            return Vector2.ZERO;
        }
        else
        {
            return this / length;
        }
    }

    public override string ToString()
    {
        return x.ToString() + ':' + y.ToString();
    }

    public static Vector2 Parse (string s)
    {
        var arr = s.Split(':');

        return new Vector2(FInt.Parse(arr[0]), FInt.Parse(arr[1]));
    }


    public static Vector2 operator + (Vector2 v1, Vector2 v2)
    {
        return new Vector2 (v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2 operator - (Vector2 v1, Vector2 v2)
    {
        return new Vector2 (v1.x - v2.x, v1.y - v2.y);
    }

    public static Vector2 operator * (Vector2 v1, Vector2 v2)
    {
        return new Vector2 (v1.x * v2.x, v1.y * v2.y);
    }

    public static Vector2 operator * (Vector2 v1, FInt d2)
    {
        return new Vector2 (v1.x * d2, v1.y * d2);
    }

    public static Vector2 operator / (Vector2 v1, Vector2 v2)
    {
        return new Vector2 (v1.x / v2.x, v1.y / v2.y);
    }

    public static Vector2 operator / (Vector2 v1, FInt d2)
    {
        return new Vector2 (v1.x / d2, v1.y / d2);
    }

    public PointF ToPoint ()
    {
        return new PointF(x.ToFloat(), y.ToFloat());
    }

    public Vertex ToVertex ()
    {
        return new Vertex(new SFML.System.Vector2f(x.ToFloat(), y.ToFloat()));
    }
}