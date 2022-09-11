using SFML.Graphics;
using System;
using GJham.Rendering.Optimization;

public class DTestLineProvider : RenderEntity
{
    //test for drawable entity
    public Vector2[] line = null;

    Vector2 _Position = new Vector2();

    public override sealed Vector2 Position
    {
        get => _Position;
        set => _Position = value;
    }

    Vector2 LastPosition;

    CullingAABB Culling;

    public override void Init()
    {
        base.Init();
        LastPosition = Position;

        Culling = new CullingAABB(new AABB(new Vector2(), new Vector2()), ID, true);
    }

    int t = 0;

    public override void Tick ()
    {
        LastPosition = Position;

        int factor = (Engine.MaxTPS / 10);
        
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

    protected override bool TrulyVisible()
    {
        return true;
    }
}