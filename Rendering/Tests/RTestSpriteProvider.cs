using SFML.System;

public class RTestSpriteProvider : RenderEntity
{
    bool HasTexture = false;

    string _TexturePath = "";

    bool TextureChanged = false;

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

                TextureChanged = true;
            }

            _TexturePath = value;
        }
    }

    public bool IsStatic = true;

    Vector2 LastPosition;

    public Vector2 Position = new Vector2();

    public FInt Rotation = (FInt)45;

    public bool Rotate = true;

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

    public override void Tick()
    {
        base.Tick();

        if(!Rotate) return;

        Rotation += 1;

        if(Rotation > 360) Rotation -= 360;
    }

    DrawableSprite2D drawable = null;
    bool dInitialized = false;

    public override DrawableObject GetDrawable ()
    {
        if(!HasTexture) return null;

        if(!dInitialized)
        {
            drawable = new DrawableSprite2D( ZValue,
                TexturePath, LastPosition, Position, 
                new Vector2u[]{TextureAreaTopLeft, TextureAreaBottomRight},
                BoundriesSet, Rotation, IsStatic);
            
            dInitialized = true;
        }
        else
        {
            drawable.z = ZValue;

            if(TextureChanged) drawable.ChangeTexturePath(_TexturePath);

            drawable.SetRotation(Rotation);
        }

        return drawable;
    }

    public override void LeaveTree()
    {
        TexturePath = null;
        
        inTree = false;

        if(drawable != null)
        {
            drawable.DisposeResources();
            drawable = null;
        }
    }

    protected override bool Tickable()
    {
        return true;
    }
}