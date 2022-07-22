
public class ConvexPolygon
{
    bool Updated = false;

    Vector2[] OriginalModel;

    Vector2 _pos;

    public Vector2 Position {get{return _pos;} set{Updated = true; _pos = value;}}

    FInt _rot;

    ///<summary>
    ///Rotation in degrees.
    ///</summary>
    public FInt Rotation {get{return _rot;} set {Updated = true; _rot = value;}}

    Vector2[] ResultModel;

    public FInt RangeX;

    public FInt RangeY;

    public ConvexPolygon(Vector2[] model, Vector2 position, FInt rotation)
    {
        OriginalModel = model;

        Position = position;

        Rotation = rotation;

        ResultModel = new Vector2[model.Length];
    }

    void UpdateModel()
    {
        if (!Updated) return;

        //solves rotation
        Vector2 center = Vector2.ZERO;

        for(int i = 0; i< OriginalModel.Length; ++i)
        {
            Vector2 curr = OriginalModel[i];

            ResultModel[i] = Vector2.RotateVec(curr, center, _rot);
        }

        //solves range
        UpdateRange();

        //solves position
        for(int i = 0; i< ResultModel.Length; ++i)
        {
            ResultModel[i] = ResultModel[i] + Position;
        }

        Updated = false;
    }

    private void UpdateRange()
    {
        FInt rangX = FInt.MinValue;
        FInt rangY = FInt.MinValue;

        for(int i = 0; i < ResultModel.Length; ++i)
        {
            Vector2 curr = ResultModel[i];

            FInt currX = DeterministicMath.Abs(curr.x);
            FInt currY = DeterministicMath.Abs(curr.x);

            if(rangX < currX) rangX = currX;

            if(rangY < currY) rangY = currY;
        }

        RangeX = rangX;
        RangeY = rangY;
    }

    public Vector2[] GetModel()
    {
        UpdateModel();

        return ResultModel;
    }

    public bool Intersects(ConvexPolygon poly)
    {
        Vector2[] a = GetModel();
        Vector2[] b = poly.GetModel();

        Vector2 RangeAMin = new Vector2(Position.x - RangeX, Position.y - RangeY);
        Vector2 RangeAMax = new Vector2(Position.x + RangeX, Position.y + RangeY);

        Vector2 bPosition = poly.Position;

        FInt bRangeX = poly.RangeX;
        FInt bRangeY = poly.RangeY;

        Vector2 RangeBMin = new Vector2(bPosition.x - bRangeX, bPosition.y - bRangeY);
        Vector2 RangeBMax = new Vector2(bPosition.x + bRangeX, bPosition.y + bRangeY);

        FInt d1x = RangeBMin.x - RangeAMax.x;
        FInt d1y = RangeBMin.y - RangeAMax.y;
        FInt d2x = RangeAMin.x - RangeBMax.x;
        FInt d2y = RangeAMin.y - RangeBMax.y;

        FInt zero = new FInt();

        if (d1x > zero || d1y > zero) goto end;

        if (d2x > zero || d2y > zero) goto end;

        for(int polyi = 0; polyi < 2; ++polyi)
        {
            Vector2[] polygon = polyi == 0 ? a : b;

            for(int i1 = 0; i1 < polygon.Length; ++i1)
            {
                int i2 = (i1 + 1) % polygon.Length;

                Vector2 normal = polygon[i2] - polygon[i1];
                
                FInt minA = FInt.MaxValue;
                FInt maxA = FInt.MinValue;


                //Projects verts for poly 'a' for min max.
                for(int ai = 0; ai < a.Length; ++ai)
                {
                    FInt projected = Vector2.DotProduct(normal, a[ai]);

                    if( projected < minA ) minA = projected;
                    if( projected > maxA ) maxA = projected;
                }

                //Projects verts for poly 'b' for min max.
                FInt minB = FInt.MaxValue;
                FInt maxB = FInt.MinValue;
                for(int bi = 0; bi < b.Length; ++bi)
                {
                    FInt projected = Vector2.DotProduct(normal, b[bi]);

                    if( projected < minB ) minB = projected;
                    if( projected > maxB ) maxB = projected;
                }

                if(maxA < minB || maxB < minA) goto end;
            }
        }

        return true;

        end:
        return false;
    }

    public void IntersectsInfo(ConvexPolygon poly, CollisionResult result)
    {
        result.Intersects = true;

        Vector2[] a = GetModel();
        Vector2[] b = poly.GetModel();

        Vector2 RangeAMin = new Vector2(Position.x - RangeX, Position.y - RangeY);
        Vector2 RangeAMax = new Vector2(Position.x + RangeX, Position.y + RangeY);

        Vector2 bPosition = poly.Position;

        FInt bRangeX = poly.RangeX;
        FInt bRangeY = poly.RangeY;

        Vector2 RangeBMin = new Vector2(bPosition.x - bRangeX, bPosition.y - bRangeY);
        Vector2 RangeBMax = new Vector2(bPosition.x + bRangeX, bPosition.y + bRangeY);

        FInt d1x = RangeBMin.x - RangeAMax.x;
        FInt d1y = RangeBMin.y - RangeAMax.y;
        FInt d2x = RangeAMin.x - RangeBMax.x;
        FInt d2y = RangeAMin.y - RangeBMax.y;

        FInt zero = new FInt();

        if (d1x > zero || d1y > zero) goto doesntIntersect;

        if (d2x > zero || d2y > zero) goto doesntIntersect;

        FInt distance = FInt.MaxValue;

        FInt shortestDist = FInt.MaxValue;

        Vector2 vector = new Vector2();

        for(int polyi = 0; polyi < 2; ++polyi)
        {
            Vector2[] polygon = polyi == 0 ? a : b;

            for(int i1 = 0; i1 < polygon.Length; ++i1)
            {
                int i2 = (i1 + 1) % polygon.Length;

                FInt normalx = -(polygon[i2].y - polygon[i1].y);
                
                FInt normaly = polygon[i2].x - polygon[i1].x;

                Vector2 normal = new Vector2(normalx, normaly).Normalized();
                
                FInt minA = FInt.MaxValue;
                FInt maxA = FInt.MinValue;


                //Projects verts for poly 'a' for min max.
                for(int ai = 0; ai < a.Length; ++ai)
                {
                    FInt projected = Vector2.DotProduct(normal, a[ai]);

                    if( projected < minA ) minA = projected;
                    if( projected > maxA ) maxA = projected;
                }

                //Projects verts for poly 'b' for min max.
                FInt minB = FInt.MaxValue;
                FInt maxB = FInt.MinValue;
                for(int bi = 0; bi < b.Length; ++bi)
                {
                    FInt projected = Vector2.DotProduct(normal, b[bi]);

                    if( projected < minB ) minB = projected;
                    if( projected > maxB ) maxB = projected;
                }

                if(maxA < minB || maxB < minA) goto doesntIntersect;

                //FInt distMin = DeterministicMath.Min(maxA, maxB) - DeterministicMath.Max(minA, minB);
                FInt distMin = maxB - minA;
                distMin *= -1 + polyi * 2;

                FInt distMinAbs = DeterministicMath.Abs(distMin);

                if (distMinAbs < shortestDist)
                {
                    shortestDist = distMinAbs;
                    distance = distMinAbs;

                    vector = normal;
                }
            }
        }

        result.Separation = vector * distance * -1;

        return;

        doesntIntersect:
        result.Intersects = false;
    }

    public void IntersectsInfoFast(ConvexPolygon poly, CollisionResult result)
    {
        result.Intersects = true;

        Vector2[] a = GetModel();
        Vector2[] b = poly.GetModel();

        Vector2 RangeAMin = new Vector2(Position.x - RangeX, Position.y - RangeY);
        Vector2 RangeAMax = new Vector2(Position.x + RangeX, Position.y + RangeY);

        Vector2 bPosition = poly.Position;

        FInt bRangeX = poly.RangeX;
        FInt bRangeY = poly.RangeY;

        Vector2 RangeBMin = new Vector2(bPosition.x - bRangeX, bPosition.y - bRangeY);
        Vector2 RangeBMax = new Vector2(bPosition.x + bRangeX, bPosition.y + bRangeY);

        FInt d1x = RangeBMin.x - RangeAMax.x;
        FInt d1y = RangeBMin.y - RangeAMax.y;
        FInt d2x = RangeAMin.x - RangeBMax.x;
        FInt d2y = RangeAMin.y - RangeBMax.y;

        FInt zero = new FInt();

        if (d1x > zero || d1y > zero) goto doesntIntersect;

        if (d2x > zero || d2y > zero) goto doesntIntersect;

        FInt distance = FInt.MaxValue;

        FInt shortestDist = FInt.MaxValue;

        Vector2 vector = new Vector2();

        for(int polyi = 0; polyi < 2; ++polyi)
        {
            Vector2[] polygon = polyi == 0 ? a : b;

            for(int i1 = 0; i1 < polygon.Length; ++i1)
            {
                int i2 = (i1 + 1) % polygon.Length;

                FInt normalx = -(polygon[i2].y - polygon[i1].y);
                
                FInt normaly = polygon[i2].x - polygon[i1].x;

                Vector2 normal = new Vector2(normalx, normaly).FastNormalized();
                
                FInt minA = FInt.MaxValue;
                FInt maxA = FInt.MinValue;


                //Projects verts for poly 'a' for min max.
                for(int ai = 0; ai < a.Length; ++ai)
                {
                    FInt projected = Vector2.DotProduct(normal, a[ai]);

                    if( projected < minA ) minA = projected;
                    if( projected > maxA ) maxA = projected;
                }

                //Projects verts for poly 'b' for min max.
                FInt minB = FInt.MaxValue;
                FInt maxB = FInt.MinValue;
                for(int bi = 0; bi < b.Length; ++bi)
                {
                    FInt projected = Vector2.DotProduct(normal, b[bi]);

                    if( projected < minB ) minB = projected;
                    if( projected > maxB ) maxB = projected;
                }

                if(maxA < minB || maxB < minA) goto doesntIntersect;

                //FInt distMin = DeterministicMath.Min(maxA, maxB) - DeterministicMath.Max(minA, minB);
                FInt distMin = maxB - minA;
                distMin *= -1 + polyi * 2;

                FInt distMinAbs = DeterministicMath.Abs(distMin);

                if (distMinAbs < shortestDist)
                {
                    shortestDist = distMinAbs;
                    distance = distMinAbs;

                    vector = normal;
                }
            }
        }

        result.Separation = vector * distance * -1;

        return;

        doesntIntersect:
        result.Intersects = false;
    }
}