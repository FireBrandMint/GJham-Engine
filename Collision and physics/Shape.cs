using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Shape
{
    ///<summary>
    ///Shape type: 0 is convex polygon.
    ///</summary>

    public virtual Vector2 Position{get;set;}

    public virtual Vector2 GetRange()
    {
        throw new NotImplementedException();
    }

    public void IntersectsInfo(Shape poly, CollisionResult result)
    {
        Vector2
        bRange = poly.GetRange(),
        bPosition = poly.Position;

        Vector2
        aRange = GetRange(),
        aPosition = Position;

        Vector2 r = aRange + bRange;

        Vector2 d = aPosition - bPosition;
        d = new Vector2(DeterministicMath.Abs(d.x), DeterministicMath.Abs(d.y));

        if(d.x > r.x || d.y > r.y) return;

        if(this is ConvexPolygon)
        {
            if(poly is ConvexPolygon)
            {
                ((ConvexPolygon)this).PolyIntersectsInfo((ConvexPolygon)poly, result);
                return;
            }
        }
        throw new System.Exception($"Shape not implemented! Shape ids: {this.GetType()}, {poly.GetType()}.");
    }

    public bool Intersect(Shape poly)
    {
        Vector2
        bRange = poly.GetRange(),
        bPosition = poly.Position;

        Vector2
        aRange = GetRange(),
        aPosition = Position;

        Vector2 r = aRange + bRange;

        Vector2 d = aPosition - bPosition;
        d = new Vector2(DeterministicMath.Abs(d.x), DeterministicMath.Abs(d.y));

        if(d.x > r.x || d.y > r.y) return false;

        switch(this)
        {
            case ConvexPolygon:
            switch(poly)
            {
                case ConvexPolygon: return ((ConvexPolygon)this).PolyIntersects((ConvexPolygon) poly);
            }
                throw new System.Exception($"Shape not implemented! Shape id: {this.GetType()}, {poly.GetType()}.");
        }

        throw new System.Exception($"Shape not implemented! Shape id: {this.GetType()}, {poly.GetType()}.");
    }
}