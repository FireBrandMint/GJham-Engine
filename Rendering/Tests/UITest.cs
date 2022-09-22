using GJham.Rendering.Optimization;

public class UITest : RenderEntity
{
    public override Vector2 Position { get; set; }

    public string TexturePath = "";

    CullingAABB RenderCulling;

    public override void EnterTree()
    {
        TextureHolder.RegisterTextureRef(ref TexturePath, 5000);

        RenderCulling = new CullingAABB(new AABB(Position, new Vector2()), ID, true);

        Visible = true;
    }

    UISprite2D drawable = null;

    public override DrawableObject GetDrawable()
    {
        if(drawable == null)
        {
            drawable = new UISprite2D(TexturePath, new Vector2(10, 10), new Vector2(5, 5), UIAdjustmentMode.Compact);
            drawable.z = ZValue;
        }

        return drawable;
    }

    public override void LeaveTree()
    {
        TextureHolder.UnregisterTextureRef(ref TexturePath);
        RenderCulling.Dispose();
    }


    protected override bool Tickable() => false;

    protected override bool TrulyVisible() => true;
}