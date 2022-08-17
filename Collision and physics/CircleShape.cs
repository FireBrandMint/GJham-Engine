
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
}