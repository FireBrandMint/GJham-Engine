using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class ShapeColliderBase : Entity, CollisionAntenna
{
    public delegate void ColResponse (CollisionAntenna colSubject, ref CollisionResult colInfo);

    bool _IDSet = false;
    int _ID = -1;
    bool _IsDestroyed = false;
    bool _IsVisible = false;
    int _Z = 0;
    Entity _Parent = null;
    NodeChildren<Entity> _Children = null;

    Vector2 _Position = new Vector2();

    public bool IDSet { get => _IDSet; set => _IDSet = value; }

    public int ID { get => _ID; set {_IDSet = true; _ID = value;} }

    bool ShapeIDSet = false;
    int _ShapeID;

    public int ShapeID
    {
        get => _ShapeID;
        set
        {
            if(InTree) CollisionFodder.ID = _ShapeID;

            _ShapeID = value;
            ShapeIDSet = true;
        }
    }

    public bool IsDrawable => false;

    public bool IsTickable => false;
    

    public bool CanProcess { get => false; set {} }
    public bool IsDestroyed { get => _IsDestroyed; set => _IsDestroyed = value; }

    public bool IsVisible { get => _IsVisible; set => _IsVisible = value; }
    public int ZValue { get => _Z; set => _Z = value; }
    public Entity Parent { get => _Parent; set => _Parent = value; }
    public NodeChildren<Entity> Children { get => _Children; set => _Children = value; }
    public bool IsStatic = false;

    protected bool InTree = false;
    
    bool _Active = true;

    public bool Active
    {
        get => _Active;
        set
        {
            if(InTree)
            {
                if(value) Activate();
                else Deactivate();
            }

            _Active = value;
        }
    }

    public Vector2 Position
    {
        get => _Position;
        set
        {
            if (InTree) CollisionFodder.Position = value;
            _Position = value;
        }
    }

    protected Shape CollisionFodder = null;

    protected virtual Shape CreateShape()
    {
        return ConvexPolygon.CreateRect(new Vector2(50,50), new Vector2(1,1), (FInt)0, Position);
    }

    public void Activate(bool Initializing = false)
    {
        if(!Initializing && _Active) return;

        CollisionFodder.Activate();
        GlobalCollision.AddShape(ID, CollisionFodder);

        _Active = true;
    }

    public void Deactivate(bool Destroying = false)
    {
        if(!_Active) return;

        CollisionFodder.Deactivate();
        GlobalCollision.RemoveShape(ID);

        if(!Destroying) _Active = false;
    }

    public void Init()
    {
        if(CollisionFodder == null) CollisionFodder = CreateShape();

        if(ShapeIDSet) CollisionFodder.ID = ShapeID;
    }

    public void EnterTree()
    {
        CollisionFodder.ObjectUsingIt = this;

        if(_Active) Activate(true);

        InTree = true;
    }

    public DrawableObject GetDrawable()
    {
        return null;
    }

    public void LeaveTree()
    {
        Deactivate(true);

        InTree = false;
    }

    public void Tick()
    {
        
    }

    public virtual void OnCollision(CollisionAntenna colSubject, ref CollisionResult colInfo)
    {
        
    }

    public virtual void ResolveOverlap(CollisionAntenna colSubject, ref CollisionResult colInfo)
    {
        
    }
}