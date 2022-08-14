using System;
using System.Collections.Generic;
using System.Threading;

namespace GJham.Rendering.Optimization;

///<summary>
///Class that asynchronously processes
///CullingAABB objects, you don't need
///to understand what it does, it's
///useless knowlege.
///</summary>
public static class CullingMaster
{
    static CullingMaster ()
    {
        Engine.ExecuteOnCloseProgram(OnExit);

        Thread thread = new Thread(Run);

        thread.Start();
    }

    static bool ProgramExists = true;

    static List<CulValue> ToAdd = new List<CulValue>();

    static List<CulValue> ToRemove = new List<CulValue>();

    static List<CulValue> ToProcess = new List<CulValue>();

    public static void Add(CulValue value)
    {
        lock (ToAdd) ToAdd.Add(value);
    }

    public static void Remove(CulValue value)
    {
        lock (ToRemove) ToRemove.Add(value);
    }

    static void OnExit()
    {
        ProgramExists = false;
    }

    static void Run()
    {
        while (ProgramExists)
        {
            Thread.Sleep(30);

            lock(ToAdd)
            {
                for(int i = ToAdd.Count - 1; i >= 0; --i)
                {
                    ToProcess.Add(ToAdd[i]);

                    ToAdd.RemoveAt(i);
                }
            }

            lock(ToRemove)
            {
                for(int i = ToRemove.Count - 1; i >= 0; --i)
                {
                    ToProcess.Remove(ToRemove[i]);

                    ToRemove.RemoveAt(i);
                }
            }

            AABB screenAABB = new AABB(Engine.ViewPos, Engine.WindowSize);

            for(int i = 0; i < ToProcess.Count; ++i)
            {
                CulValue curr = ToProcess[i];

                lock(curr)
                {
                    curr.CanRender = screenAABB.Intersects(ref curr.ColShape);
                }
            }
        }
    }
}

public class CulValue
{
    public AABB ColShape;

    public bool CanRender = false;

    bool Processing = false; 

    public CulValue(AABB Shape)
    {
        ColShape = Shape;
    }

    public void StartProcessing()
    {
        if(!Processing) CullingMaster.Add(this);

        Processing = true;
    }

    public void StopProcessing()
    {
        if(Processing) CullingMaster.Remove(this);

        Processing = false;
    }

    public void ChangePos (Vector2 pos) => ColShape.Origin = pos;

    public void ChangeRange (Vector2 range) => ColShape.Ranges = range;


}