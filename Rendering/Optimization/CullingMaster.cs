using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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


    static int CanDrawCount = 0;
    static int[] CanDrawIDS = new int[50]; 

    static int VisibleNodesCount = 0;

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
        WTFHashSet culHashSet = new WTFHashSet(50);

        while (ProgramExists)
        {
            Thread.Sleep(30);

            int tAddC = 0;

            lock(ToAdd) tAddC = ToAdd.Count;

            for(int i = tAddC - 1; i >= 0; --i)
            {
                lock(ToAdd)
                {
                    ToProcess.Add(ToAdd[i]);

                    ToAdd.RemoveAt(i);
                }
            }

            int tRemoveC = 0;

            lock(ToRemove) tRemoveC = ToRemove.Count;

            for(int i = tRemoveC - 1; i >= 0; --i)
            {
                lock(ToRemove)
                {
                    ToProcess.Remove(ToRemove[i]);

                    ToRemove.RemoveAt(i);
                }
            }

            //Process to determine what item is visible and what is not below.

            culHashSet.Clear();

            AABB screenAABB = new AABB(Engine.ViewPos, Engine.WindowSize);

            int visNodesCount = 0;

            for(int i = 0; i < ToProcess.Count; ++i)
            {
                CulValue curr = ToProcess[i];

                bool canRender = false;
                int ID = 0;

                lock(curr)
                {
                    ID = curr.ItemID;
                    canRender = curr.AlwaysVisible || screenAABB.Intersects(ref curr.ColShape);
                    curr.CanRender = canRender;
                }

                if(canRender)
                {
                    culHashSet.AddIfNonexist(ID);
                    ++visNodesCount;
                }
            }

            var visibleIds = culHashSet.GetInternalList();

            lock(CanDrawIDS)
            {
                VisibleNodesCount = visNodesCount;

                int length = visibleIds.Count;
                if(CanDrawIDS.Length < length) Array.Resize(ref CanDrawIDS, length);

                for(int i = 0; i < length; ++i)
                {
                    CanDrawIDS[i] = visibleIds[i];
                }

                CanDrawCount = length;


            }
        }
    }

    public static int[] GetVisiblesIDS(out int visibleNodesCount)
    {
        lock(CanDrawIDS)
        {
            int[] fodder = new int[CanDrawCount];

            Array.Copy(CanDrawIDS, fodder, CanDrawCount);

            visibleNodesCount = VisibleNodesCount;

            return fodder;
        }
    }
}

public class CulValue
{
    public AABB ColShape;

    public int ItemID = -1;

    public bool CanRender = false;

    public bool AlwaysVisible = false;

    bool Processing = false; 

    public CulValue(AABB Shape, int itemID)
    {
        ColShape = Shape;
        ItemID = itemID;
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

    public void ChangeItemID (int newID)
    {
        ItemID = newID;
    }

    public void ChangePos (Vector2 pos) => ColShape.Origin = pos;

    public void ChangeRange (Vector2 range) => ColShape.Ranges = range;


}