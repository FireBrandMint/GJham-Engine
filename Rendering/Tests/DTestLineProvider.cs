using SFML.Graphics;
using System;

public class DTestLineProvider : RenderEntity
{
    //test for drawable entity
    public Vector2[] line = null;

    Vector2 Position = new Vector2();

    Vector2 LastPosition;

    public override void Init()
    {
        base.Init();
        LastPosition = Position;
    }

    int t = 0;

    public override void Tick ()
    {
        LastPosition = Position;

        int factor = (((int)Engine.MaxTPS) / 10);
        
        if (t < 60)
        {
            Position += new Vector2((FInt)10,(FInt)(-5));

            ++t;
        }
        else
        {
            Position -= new Vector2((FInt)t * 10,(FInt)t * -5 );

            t=0;
        }
    }

    DrawableLine2D drawable = null;

    public override DrawableObject GetDrawable ()
    {
        if (line == null) return null;

        if (drawable == null) drawable = new DrawableLine2D(line[0], line[1], Color.Cyan, Color.Red, Position, LastPosition, ZValue);
        else drawable.Reinitialize(line[0], line[1], Color.Cyan, Color.Red, Position, LastPosition, ZValue);

        return drawable;
    }

    protected override bool Tickable() => true;
}