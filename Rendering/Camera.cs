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

    public Vector2 Position;

    public bool IsDrawable => false;

    public bool IsTickable => true;

    public bool CanProcess { get => _IsMain; set => IsMain = value; }

    bool _IsDestroyed = false;

    public bool IsDestroyed { get => _IsDestroyed; set => _IsDestroyed = value; }
    public int ZValue { get => 0; set{} }

    public void EnterTree()
    {

    }

    public DrawableObject GetDrawable()
    {
        return null;
    }

    public void Init()
    {
        
    }

    public void LeaveTree()
    {
        if(IsMain) IsMain = false;
    }

    public void Tick()
    {
        Engine.ViewPos = Position;
    }
}