using System;

public sealed class CircleShape: Shape
{
    public FInt Area;

    public CircleShape (Vector2 position, FInt area)
    {
        Area = area;

        Position = position;
    }

    public override sealed Vector2 GetRange()
    {
        return new Vector2(Area, Area);
    }

    public bool CircleIntersects (CircleShape circle)
    {
        Vector2 pos1 = Vector2.ZERO;
        FInt area1 = Area;

        Vector2 pos2 = circle.Position - Position;
        FInt area2 = circle.Area;

        return Vector2.InRangeSquared(pos1, pos2, area1 + area2);
    }

    public void CircleIntersectsInfo(CircleShape circle, ref CollisionResult result)
    {
        Vector2 pos1 = Vector2.ZERO;
        FInt area1 = Area;

        Vector2 pos2 = circle.Position - Position;
        FInt area2 = circle.Area;

        FInt areaTotal = area1 + area2;

        FInt distanceSqr = Vector2.DistanceSquared(pos1, pos2);



        if(distanceSqr <= areaTotal * areaTotal)
        {
            var normalized = pos2.Normalized();

            result.Intersects =  true;

            result.Separation = -normalized * (DeterministicMath.Sqrt(distanceSqr) - areaTotal);

            return;
        }
        
        result.Intersects = false;
    }

    public bool PolyIntersects(ConvexPolygon poly)
    {
        //The only way a circle is intersecting a polygon is
        //if it colides with any of the poly's lines
        //OR if the circle itself is inside the polygon

        Vector2 polyPos = poly.Position;

        Vector2[] vertsRaw = poly.GetModel();

        int vertsAmount = vertsRaw.Length;

        Vector2[] verts = new Vector2[vertsAmount];

        for(int i = 0; i<vertsAmount; ++i)
        {
            verts[i] = vertsRaw[i] - polyPos;
        }

        Vector2 circlePos = Position - polyPos;

        FInt circleArea = Area;

        FInt circleAreaSquared = circleArea * circleArea;

        bool result = false;

        for(int i1 = 0; i1 < vertsAmount; ++i1)
        {
            int i2 = (i1 + 1) % vertsAmount;

            FInt distSquared = Vector2.LinePointDistSqr(verts[i1], verts[i2], circlePos);

            result = result || distSquared <= circleAreaSquared;
        }

        //If circle is not touching one of the shape's lines,
        //then the only way they intersect is if the circle is inside.
        if(result!)
        {
            return Shape.PointInConvexPolygon(circlePos, verts);
        }

        return result;
    }

    
}