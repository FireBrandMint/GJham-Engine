using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Camera : Entity
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

    public static Camera MainCamera = null;

    bool _IsMain = false;

    public bool IsMain {get => _IsMain;
        set
        {
            if(value && !_IsMain)
            {
                if(MainCamera != null) MainCamera.IsMain = false;

                MainCamera = this;
        
            }
            else if(!value && _IsMain)
            {
                MainCamera = null;
            }

            _IsMain = value;
        }
    }

    private Vector2 _Position;
    public Vector2 Position{get => _Position + Engine.WindowSize/2;
        set
        {
            _Position = value - Engine.WindowSize/2;
            Engine.ViewPos = _Position;
        }
    }

    //Engine.ViewPos = _Position

    public bool IsDrawable => false;

    public bool IsTickable => false;

    public bool CanProcess { get => _IsMain; set => IsMain = value; }

    bool _IsDestroyed = false;

    public bool IsDestroyed { get => _IsDestroyed; set => _IsDestroyed = value; }
    public int ZValue { get => 0; set{} }

    bool _IsVisible = true;

    public bool IsVisible { get => _IsVisible; set => _IsVisible = value; }

    Entity _Parent = null;
    NodeChildren<Entity> _Children = null;

    public Entity Parent { get => _Parent; set => _Parent = value; }
    public NodeChildren<Entity> Children { get => _Children; set => _Children = value; }

    public void EnterTree()
    {
        if(_IsMain) Engine.ViewPos = _Position;
    }

    public DrawableObject GetDrawable()
    {
        return null;
    }

    public void Init()
    {
        
    }

    public void Tick()
    {
        
    }

    public void LeaveTree()
    {
        if(IsMain) IsMain = false;
    }
}