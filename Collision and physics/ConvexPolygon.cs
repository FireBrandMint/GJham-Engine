public sealed class ConvexPolygon : Shape
{
    bool Updated = false;

    bool NormalsUpdated = false;

    bool RangeUpdated = false;

    long[] GridIdentifier;

    Vector2[] OriginalModel;

    Vector2 _pos;

    public override sealed Vector2 Position
    {
        get{return _pos;} 
        set
        {
            Updated = Updated && value == _pos;

            //if(!Updated) ModelAction = UpdateModel;

            _pos = value;

            UpdateModel();

            /*if(value != _pos)
            {
                _pos = value;
                UpdateModel();
            }*/
        }
    }

    Vector2 _scale = new Vector2(1,1);

    public Vector2 Scale
    {
        get => _scale;
        set
        {
            Updated = Updated && value == _scale;

            _scale = value;

            UpdateModel();
        }
    }

    FInt _rot;

    ///<summary>
    ///Rotation in degrees.
    ///</summary>
    public FInt Rotation
    {
        get{return _rot;} 
        set 
        {
            Updated = Updated && value == _rot;
            NormalsUpdated = NormalsUpdated && value == _rot;
            RangeUpdated = RangeUpdated && value == _rot;

            //if(!Updated) ModelAction = UpdateModel;
            //if(!NormalsUpdated) NormalsAction = UpdateNormals;

            _rot = value;

            UpdateModel();
            UpdateNormals();

            /*if(value != _rot)
            {
                _rot = value;

                UpdateModel();
                UpdateNormals();
            }*/
        }
    }

    Vector2[] ResultModel;

    Vector2[] Normals;

    public Vector2 Range;

    public static ConvexPolygon CreateRect(Vector2 length, Vector2 scale, FInt rotation, Vector2 position)
    {
        FInt x = length.x * FInt.Half;
        FInt y = length.y * FInt.Half;

        return new ConvexPolygon(
            new Vector2[]
            {
                //top left
                new Vector2(-x, -y),
                //bottom left
                new Vector2(-x, y),
                //bottom right
                new Vector2(x, y),
                //top right
                new Vector2(x, -y),
            },
            position,
            scale,
            rotation
            );
    }

    public static ConvexPolygon CreateTriangle(Vector2 length, Vector2 scale, FInt rotation, Vector2 position)
    {
        FInt x = length.x * FInt.Half;
        FInt y = length.y * FInt.Half;

        return new ConvexPolygon(
            new Vector2[]
            {
                //top
                new Vector2(new FInt(), -y),
                //bottom left
                new Vector2(-x, y),
                //bottom right
                new Vector2(x, y)
            },
            position,
            scale,
            rotation
            );
    }

    public ConvexPolygon(Vector2[] model, Vector2 position, Vector2 scale, FInt rotation)
    {
        OriginalModel = model;

        _pos = position;

        _rot = rotation;

        _scale = scale;

        ResultModel = new Vector2[model.Length];

        Normals = new Vector2[model.Length];

        //ModelAction = UpdateModel;

        //NormalsAction = UpdateNormals;

        UpdateModel();

        UpdateNormals();
    }

    void UpdateModel()
    {
        if (Updated) return;

        //solves rotation and scale
        Vector2 center = Vector2.ZERO;

        for(int i = 0; i< OriginalModel.Length; ++i)
        {
            Vector2 curr = OriginalModel[i] * _scale;

            ResultModel[i] = Vector2.RotateVec(curr, center, _rot);
        }

        //solves range
        UpdateRange();

        //solves position
        for(int i = 0; i< ResultModel.Length; ++i)
        {
            ResultModel[i] = ResultModel[i] + Position;
        }

        MoveActive();

        Updated = true;

        //ModelAction = DoNothing;
    }

    private void UpdateRange()
    {
        if(RangeUpdated) return;

        FInt rangX = FInt.MinValue;
        FInt rangY = FInt.MinValue;

        for(int i = 0; i < ResultModel.Length; ++i)
        {
            Vector2 curr = ResultModel[i];

            FInt currX = DeterministicMath.Abs(curr.x);
            FInt currY = DeterministicMath.Abs(curr.y);

            if(rangX < currX) rangX = currX;

            if(rangY < currY) rangY = currY;
        }

        Range = new Vector2(rangX, rangY);

        RangeUpdated = true;
    }

    private void UpdateNormals()
    {
        if(NormalsUpdated) return;

        int len = ResultModel.Length - 1;

        Vector2 p1, p2;
        FInt normalx, normaly;
        
        for(int i = 0; i< len; ++i)
        {
            p1 = ResultModel[i];
            p2 = ResultModel[i+1];

            normalx = -(p2.y - p1.y);
                
            normaly = p2.x - p1.x;

            Normals[i] = new Vector2(normalx, normaly).Normalized();
        }

        p1 = ResultModel[len];
        p2 = ResultModel[0];

        normalx = -(p2.y - p1.y);
                
        normaly = p2.x - p1.x;

        Normals[len] = new Vector2(normalx, normaly).Normalized();

        NormalsUpdated = true;

        //NormalsAction = DoNothing;
    }

    public Vector2[] GetModel()
    {
        return ResultModel;
    }

    public Vector2[] GetNormals()
    {
        return Normals;
    }

    public override sealed Vector2 GetRange()
    {
        return Range;
    }

    public override long[] GetGridIdentifier()
    {
        return GridIdentifier;
    }

    public override void SetGridIdentifier(long[] newValue)
    {
        GridIdentifier = newValue;
    }
    
    public bool PolyIntersects(ConvexPolygon poly)
    {
        #region Bring vertices close to Vector2.ZERO to prevent overflow.

        Vector2 aPosition = Position;

        Vector2[] mA = GetModel();
        Vector2[] mB = poly.GetModel();

        int aLength = mA.Length;
        int bLength = mB.Length;

        Vector2[] a = new Vector2[aLength];
        Vector2[] b = new Vector2[bLength];

        for(int i = 0; i< aLength; ++i)
        {
            a[i] = mA[i] - aPosition;
        }

        for(int i = 0; i< bLength; ++i)
        {
            b[i] = mB[i] - aPosition;
        }

        #endregion

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

    public void PolyIntersectsInfoSlow(ConvexPolygon poly, ref CollisionResult result)
    {
        result.Intersects = true;

        Vector2[] a = GetModel();
        Vector2[] b = poly.GetModel();

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
    public void PolyIntersectsInfo(ConvexPolygon poly, ref CollisionResult result)
    {
        result.Intersects = false;

        var mA = GetModel();
        var mB = poly.GetModel();

        Vector2 aPosition = Position;

        int aLength = mA.Length;
        int bLength = mB.Length;

        #region Bring vertices close to Vector2.ZERO to prevent overflow. 

        #endregion

        Vector2[] a = new Vector2[aLength];
        Vector2[] b = new Vector2[bLength];

        for(int i = 0; i< aLength; ++i)
        {
            a[i] = mA[i] - aPosition;
        }

        for(int i = 0; i< bLength; ++i)
        {
            b[i] = mB[i] - aPosition;
        }

        FInt distance = FInt.MaxValue;

        Vector2 vector = new Vector2();

        FInt minA, maxA, minB, maxB;
        
        Vector2 normal;

        for(int polyi = 0; polyi < 2; ++polyi)
        {
            Vector2[] polygon;
            Vector2[] normals;

            if(polyi == 0)
            {
                polygon = a;
                normals = this.GetNormals();
            }
            else
            {
                polygon = b;
                normals = poly.GetNormals();
            }

            for(int i1 = 0; i1 < polygon.Length; ++i1)
            {
                //int i2 = (i1 + 1) % polygon.Length;

                normal = normals[i1];
                
                minA = FInt.MaxValue;
                maxA = FInt.MinValue;

                
                //Projects verts for poly 'a' for min max.
                for(int ai = 0; ai < aLength; ++ai)
                {
                    FInt projected = Vector2.DotProduct(normal, a[ai]);

                    if( projected < minA ) minA = projected;
                    if( projected > maxA ) maxA = projected;
                }

                //Projects verts for poly 'b' for min max.
                minB = FInt.MaxValue;
                maxB = FInt.MinValue;
                for(int bi = 0; bi < bLength; ++bi)
                {
                    FInt projected = Vector2.DotProduct(normal, b[bi]);

                    if( projected < minB ) minB = projected;
                    if( projected > maxB ) maxB = projected;
                }

                if(maxA < minB || maxB < minA) return;

                //FInt distMin = DeterministicMath.Min(maxA, maxB) - DeterministicMath.Max(minA, minB);
                FInt distMin = maxB - minA;

                FInt distMinAbs = DeterministicMath.Abs(distMin);

                if (distMinAbs < distance)
                {
                    distance = distMinAbs;

                    vector = normal;
                }
            }
        }

        FInt factor;

        factor.RawValue = 4140L;

        result.Separation = vector * distance * factor;

        result.SeparationDirection = vector;

        result.Intersects = true;
    }

    public bool CircleIntersects(CircleShape circle)
    {
        //The only way a circle is intersecting a polygon is
        //if it colides with any of the poly's lines
        //OR if the circle itself is inside the polygon

        Vector2 polyPos = Position;

        Vector2[] vertsRaw = GetModel();

        int vertsAmount = vertsRaw.Length;

        Vector2[] verts = new Vector2[vertsAmount];

        for(int i = 0; i<vertsAmount; ++i)
        {
            verts[i] = vertsRaw[i] - polyPos;
        }

        Vector2 circlePos = circle.Position - polyPos;

        FInt circleArea = circle.Area;

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

    public void CircleIntersectsInfo(CircleShape circle, ref CollisionResult result)
    {
        //The only way a circle is intersecting a polygon is
        //if it colides with any of the poly's lines
        //OR if the circle itself is inside the polygon

        Vector2 polyPos = Position;

        Vector2[] vertsRaw = GetModel();

        int vertsAmount = vertsRaw.Length;

        Vector2[] verts = new Vector2[vertsAmount];

        for(int i = 0; i<vertsAmount; ++i)
        {
            verts[i] = vertsRaw[i] - polyPos;
        }

        Vector2 circlePos = circle.Position - polyPos;

        FInt circleArea = circle.Area;

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
            //The direction from the line to the circle middle.
            var direction = circlePos - lineColPoint;

            var dir = direction.Normalized();

            result.Separation = dir * (circleArea + DeterministicMath.Sqrt(lowestDistanceSqr));

            result.SeparationDirection = dir;
        }
        else
        {

            //The direction from the circle middle to the line.
            var direction = lineColPoint - circlePos;

            var dir = direction.Normalized();

            result.Separation = dir * (circleArea - DeterministicMath.Sqrt(lowestDistanceSqr));

            result.SeparationDirection = dir;
        }

        

        result.Intersects = true;
    }
}