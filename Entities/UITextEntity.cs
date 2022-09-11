using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GJham.Rendering.Optimization;

public class UITextEntity : RenderEntity
{

    string _Path = null;

    bool InTree = false;

    bool Changed = true;

    Vector2 _Position = new Vector2();

    Vector2 _Scale = new Vector2(1,1);

    FInt _Rotation = new FInt(0);

    SFML.Graphics.Color _Color = SFML.Graphics.Color.White;

    string _Text = "Lorem Lpsum";

    int _LetterSpacing = 2;

    int _LineSpacing = 2;

    int _CharacterSize = 10;

    public override Vector2 Position
    {
        get => _Position;
        set
        {
            Changed = true;

            _Position = value;
        }
    }

    public Vector2 Scale
    {
        get => _Scale;
        set
        {
            Changed = true;

            _Scale = value;
        }
    }

    public FInt Rotation
    {
        get => _Rotation;
        set
        {
            Changed = true;

            _Rotation = value;
        }
    }

    public SFML.Graphics.Color Color
    {
        get => _Color;
        set
        {
            Changed = true;

            _Color = value;
        }
    }

    public string Text
    {
        get => _Text;
        set
        {
            Changed = true;

            _Text = value;
        }
    }
    
    public int LetterSpacing
    {
        get => _LetterSpacing;
        set
        {
            Changed = true;

            _LetterSpacing = value;
        }
    }

    public int LineSpacing
    {
        get => _LineSpacing;
        set
        {
            Changed = true;

            _LineSpacing = value;
        }
    }
    
    public int CharacterSize
    {
        get => _CharacterSize;
        set
        {
            Changed = true;

            _CharacterSize = value;
        }
    }

    bool PathChanged = true;

    public string FontPath
    {
        get => _Path;
        set
        {
            if(value == _Path) return;

            if(InTree)
            {
                if(!(_Path == null || _Path == "")) FontHolder.UnregisterReference(_Path);

                if(!(value == null || value == "")) FontHolder.RegisterReference(value);

                PathChanged = true;
            }

            _Path = value;
        }
    }

    CullingAABB RenderCulling;

    public override void Init()
    {
        base.Init();

        RenderCulling = new CullingAABB(new AABB(Vector2.ZERO, Vector2.ZERO), ID, true);

        FontHolder.RegisterReference(_Path);

        InTree = true;
    }

    public override void LeaveTree()
    {
        FontHolder.UnregisterReference(_Path);

        InTree = false;
    }

    DrawableText2D Drawable = null;

    public override DrawableObject GetDrawable()
    {
        if(Drawable == null)
        {
            Drawable = new DrawableText2D(_Path, _Position);
        }

        if(PathChanged)
        {
            PathChanged = false;
        }

        if(Changed)
        {
            //Braindead code here.
            //Most efficient way to do this would be a single method
            //in the drawable, but it would also be the most difficult
            //to understand.
            Drawable.ChangeText(_Text);
            Drawable.ChangeScale(_Scale);
            Drawable.ChangeRotation(_Rotation);
            Drawable.ChangeLineSpacing(_LineSpacing);
            Drawable.ChangeLetterSpacing(_LetterSpacing);
            Drawable.ChangeColor(_Color);
            Drawable.ChangeCharacterSize(CharacterSize);

            Changed = false;
        }

        return Drawable;
    }

    protected override bool Tickable()
    {
        return false;
    }

    protected override bool TrulyVisible()
    {
        return true;
    }
}