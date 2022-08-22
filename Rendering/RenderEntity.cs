
public abstract class RenderEntity : Entity
{
    bool _IDSet = false;

    public bool IDSet
    {
        get => _IDSet;
        set => _IDSet = value;
    }

    int _ID;
    public int ID
    {
        get => _ID;
        set
        {
            _IDSet = true;
            _ID = value;
        }
    }

    public static int VisibleEntityCount = 0;

    public bool IsDrawable{get => true;}

    public bool IsTickable{get => Tickable();}

    public bool IsDestroyed{get;set;}

    protected bool Processable = true;

    public bool CanProcess{get => Processable;set => Processable = value;}

    public int ZValue {get; set;}

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