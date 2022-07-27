using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Shape
{
    ///<summary>
    ///Shape type: 0 is convex polygon.
    ///</summary>
    public virtual int ShapeType() => -4;

    public void IntersectsInfo(Shape poly, CollisionResult result)
    {
        if(this.ShapeType() == 0)
        {
            if(poly.ShapeType() == 0)
            {
                ((ConvexPolygon)this).PolyIntersectsInfo((ConvexPolygon)poly, result);
                return;
            }
        }
        throw new System.Exception($"Shape not implemented! Shape ids: {this.ShapeType()}, {poly.ShapeType()}.");
    }

    public bool Intersect(Shape poly)
    {
        if(this.ShapeType() == 0)
        {
            if(poly.ShapeType() == 0)
            {
                return ((ConvexPolygon)this).PolyIntersects((ConvexPolygon) poly);
            }
        }

        throw new System.Exception($"Shape not implemented! Shape id: {this.ShapeType()}, {poly.ShapeType()}.");
    }
}