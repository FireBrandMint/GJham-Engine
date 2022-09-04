using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class CircleCollider : ShapeColliderBase
{
    FInt _Range = (FInt)0;
    public FInt Range
    {
        get => _Range;
        set
        {
            if(_Range != value && CollisionFodder != null) ((CircleShape)CollisionFodder).Area = value;

            _Range = value;
        }
    }

    protected override Shape CreateShape()
    {
        return new CircleShape(Position, _Range);
    }
}