using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Shape: XYBoolHolder
{
    private bool _Selected = false;
    public bool SelectedC {get => _Selected; set => _Selected = value;}

    public virtual Vector2 Position{get;set;}

    public virtual Vector2 GetRange()
    {
        throw new NotImplementedException();
    }

    public void IntersectsInfo(Shape poly, ref CollisionResult result)
    {
        Vector2
        bRange = poly.GetRange(),
        bPosition = poly.Position;

        Vector2
        aRange = GetRange(),
        aPosition = Position;

        Vector2 r = aRange + bRange;

        Vector2 d = aPosition - bPosition;

        FInt dx = d.x, dy = d.y;

        if(dx < 0) dx = -dx;
        if(dy < 0) dy = -dy;

        if(dx > r.x || dy > r.y) return;

        if(this is ConvexPolygon)
        {
            if(poly is ConvexPolygon)
            {
                ((ConvexPolygon)this).PolyIntersectsInfo((ConvexPolygon)poly, ref result);
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

        FInt dx = d.x, dy = d.y;

        if(dx < 0) dx = -dx;
        if(dy < 0) dy = -dy;

        if(dx > r.x || dy > r.y) return false;

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

    public (Shape[] arr, int count) GetIntersectionInfos (List<Shape> shapes)
    {
        Shape[] arr = new Shape[10];
        
        int arrSize = 0;

        Vector2
        aRange = GetRange(),
        aPosition = Position;

        for(int i = 0; i < shapes.Count; ++i)
        {
            Shape curr = shapes[i];

            if(curr == this) continue;

            Vector2
            bRange = curr.GetRange(),
            bPosition = curr.Position;

            Vector2 r = aRange + bRange;

            Vector2 d = aPosition - bPosition;

            FInt dx = d.x, dy = d.y;

            if(dx < 0) dx = -dx;
            if(dy < 0) dy = -dy;

            if(dx > r.x || dy > r.y) continue;
            arr[arrSize] = curr;

            ++arrSize;

            if(arr.Length - 1 == arrSize)
            {
                Array.Resize(ref arr, arr.Length + 10);
            }
            
        }

        return (arr, arrSize);
    }
}