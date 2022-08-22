using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GJham.Physics.Util;
using System.Runtime.CompilerServices;

/// <summary>
/// Shape class, collision detection with GetShapesInGrid
/// can be deterministic as long as the shapes keep their IDS.
/// </summary>
public class Shape: XYBoolHolder
{
    static int IDNEXT = 0;

    public bool IDSet = false;
    
    int _ID;

    public int ID 
    {
        get => _ID;

        set 
        {

            if(IDSet && Active)
            {
                if(_ID == value) return;
                _ID = value;
                Deactivate();
                Activate();
                return;
            }

            _ID = value;

            IDSet = true;
        }
    }

    private static XYList<Shape> ShapeGrid = new XYList<Shape>(64, 1000, 10);

    #region static grid

    public bool Active = false;

    /// <summary>
    /// The object that is using this shape for collision.
    /// </summary>
    public CollisionAntenna ObjectUsingIt = null;

    public static long [] GridAddShape (Shape s)
    {
        Vector2 pos = s.Position;

        Vector2 range = s.GetRange();

        Vector2 topLeft = pos - range;

        Vector2 bottomRight = pos + range;

        return ShapeGrid.AddNode(s, topLeft, bottomRight);
    }

    public static long[] GridMoveShape (Shape s)
    {
        long[] pastIdentifier = s.GetGridIdentifier();

        Vector2 pos = s.Position;

        Vector2 range = s.GetRange();

        Vector2 topLeft = pos - range;

        Vector2 bottomRight = pos + range;

        long[] currIdentifier = ShapeGrid.GetRanges(topLeft, bottomRight);

        bool different = false;

        for(int i = 0; i<4; ++i)
        {
            different = different || pastIdentifier[i] != currIdentifier[i];
        }

        if(different)
        {
            ShapeGrid.RemoveValue(pastIdentifier, s);

            return ShapeGrid.AddNode(s, currIdentifier);
        }

        return pastIdentifier;
    }

    public static void GridRemoveShape (Shape s)
    {
        ShapeGrid.RemoveValue(s.GetGridIdentifier(), s);
    }

    public static Shape[] GetShapesInGrid (long[] identifier)
    {
        return ShapeGrid.GetValues(identifier);
    }

    public static Shape[] GetShapesInGrid (Shape s)
    {
        return ShapeGrid.GetValues(s.GetGridIdentifier());
    }

    #endregion

    private bool _Selected = false;
    public bool SelectedC {get => _Selected; set => _Selected = value;}

    public virtual Vector2 Position{get;set;}

    ///<summary>
    ///Range is a vector
    ///X is the highest absolute x coord of the points list in position Vector.ZERO
    ///Y is the highest absolute y coord of the points list in position Vector.ZERO
    ///</summary>
    public virtual Vector2 GetRange() => throw new NotImplementedException();

    public virtual long[] GetGridIdentifier() => throw new NotImplementedException();

    public virtual void SetGridIdentifier(long[] newValue) => throw new NotImplementedException();

    public void Activate()
    {
        if(!Active)
        {
            if(!IDSet)
            {
                ID = IDNEXT;
                ++IDNEXT;
            }

            SetGridIdentifier(GridAddShape(this));
        }

        Active = true;
    }

    public void Deactivate()
    {
        if(Active) GridRemoveShape(this);
        
        Active = false;
    }

    public void MoveActive()
    {
        if(Active) SetGridIdentifier(GridMoveShape(this));
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

        if(this is ConvexPolygon thisShape)
        {
            if(poly is ConvexPolygon convPoly)
            {
                thisShape.PolyIntersectsInfo(convPoly, ref result);
                return;
            }
            else if(poly is CircleShape circle)
            {
                thisShape.CircleIntersectsInfo(circle, ref result);
                return;
            }
        }
        else if(this is CircleShape thisShape2)
        {
            if(poly is ConvexPolygon convPoly)
            {
                thisShape2.PolyIntersectsInfo(convPoly, ref result);
                return;
            }
            else if(poly is CircleShape circle)
            {
                thisShape2.CircleIntersectsInfo(circle, ref result);
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
                case ConvexPolygon:
                return ((ConvexPolygon)this).PolyIntersects((ConvexPolygon) poly);
                
                case CircleShape:
                return ((ConvexPolygon)this).CircleIntersects((CircleShape) poly);
            }
            break;

            case CircleShape:
            switch(poly)
            {
                case ConvexPolygon:
                return ((CircleShape)this).PolyIntersects((ConvexPolygon) poly);
                
                case CircleShape:
                return ((CircleShape)this).CircleIntersects((CircleShape) poly);
            }
            break;
        }

        throw new System.Exception($"Shape not implemented! Shape id: {this.GetType()}, {poly.GetType()}.");
    }

    /*public (Shape[] arr, int count) GetIntersectionInfos (List<Shape> shapes)
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
    }*/

    public virtual void Dispose()
    {
        throw new NotImplementedException();
    }

    public static bool PointInConvexPolygon(Vector2 testPoint, Vector2[] polygon)
    {
        //From: https://stackoverflow.com/questions/1119627/how-to-test-if-a-point-is-inside-of-a-convex-polygon-in-2d-integer-coordinates

        //n>2 Keep track of cross product sign changes
        var pos = 0;
        var neg = 0;

        for (var i = 0; i < polygon.Length; i++)
        {
            //If point is in the polygon
            if (polygon[i] == testPoint) break;

            //Form a segment between the i'th point
            var x1 = polygon[i].x;
            var y1 = polygon[i].y;

            //And the i+1'th, or if i is the last, with the first point
            var i2 = (i+1)%polygon.Length;

            var x2 = polygon[i2].x;
            var y2 = polygon[i2].y;

            var x = testPoint.x;
            var y = testPoint.y;

            //Compute the cross product
            var d = (x - x1)*(y2 - y1) - (y - y1)*(x2 - x1);

            if (d > 0) pos++;
            if (d < 0) neg++;

            //If the sign changes, then point is outside
            if (pos > 0 && neg > 0)
                return false;
        }

        //If no change in direction, then on same side of all segments, and thus inside
        return true;
    }
}