
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public interface CollisionShape
{
    Vector2 Position{get;set;}
    void IntersectsInfo(CollisionShape poly, CollisionResult result);
    bool Intersects(CollisionShape poly);
}