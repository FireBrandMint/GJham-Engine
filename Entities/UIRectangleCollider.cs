using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class UIRectangleCollider: UICollider
{
    Vector2 _Proportions = new Vector2(1,1);
    public Vector2 Proportions
    {
        get => _Proportions;
        set
        {
            if(CollisionFodder != null) throw new Exception("Can't change rectangle Proportions in real time, try Scale instead.");

            _Proportions = value;
        }
    }

    Vector2 _Scale = new Vector2(1,1);

    public Vector2 Scale
    {
        get => _Scale;
        set
        {
            if(CollisionFodder != null) ((UIConvexPolygon) CollisionFodder).Scale = value;

            _Scale = value;
        }
    }

    UIAdjustmentMode _AdjustmentMode = UIAdjustmentMode.Compact;

    public UIAdjustmentMode AdjustmentMode
    {
        get => _AdjustmentMode;
        set
        {
            if(CollisionFodder != null) CollisionFodder.Mode = value;
            
            _AdjustmentMode = value;
        }
    }

    protected override UIShape CreateShape()
    {
        var poly = UIConvexPolygon.CreateRect(Position, _Proportions, _AdjustmentMode);

        poly.Scale = Scale;

        return poly;
    }
}
