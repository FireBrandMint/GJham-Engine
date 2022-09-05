using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class UICollider : Entity
{
    public static List<UICollider> UIColliders = new List<UICollider>();

    public delegate void ColResponse (CollisionAntenna colSubject, ref CollisionResult colInfo);

    bool _IDSet = false;
    int _ID = -1;
    bool _IsDestroyed = false;
    bool _IsVisible = true;
    int _Z = 0;
    Entity _Parent = null;
    NodeChildren<Entity> _Children = null;

    Vector2 _Position = new Vector2();

    public bool IDSet { get => _IDSet; set => _IDSet = value; }

    public int ID { get => _ID; set {_IDSet = true; _ID = value;} }

    public bool IsDrawable => false;

    public bool IsTickable => false;
    

    public bool CanProcess { get => false; set {} }
    public bool IsDestroyed { get => _IsDestroyed; set => _IsDestroyed = value; }

    public bool IsVisible { get => _IsVisible; set => _IsVisible = value; }
    public int ZValue { get => _Z; set => _Z = value; }
    public Entity Parent { get => _Parent; set => _Parent = value; }
    public NodeChildren<Entity> Children { get => _Children; set => _Children = value; }

    public Vector2 Position
    {
        get => _Position;
        set
        {
            if (CollisionFodder != null) CollisionFodder.Position = value;
            _Position = value;
        }
    }

    bool _Active = true;

    public bool Active
    {
        get => _Active;
        set
        {

            SetActive(value);
        }
    }

    protected UIShape CollisionFodder = null;

    public void Init()
    {
        CollisionFodder = CreateShape();

        SetActive(Active, true);
    }

    public virtual void EnterTree()
    {

    }

    public DrawableObject GetDrawable()
    {
        return null;
    }

    public void LeaveTree()
    {
        Active = false;
    }

    public void Tick()
    {
        
    }

    protected virtual UIShape CreateShape ()
    {
        return UIConvexPolygon.CreateRect(_Position, new Vector2(25, 25), UIAdjustmentMode.Compact);
    } 

    public bool Intersects(Vector2 cursor)
    {
        if(!Active) return false;

        return CollisionFodder.IsColliding(cursor);
    }

    public virtual void OnMouseAbove(bool clicked)
    {

    }

    void SetActive(bool active, bool initializing = false)
    {
        if(initializing && active)
        {
            UIColliders.Add(this);
        }

        //If the state hasn't changed, then bail.
        if (Active == active) return;

        if(active)
        {
            UIColliders.Add(this);
        }
        else
        {
            UIColliders.Remove(this);
        }

        Active = active;
    }
}