using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

///<summary>
///Simple AABB rectangle only used
///for culling in rendering at the time of writing!
///</summary>
public struct AABB
{
    public Vector2 Origin;

    public Vector2 Ranges;

    ///<summary>
    ///Ranges is always positive;
    ///</summary>
    public AABB(Vector2 origin, Vector2 ranges)
    {
        Origin = origin;
        Ranges = ranges;
    }

    public bool Intersects(ref AABB other)
    {
        var minA = Origin;

        var minB = other.Origin;
        var maxB = other.Origin + other.Ranges;

        return
        minA.x < maxB.x &&
        minA.x + Ranges.x > minB.x &&
        minA.y < maxB.y &&
        minA.y + Ranges.y > minB.y;
    }
}