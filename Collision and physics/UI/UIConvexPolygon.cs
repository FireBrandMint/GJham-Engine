
public class UIConvexPolygon : UIShape
{
    Vector2[] OriginalModel;

    public static UIConvexPolygon CreateRect (Vector2 position, Vector2 scale, UIAdjustmentMode mode)
    {
        FInt x = scale.x >> 1;
        FInt y = scale.y >> 1;

        Vector2[] model = new Vector2[]
        {
            //top left
            new Vector2(-x, -y),
            //bottom left
            new Vector2(-x, y),
            //bottom right
            new Vector2(x, y),
            //top right
            new Vector2(x, -y),
        };

        return new UIConvexPolygon(position, model, mode);
    }

    public static UIConvexPolygon CreateTriangle (Vector2 position, Vector2 scale, UIAdjustmentMode mode)
    {
        FInt x = scale.x >> 1;
        FInt y = scale.y >> 1;

        Vector2[] model = new Vector2[]
        {
            //bottom left
            new Vector2(-x, y),
            //bottom right
            new Vector2(x, y),
            //top
            new Vector2((FInt)0, -y),
        };

        return new UIConvexPolygon(position, model, mode);
    }

    /// <summary>
    /// Position is the position of the polygon with vector
    /// corresponding to porcentage of the screen 0 to 100.
    /// Model is the model of the convex polygon with vector
    /// corresponding to porcentages of the screen -100 to 100.
    /// </summary>
    /// <param name="Model"></param>
    public UIConvexPolygon (Vector2 position, Vector2[] Model, UIAdjustmentMode mode)
    {
        OriginalModel = Model;
        Position = position;

        Mode = mode;

        UIShape.Shapes.Add(this);
    }

    public override bool IsColliding(Vector2 mousePoint)
    {
        int lenght = OriginalModel.Length;

        Vector2[] ProducedModel = new Vector2[lenght];

        Vector2 viewSize = Engine.ViewSize;

        if(Mode == UIAdjustmentMode.Compact)
        {
            if(viewSize.x < viewSize.y)
            {
                viewSize = new Vector2(viewSize.x, viewSize.x);
            }
            else
            {
                viewSize = new Vector2(viewSize.y, viewSize.y);
            }
        }

        Vector2 currPos = (Position * viewSize) / 100;

        for(int i = 0; i < lenght; ++i)
        {
            ProducedModel[i] = currPos + (( OriginalModel[i] * viewSize) / 100);
        }

        return PointInConvexPolygon(mousePoint, ProducedModel);
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