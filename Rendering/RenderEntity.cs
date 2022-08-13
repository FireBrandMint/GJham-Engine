
public abstract class RenderEntity : Entity
{
    public static int VisibleEntityCount = 0;

    public bool IsDrawable{get => true;}

    public bool IsTickable{get => Tickable();}

    public bool IsDestroyed{get;set;}

    protected bool Processable = true;

    public bool CanProcess{get => Processable;set => Processable = value;}

    int Z = 0;

    public int ZValue {get => Z; set => Z = value;}

    protected bool Visible = true;

    public bool IsVisible {get => Visible && TrulyVisible();
    set
    {
        if (Visible && !value && Initialized) --VisibleEntityCount;
        if (!Visible && value && Initialized) ++VisibleEntityCount;
        Visible = value;
    }
    }

    ///<summary>
    ///Is the object truly visible if it's visible?
    ///</summary>
    protected abstract bool TrulyVisible();

    protected abstract bool Tickable();

    protected bool Initialized = false;

    public virtual void Init()
    {
        if (Visible) ++VisibleEntityCount;

        Initialized = true;
    }

    public virtual void EnterTree()
    {

    }

    public virtual void LeaveTree()
    {
        if(Visible) --VisibleEntityCount;
    }

    public virtual void Tick ()
    {

    }

    public virtual DrawableObject GetDrawable ()
    {
        return null;
    }
}