using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GJham.Rendering.Optimization;

///<summary>
///Class used for culling in rendering.
///The AABB is tested against the screen AABB
///space to see if it's necessary to render
///the object.
///Starts active, to retrieve wether the
///thing you're using can render or not
///use the CanRender() function.
///NOTE TO SELF: please update the AABB
///position and range if a object moves or changes size.
///</summary>
public class CullingAABB
{
    CulValue CAABB;

    public bool AlwaysVisible
    {
        get
        {
            lock (CAABB) return CAABB.AlwaysVisible;
        }
        set
        {
            lock (CAABB) CAABB.AlwaysVisible = value;
        }
    }

    public int ItemID
    {
        get
        {
            lock (CAABB) return CAABB.ItemID;
        }
        set
        {
            lock (CAABB) CAABB.ItemID = value;
        }
    }

    public CullingAABB(AABB objectArea, int itemID, bool AlwaysVisible)
    {
        CAABB = new CulValue(objectArea, itemID);

        CAABB.StartProcessing();
    }

    public void RestartCulling()
    {
        lock (CAABB) CAABB.StartProcessing();
    }

    public void StopCulling()
    {
        lock (CAABB) CAABB.StopProcessing();
    }

    public void Dispose()
    {
        StopCulling();
    }

    public void ChangePosition(Vector2 pos)
    {
        lock(CAABB) CAABB.ChangePos(pos);
    }

    public void ChangeRange(Vector2 range)
    {
        lock(CAABB) CAABB.ChangeRange(range);
    }

    public bool CanRender ()
    {
        bool result = false;
        lock(CAABB) result = CAABB.CanRender;

        return result;
    }
}