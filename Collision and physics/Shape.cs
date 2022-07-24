using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public interface Shape
{
    ///<summary>
    ///Shape type: 0 is convex polygon.
    ///</summary>
    int ShapeType{get;}

    void IntersectsInfo(Shape poly, CollisionResult result);

    bool Intersect(Shape poly);
}