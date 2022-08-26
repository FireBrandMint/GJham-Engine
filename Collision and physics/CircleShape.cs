using System;

public sealed class CircleShape: Shape
{
    private Vector2 _Position;

    public override Vector2 Position {
        get
        {
            return _Position;
        }
        set
        {
            _Position = value;

            MoveActive();
        }
    }

    private FInt _Area;

    public FInt Area
    {
        get
        {
            return _Area;
        }
        set
        {
            _Area = value;

            MoveActive();
        }
    }

    long[] GridIdentifier;

    public CircleShape (Vector2 position, FInt area)
    {
        _Area = area;

        _Position = position;

        CollisionRepetition = 2;
    }

    public override sealed Vector2 GetRange()
    {
        return new Vector2(Area, Area);
    }

    public override long[] GetGridIdentifier()
    {
        return GridIdentifier;
    }

    public override void SetGridIdentifier(long[] newValue)
    {
        GridIdentifier = newValue;
    }

    public bool CircleIntersects (CircleShape circle)
    {
        Vector2 pos1 = Vector2.ZERO;
        FInt area1 = Area;

        Vector2 pos2 = circle.Position - Position;
        FInt area2 = circle.Area;

        return Vector2.InRange(pos1, pos2, area1 + area2);
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

            result.Separation = normalized * (DeterministicMath.Sqrt(distanceSqr) - areaTotal);

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

    public void PolyIntersectsInfo (ConvexPolygon poly, ref CollisionResult result)
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

        FInt lowestDistanceSqr = FInt.MaxValue;

        Vector2 lineColPoint = new Vector2();

        for(int i1 = 0; i1 < vertsAmount; ++i1)
        {
            int i2 = (i1 + 1) % vertsAmount;

            Vector2 colPoint;

            FInt distSquared = Vector2.LinePointDistSqr(verts[i1], verts[i2], circlePos, out colPoint);

            if(distSquared < lowestDistanceSqr)
            {
                lineColPoint = colPoint;
                lowestDistanceSqr = distSquared;
            }
        }

        bool IsInside = PointInConvexPolygon(circlePos, verts);

        if(lowestDistanceSqr > circleAreaSquared && !IsInside)
        {
            result.Intersects = false;
            return;
        }

        if(IsInside)
        {
            //The direction from the circle to the line.
            var direction = lineColPoint - circlePos;

            result.Separation = direction.Normalized() * (circleArea + DeterministicMath.Sqrt(lowestDistanceSqr));
        }
        else
        {
            //The direction from the line to the circle middle.
            var direction =  circlePos - lineColPoint;

            result.Separation = direction.Normalized() * (circleArea - DeterministicMath.Sqrt(lowestDistanceSqr));
        }

        result.Intersects = true;
    }

    bool Disposed = false;

    public override void Dispose()
    {
        if(Disposed) return;
        Disposed = true;

        Deactivate();
    }
}