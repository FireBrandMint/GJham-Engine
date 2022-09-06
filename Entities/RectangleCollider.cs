using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RectangleCollider : ShapeColliderBase
{
    public Vector2 _Proportions = new Vector2(50, 50);
    public Vector2 Proportions
    {
        get => _Proportions;
        set
        {
            if(CollisionFodder != null) throw new Exception("Can't change rectangle Proportions in real time, try Scale instead.");

            _Proportions = value;
        }
    }

    public Vector2 _Scale = new Vector2(50, 50);
    public Vector2 Scale
    {
        get => _Scale;
        set
        {
            if(CollisionFodder != null) ((ConvexPolygon)CollisionFodder).Scale = value;

            _Scale = value;
        }
    }

    FInt _Rotation = new FInt(0);
    FInt Rotation
    {
        get => _Rotation;
        set
        {
            if(CollisionFodder != null) ((ConvexPolygon)CollisionFodder).Rotation = value;

            _Rotation = value;

        }
    }

    

    protected override Shape CreateShape()
    {
        return ConvexPolygon.CreateRect(_Proportions, _Scale, Rotation, Position);
    }
}