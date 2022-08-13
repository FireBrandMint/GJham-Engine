
public class UITest : RenderEntity
{
    public string TexturePath = "";

    public override void EnterTree()
    {
        TextureHolder.RegisterTextureRef(ref TexturePath, 5000);
    }

    UISprite2D drawable = null;

    public override DrawableObject GetDrawable()
    {
        if(drawable == null)
        {
            drawable = new UISprite2D(TexturePath, new Vector2(50, 50), new Vector2(50, 50), UISprite2D.AdjustmentMode.Compact);
            drawable.z = ZValue;
        }

        return drawable;
    }

    public override void LeaveTree()
    {
        TextureHolder.UnregisterTextureRef(ref TexturePath);
    }


    protected override bool Tickable() => false;

    protected override bool TrulyVisible() => true;
}