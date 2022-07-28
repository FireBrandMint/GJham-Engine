using System;
using System.Threading.Tasks;
using System.Threading;

public sealed class ConvexPolygon : Shape
{
    bool Updated = false;

    //Action ModelAction;

    bool NormalsUpdated = false;

    //Action NormalsAction;

    Vector2[] OriginalModel;

    Vector2 _pos;

    public override Vector2 Position
    {
        get{return _pos;} 
        set
        {
            Updated = Updated && value == _pos;

            //if(!Updated) ModelAction = UpdateModel;

            _pos = value;

            /*if(value != _pos)
            {
                _pos = value;
                UpdateModel();
            }*/
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

            //if(!Updated) ModelAction = UpdateModel;
            //if(!NormalsUpdated) NormalsAction = UpdateNormals;

            _rot = value;

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

    public static ConvexPolygon CreateRect(Vector2 length, FInt rotation, Vector2 position)
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
            rotation
            );
    }

    public static ConvexPolygon CreateTriangle(Vector2 length, FInt rotation, Vector2 position)
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
            rotation
            );
    }

    public ConvexPolygon(Vector2[] model, Vector2 position, FInt rotation)
    {
        OriginalModel = model;

        _pos = position;

        _rot = rotation;

        ResultModel = new Vector2[model.Length];

        Normals = new Vector2[model.Length];

        //ModelAction = UpdateModel;

        //NormalsAction = UpdateNormals;

        //UpdateModel();
        //UpdateNormals();
    }

    public ConvexPolygon(Vector2[] model, Vector2 position, FInt rotation, bool calculateNormals)
    {
        OriginalModel = model;

        Position = position;

        Rotation = rotation;

        ResultModel = new Vector2[model.Length];

        Normals = new Vector2[model.Length];

        if(calculateNormals)
        {
            UpdateModel();
            UpdateNormals();
        }
    }

    void UpdateModel()
    {
        if (Updated) return;

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

        Updated = true;

        //ModelAction = DoNothing;
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

        Range = new Vector2(rangX, rangY);
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
        UpdateModel();

        return ResultModel;
    }

    public Vector2[] GetNormals()
    {
        UpdateNormals();

        return Normals;
    }

    public override Vector2 GetRange()
    {
        UpdateModel();

        return Range;
    }

    public bool PolyIntersects(ConvexPolygon poly)
    {
        Vector2[] a = GetModel();
        Vector2[] b = poly.GetModel();

        Vector2 bPosition = poly.Position;

        Vector2
        aRange = Range,
        bRange = poly.Range;

        Vector2 r = aRange + bRange;

        Vector2 d = Position - bPosition;
        d = d.Abs();

        if(d.x > r.x || d.y > r.y) goto end;

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

    public void PolyIntersectsInfoSlow(ConvexPolygon poly, CollisionResult result)
    {
        result.Intersects = true;

        Vector2[] a = GetModel();
        Vector2[] b = poly.GetModel();

        Vector2 bPosition = poly.Position;

        Vector2
        aRange = Range,
        bRange = poly.Range;

        Vector2 r = aRange + bRange;

        Vector2 d = Position - bPosition;
        d = d.Abs();

        if(d.x > r.x || d.y > r.y) goto doesntIntersect;

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

    public void PolyIntersectsInfo(ConvexPolygon poly, CollisionResult result)
    {
        result.Intersects = false;

        var mA = GetModel();
        var mB = poly.GetModel();

        Vector2
        bRange = poly.Range,
        bPosition = poly.Position;

        Vector2
        aRange = Range,
        aPosition = Position;

        Vector2 r = aRange + bRange;

        Vector2 d = aPosition - bPosition;
        d = new Vector2(DeterministicMath.Abs(d.x), DeterministicMath.Abs(d.y));

        if(d.x > r.x || d.y > r.y)
        {
            //goto doesntIntersect;

            return;
        }

        int aLength = mA.Length;
        int bLength = mB.Length;

        Vector2[] a = new Vector2[aLength];
        Vector2[] b = new Vector2[bLength];

        Array.Copy(mA, a, aLength);
        Array.Copy(mB, b, bLength);

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

        result.Separation = vector * distance;

        result.Intersects = true;
    }

    //This void does nothing.
    private void DoNothing ()
    {

    }
}