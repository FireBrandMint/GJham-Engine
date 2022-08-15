using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Camera : Entity
{
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