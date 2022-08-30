using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.System;
using GJham.Rendering.Optimization;


public class SpriteEntity : RenderEntity
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
                    TextureHolder.RegisterTextureRef(ref value, 5000);
                    HasTexture = true;
                }


                Changed = true;
                TextureChanged = true;
            }

            _TexturePath = value;
        }
    }

    bool Changed = true;

    public bool IsStatic = false;

    Vector2 LastPosition;

    Vector2 _Position = new Vector2();

    /// <summary>
    /// SET sets LastPosition to the last value
    /// of _Position and _Position is replaced with the specified value.
    /// GET returns _Position.
    /// </summary>
    /// <value></value>
    public Vector2 Position
    {
        get => _Position;
        set
        {
            LastPosition = _Position;
            _Position = value;

            if(inTree) RenderCulling.ChangePosition(_Position - (_CullingRange >> 1));
        }
    }

    Vector2 _Scale = new Vector2(1,1);

    public Vector2 Scale
    {
        get => _Scale;
        set
        {
            Changed = Changed || _Scale != value;
            _Scale = value;
        }
    }

    private FInt _Rotation = (FInt)0;
    public FInt Rotation
    {
        get => _Rotation;
        set
        {
            Changed = Changed || _Rotation != value;
            _Rotation = value;
        }
    }

    SFML.Graphics.Color _Modulate = SFML.Graphics.Color.White;

    public SFML.Graphics.Color Modulate
    {
        get => _Modulate;
        set
        {
            _Modulate = value;
            Changed = Changed || _Modulate != value;
        }
    }

    bool inTree = false;

    Vector2u _TextureAreaTopLeft = new Vector2u();
    Vector2u _TextureAreaBottomRight = new Vector2u();

    public Vector2u TextureAreaTopLeft
    {
        get => _TextureAreaTopLeft;
        set
        {
            Changed = Changed || _TextureAreaTopLeft != value;
            BoundriesSet = true;

            _TextureAreaTopLeft = value;
        }
    }

    public Vector2u TextureAreaBottomRight
    {
        get => _TextureAreaBottomRight;
        set
        {
            Changed = Changed || _TextureAreaBottomRight != value;
            _TextureAreaBottomRight = value;
        }
    }

    Vector2 _CullingRange = new Vector2(100, 100);
    
    public Vector2 CullingRange
    {
        get => _CullingRange;
        set
        {
            _CullingRange = value;
            if(inTree)
            {
                RenderCulling.ChangePosition(_Position - (value >> 1));
                RenderCulling.ChangeRange(value);
            }
        }
    }

    bool BoundriesSet = false;

    CullingAABB RenderCulling = new CullingAABB(new AABB(new Vector2(0, 0), new Vector2(50, 50)));

    public override sealed void Init()
    {
        base.Init();

        LastPosition = Position;

        RenderCulling = new CullingAABB(new AABB(Position - (_CullingRange >> 1), _CullingRange));

        if(_TexturePath != null && _TexturePath != "")
        {
            TextureHolder.RegisterTextureRef(ref _TexturePath, 5000);
            HasTexture = true;
        }

        inTree = true;

        this.CanProcess = false;
    }
    DrawableSprite2D drawable = null;

    public override DrawableObject GetDrawable ()
    {
        if(!HasTexture) return null;

        if(drawable == null)
        {
            drawable = new DrawableSprite2D(
                ZValue, TexturePath,
                LastPosition, Position, new Vector2(1, 1),
                new Vector2u[]{TextureAreaTopLeft, TextureAreaBottomRight},
                BoundriesSet, Rotation, IsStatic);
        }

        if(Changed)
        {
            drawable.SetPosValues(Position, LastPosition);
            drawable.z = ZValue;

            if(TextureChanged) drawable.ChangeTexturePath(_TexturePath);

            drawable.SetRotation(Rotation);

            drawable.ChangeModulate(Modulate);

            if (BoundriesSet) drawable.ChangeBoundries(TextureAreaTopLeft, TextureAreaBottomRight);
            
            drawable.SetScale(_Scale);
        }

        return drawable;
    }

    public override void LeaveTree()
    {
        //Unregisters texture reference.
        TextureHolder.UnregisterTextureRef(ref _TexturePath);
        
        inTree = false;

        if(drawable != null)
        {
            drawable.DisposeResources();
            drawable = null;
        }

        RenderCulling.Dispose();
        RenderCulling = null;
    }

    protected override bool Tickable()
    {
        return true;
    }

    protected override bool TrulyVisible() => RenderCulling.CanRender();
}