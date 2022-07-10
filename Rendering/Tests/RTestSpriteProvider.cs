using SFML.System;

public class RTestSpriteProvider : RenderEntity
{
    bool HasTexture = false;

    string _TexturePath = "";

    FInt Rotation = (FInt)0;

    public string TexturePath
    {
        get
        {
            return _TexturePath;
        }
        set
        {
            if (inTree)
            {
                if(_TexturePath != "" && _TexturePath != null)
                {
                    TextureHolder.UnregisterTextureRef(ref _TexturePath);
                    HasTexture = false;
                }
            
                if (value != "" && value != null)
                {
                    TextureHolder.RegisterTextureRef(ref value);
                    HasTexture = true;
                }
            }

            _TexturePath = value;
        }
    }

    Vector2 LastPosition;

    public Vector2 Position = new Vector2();

    bool inTree = false;

    public Vector2u TextureAreaTopLeft = new Vector2u();
    public Vector2u TextureAreaBottomRight = new Vector2u();

    public bool BoundriesSet = false;

    public override void Init()
    {
        base.Init();
        LastPosition = Position;

        if(_TexturePath != null && _TexturePath != "")
        {
            TextureHolder.RegisterTextureRef(ref _TexturePath);
            HasTexture = true;
        }

        inTree = true;
    }

    DrawableSprite2D drawable = null;
    bool dInitialized = false;

    public override DrawableObject GetDrawable ()
    {
        if(!HasTexture) return null;

        if(!dInitialized)
        {
            drawable = new DrawableSprite2D(
                TexturePath, LastPosition, Position, 
                new Vector2u[]{TextureAreaTopLeft, TextureAreaBottomRight},
                BoundriesSet, Rotation);
            
            dInitialized = true;
        }
        else
        {
            //TODO: reset object
        }

        return drawable;
    }

    public override void LeaveTree()
    {
        TexturePath = "";
        
        inTree = false;
    }
}