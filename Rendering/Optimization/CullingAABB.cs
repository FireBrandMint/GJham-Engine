using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GJham.Rendering.Optimization;
public class CullingAABB
{
    CulValue CAABB;

    public CullingAABB(AABB objectArea)
    {
        CAABB = new CulValue(objectArea);

        lock (CAABB) CAABB.StartProcessing();
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