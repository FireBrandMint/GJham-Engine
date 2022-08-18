using System;
using System.Drawing;
using SFML.Graphics;
using SFML.System;
using System.Runtime.InteropServices;

///<summary>
///Deterministic Vector2D.
///</summary>

[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector2
{
    public static readonly Vector2 ZERO;

    static Vector2 ()
    {
        ZERO = new Vector2();
    }

    public readonly FInt x,y;

    public FInt this[int index]
    {
        get
        {
            switch(index)
            {
                case 0: return x;
                case 1: return y;
            }

            throw new IndexOutOfRangeException();
        }
    }

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

    public Vector2(int x_, FInt y_)
    {
        x=(FInt)x_;
        y=y_;
    }

    public Vector2(FInt x_, int y_)
    {
        x=x_;
        y= (FInt)y_;
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

    public static FInt DotProduct (Vector2 normal, Vector2 pt2)
    {
        //FInt
        //x = normal.x * pt2.x,
        //y = normal.y * pt2.y;
        return normal.x * pt2.x + normal.y * pt2.y;
    }

    public static bool InRange(Vector2 v1, Vector2 v2, FInt range)
    {
        FInt dx = v1.x - v2.x;
        FInt dy = v1.y - v2.y;

        return dx*dx + dy*dy <=  range * range;
    }

    public static bool InRangeSquared(Vector2 v1, Vector2 v2, FInt range)
    {
        FInt dx = v1.x - v2.x;
        FInt dy = v1.y - v2.y;

        return dx*dx + dy*dy <=  range * range;
    }

    /// <summary>
    /// Same thing as Hypot2.
    /// </summary>
    public static FInt DistanceSquared(Vector2 v1, Vector2 v2)
    {
        FInt dx = v1.x - v2.x;
        FInt dy = v1.y - v2.y;

        return dx*dx + dy*dy;
    }

    public static FInt Distance(Vector2 v1, Vector2 v2)
    {
        FInt dx = v1.x - v2.x;
        FInt dy = v1.y - v2.y;

        return DeterministicMath.Sqrt(dx*dx + dy*dy);
    }
    /// <summary>
    /// Distance of line against a 'point'.
    /// </summary>
    public static FInt LinePointDistSqr(Vector2 linePt1, Vector2 linePt2, Vector2 point)
    {
        //From: https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm

        var AC = point - linePt1;
        //Only way AB is 0 is if both line points are in the same place, AKA it's a point
        var AB = linePt2 - linePt1;

        var ZERO_ZERO = new FInt(0);

        //Is 0?
        bool ABx0 = AB.x == ZERO_ZERO;
        bool ABy0 = AB.y == ZERO_ZERO;

        //If it's a point them just return its distance
        if(ABx0 && ABy0) return DistanceSquared(linePt1, point);

        // Get point D by taking the projection of AC onto AB then adding the offset of linePt1
        //There's no chance that dotAB is 0
        var dotAB = Vector2.DotProduct(AB, AB);
        var dotACAB = Vector2.DotProduct(AC, AB);

        FInt kp = dotACAB / dotAB;

        var D = AB * kp + linePt1;

        var AD = D - linePt1;
        // D might not be on AB so calculate k of D down AB (aka solve AD = k * AB)
        // We can use either component, but choose larger value to eliminate the chance of dividing by zero
        // since the 'ABx0 && ABy0' if statement made sure one of the values of AB is not 0.
        FInt k = DeterministicMath.Abs(AB.x) > DeterministicMath.Abs(AB.y) ? AD.x / AB.x : AD.y / AB.y;

        // Check if D is off either end of the line segment

        if (k <= 0) {
            return DistanceSquared(point, linePt1);
        } else if (k >= 1) {
            return DistanceSquared(point, linePt2);
        }

        return DistanceSquared(point, D);
    }

    public static FInt LinePointDistSqr(Vector2 linePt1, Vector2 linePt2, Vector2 point, out Vector2 mesurementPoint)
    {
        //From: https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm

        var AC = point - linePt1;
        //Only way AB is 0 is if both line points are in the same place, AKA it's a point
        var AB = linePt2 - linePt1;

        var ZERO_ZERO = new FInt(0);

        //Is 0?
        bool ABx0 = AB.x == ZERO_ZERO;
        bool ABy0 = AB.y == ZERO_ZERO;

        //If it's a point them just return its distance
        if(ABx0 && ABy0)
        {
            mesurementPoint = linePt1;
            return DistanceSquared(linePt1, point);
        }

        // Get point D by taking the projection of AC onto AB then adding the offset of linePt1
        //There's no chance that dotAB is 0
        var dotAB = Vector2.DotProduct(AB, AB);
        var dotACAB = Vector2.DotProduct(AC, AB);

        FInt kp = dotACAB / dotAB;

        var D = AB * kp + linePt1;

        var AD = D - linePt1;
        // D might not be on AB so calculate k of D down AB (aka solve AD = k * AB)
        // We can use either component, but choose larger value to eliminate the chance of dividing by zero
        // since the 'ABx0 && ABy0' if statement made sure one of the values of AB is not 0.
        FInt k = DeterministicMath.Abs(AB.x) > DeterministicMath.Abs(AB.y) ? AD.x / AB.x : AD.y / AB.y;

        // Check if D is off either end of the line segment

        if (k <= 0)
        {
            mesurementPoint = linePt1;
            return DistanceSquared(point, linePt1);
        }
        else if (k >= 1)
        {
            mesurementPoint = linePt2;
            return DistanceSquared(point, linePt2);
        }

        mesurementPoint = D;
        return DistanceSquared(point, D);
    }

    public static Vector2 RotateVec(Vector2 toRotate, Vector2 center, FInt degrees)
    {
        FInt sin = DeterministicMath.SinD(degrees);
        FInt cos = DeterministicMath.CosD(degrees);
 
        // Translate point back to origin
        FInt x = toRotate.x - center.x;
        FInt y = toRotate.y - center.y;
 
        // Rotate point
        FInt xnew = x * cos - y * sin;
        FInt ynew = x * sin + y * cos;
     
        // Translate point back
        Vector2 newPoint = new Vector2(xnew + center.x, ynew + center.y);
        return newPoint;
    }

    public FInt Length ()
    {
        return DeterministicMath.Sqrt(x*x + y*y);
    }

    public Vector2 Normalized()
    {
        bool xZero, yZero;

        xZero = x.RawValue == 0;
        yZero = y.RawValue == 0;

        //If below assures normalize doesn't calculate an answer it already has.
        if(xZero || yZero)
        {
            var o = new FInt(1);
            if (xZero && yZero) return ZERO;
            if (xZero) return new Vector2(new FInt(), y < 0? -o:o);
            return new Vector2(x < 0? -o:o, new FInt());
        }

        FInt length = (this).Length();

        return this / length;
    }

    public Vector2 Abs()
    {
        return new Vector2(DeterministicMath.Abs(x), DeterministicMath.Abs(y));
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

    public static Vector2 operator * (Vector2 v1, int d2)
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

    public static Vector2 operator / (Vector2 v1, int d2)
    {
        return new Vector2 (v1.x / d2, v1.y / d2);
    }

    public static Vector2 operator - (Vector2 v1)
    {
        return new Vector2(-v1.x, -v1.y);
    }

    public static bool operator == (Vector2 v1, Vector2 v2)
    {
        return v1.x == v2.x && v1.y == v2.y;
    }

    public static bool operator != (Vector2 v1, Vector2 v2)
    {
        return v1.x != v2.x && v1.y != v2.y;
    }

    public static bool operator == (Vector2 v1, FInt d2)
    {
        return v1.x == d2 && v1.y == d2;
    }

    public static bool operator != (Vector2 v1, FInt d2)
    {
        return v1.x != d2 && v1.y != d2;
    }

    public static explicit operator Vector2f (Vector2 v1)
    {
        return v1.ToVectorF();
    }

    public static explicit operator Vector2 (Vector2f v1)
    {
        return new Vector2((FInt)v1.X, (FInt)v1.Y);
    }

    public override bool Equals (object o)
    {
        return (Vector2) o == this;
    }

    public override int GetHashCode()
    {
        return (int)x + (int)y;
    }

    public PointF ToPoint ()
    {
        return new PointF(x.ToFloat(), y.ToFloat());
    }

    public Vertex ToVertex ()
    {
        return new Vertex(new SFML.System.Vector2f(x.ToFloat(), y.ToFloat()));
    }

    public Vector2f ToVectorF ()
    {
        return new Vector2f(x.ToFloat(), y.ToFloat());
    }
}