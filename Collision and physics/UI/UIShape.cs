using System;
using System.Collections.Generic;

public class UIShape
{
    public static List<UIShape> Shapes = new List<UIShape>(10);

    public Vector2 Position;

    public UIAdjustmentMode Mode;

    /// <summary>
    /// The object that is using this shape for collision.
    /// </summary>
    public CollisionAntenna ObjectUsingIt = null;

    public virtual bool IsColliding(Vector2 point)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        Shapes.Remove(this);
    }
}