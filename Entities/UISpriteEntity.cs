using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.System;
using GJham.Rendering.Optimization;


public class UISpriteEntity : RenderEntity
{
    public override int ID
    {
        get => _ID;
        set
        {
            if(InTree) RenderCulling.ItemID = value;

            _IDSet = true;
            _ID = value;
        }
    }

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
            if (InTree)
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
    public override sealed Vector2 Position
    {
        get => _Position;
        set
        {
            LastPosition = _Position;
            _Position = value;

            //if(InTree) RenderCulling.ChangePosition(_Position - (_CullingRange >> 1));
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

    /*private FInt _Rotation = (FInt)0;
    public FInt Rotation
    {
        get => _Rotation;
        set
        {
            Changed = Changed || _Rotation != value;
            _Rotation = value;
        }
    }*/

    UIAdjustmentMode _AdjustmentMode = UIAdjustmentMode.Compact;

    public UIAdjustmentMode AdjustmentMode
    {
        get => _AdjustmentMode;
        set
        {
            Changed = true;
            _AdjustmentMode = value;
        }
    }

    SFML.Graphics.Color _Modulate = SFML.Graphics.Color.White;

    public SFML.Graphics.Color Color
    {
        get => _Modulate;
        set
        {
            _Modulate = value;
            Changed = Changed || _Modulate != value;
        }
    }

    bool InTree = false;

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

    bool BoundriesSet = false;

    CullingAABB RenderCulling;

    public override sealed void Init()
    {
        base.Init();

        LastPosition = Position;

        RenderCulling = new CullingAABB(new AABB(Position, new Vector2()), ID, true);

        if(_TexturePath != null && _TexturePath != "")
        {
            TextureHolder.RegisterTextureRef(ref _TexturePath, 5000);
            HasTexture = true;
        }

        InTree = true;

        this.CanProcess = false;
    }
    UISprite2D drawable = null;

    public override DrawableObject GetDrawable ()
    {
        if(!HasTexture) return null;

        if(drawable == null)
        {
            drawable = new UISprite2D(TexturePath, Position, Scale, _AdjustmentMode);
        }

        if(Changed)
        {
            drawable.ChangePosition(Position);
            drawable.z = ZValue;

            if(TextureChanged) drawable.ChangeTexture(ref _TexturePath, TextureAreaTopLeft, TextureAreaBottomRight);

            //drawable.SetRotation(Rotation);

            drawable.ChangeModulate(Color);

            if (BoundriesSet) drawable.ChangeBoundries(TextureAreaTopLeft, TextureAreaBottomRight);
            
            drawable.ChangeSize(_Scale);
        }

        return drawable;
    }

    public override void LeaveTree()
    {
        //Unregisters texture reference.
        TextureHolder.UnregisterTextureRef(ref _TexturePath);
        
        InTree = false;

        if(drawable != null)
        {
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