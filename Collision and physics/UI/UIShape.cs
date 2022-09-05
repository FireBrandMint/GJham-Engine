using System;
using System.Collections.Generic;

public class UIShape
{
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
}