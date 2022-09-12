using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SFML.Graphics;
using SFML.System;

/// <summary>
/// Very confusing class, do not read.
/// It loads textures asynchronously,
/// as 'references'.
/// </summary>
public class TextureHolder
{
    private static GDictionary<string, TextureHolder> TextureDict = new GDictionary<string, TextureHolder>();

    public Texture texture;

    string texPath;

    int refCount = 0;

    public bool Disposed = false;

    static ManualResetEvent operate = new ManualResetEvent(false);

    static Object lockOperate = new Object();

    static Queue<(string path, int ttk)> RegQueue = new Queue<(string path, int ttk)>();

    static Queue<string> DeregQueue = new Queue<string>();

    static List<TextureHolder> ToRemove = new List<TextureHolder>();

    int TimeToKillLimit, TimeToKill;

    private TextureHolder(ref String path, int timeToKill)
    {
        try
        {
            texture = new Texture(path);
        }
        catch(Exception e)
        {
            Console.WriteLine($"Couldn't load texture \"{path}\", error message: {e.Message}");
        }

        TimeToKill = timeToKill;
        TimeToKillLimit = TimeToKill;

        texPath = path;

        Console.WriteLine($"Loaded texture \"{path}\".");

        refCount = 1;
    }

    void AddRef() => ++refCount;

    bool RemoveRef()
    {
        int c = refCount - 1;

        bool result = c == 0;

        refCount = c;

        return result;
    }

    bool TickKill(int ms)
    {
        TimeToKill -= ms;

        return TimeToKill <= 0;
    }

    void Dispose()
    {
        if (Disposed) return;

        texture.Dispose();

        Disposed = true;
    }

    ///<summary>
    ///Registers a reference of a texture (and loads it in a thread if it doesn't exist)
    ///and the time in MS to remove it from memory when there's
    ///no reference to it.
    ///</summary>
    public static void RegisterTextureRef(ref String path, int timeToKill)
    {
        lock(RegQueue) RegQueue.Enqueue((path, timeToKill));
        lock(lockOperate) operate.Set();
    }

    public static void UnregisterTextureRef(ref String path)
    {
        lock(DeregQueue) DeregQueue.Enqueue(path);
        lock(lockOperate) operate.Set();
    }

    ///<summary>
    ///Registers a reference to a texture internally,
    ///starts loading it in a thread if said texture
    ///doesn't already exist in memory.
    ///</summary>
    private static void RegRefInternal(String path, int timeToKill)
    {
        TextureHolder holder;

        if (TextureDict.TryGetValue(path, out holder))
        {
            lock (holder)
            {
                holder.AddRef();
                if(holder.TimeToKillLimit < timeToKill) holder.TimeToKillLimit = timeToKill;
            }
        }
        else
        {
            lock(ToRemove)
            {
                for(int i = 0; i< ToRemove.Count; ++i)
                {
                    var curr = ToRemove[i];

                    bool samePath = false;
                    
                    lock(curr) samePath = curr.texPath == path;

                    if(samePath)
                    {
                        RegAgainInternal(curr, i);
                        return;
                    }
                }
            }

            TextureDict.Add(path, new TextureHolder(ref path, timeToKill));
        }
    }

    ///<summary>
    ///Removes a reference to a texture internally,
    ///starts removing it from memory in a thread
    ///if no references to said texture remains.
    ///</summary>
    private static void DeregRefInternal(String path)
    {
        int hash = path.GetHashCode();

        TextureHolder holder = TextureDict[path];
        bool noRef = false;

        lock (holder) noRef = holder.RemoveRef();
        if (noRef)
        {
            TextureDict.Remove(path);

            lock(ToRemove)
            {
                lock (holder) holder.TimeToKill = holder.TimeToKillLimit;
                ToRemove.Add(holder);
            }
        }
    }

    private static void RegAgainInternal(TextureHolder holder, int index)
    {
        ToRemove.RemoveAt(index);

        lock(TextureDict)
        {
            lock(holder)
            {
                holder.AddRef();
                TextureDict.Add(holder.texPath, holder);
            }
        }
    }

    ///<summary>
    ///Gets texture by its path to file,
    ///Can return null in very rare ocasions
    ///or if there's no reference to the texture.
    ///</summary>
    public static TextureHolder GetTexture(ref String path)
    {
        lock (TextureDict)
        {
            TextureHolder holder;

            if(TextureDict.TryGetValue(path, out holder))
            {
                return holder;
            }

            return null;
        }
    }

    private static void ThreadCode()
    {
        while (MainClass.Running)
        {
            operate.WaitOne();
            if(!MainClass.Running) break;
            lock(lockOperate) operate.Reset();

            lock(TextureDict)
            {
                (string path, int ttk) curr = (null, 0);
                bool hasValue = false;

                RepReg:
                hasValue = false;

                lock (RegQueue)
                {
                    if(RegQueue.Count > 0)
                    {
                        curr = RegQueue.Dequeue();
                        hasValue = true;
                    }
                }

                if(hasValue)
                {
                    RegRefInternal(curr.path, curr.ttk);

                    goto RepReg;
                }

                RepDereg:

                hasValue = false;

                string dCurr = null;

                lock(DeregQueue)
                {
                    if(DeregQueue.Count > 0)
                    {
                        dCurr = DeregQueue.Dequeue();
                        hasValue = true;
                    }
                }

                if (hasValue)
                {
                    DeregRefInternal(dCurr);

                    goto RepDereg;
                }

                lock (RegQueue) if(RegQueue.Count > 0) goto RepReg;
                lock (DeregQueue) if(DeregQueue.Count > 0) goto RepDereg;
            }
        }

        Console.WriteLine("Ended texture holder thread.");
    }

    private static void RemoveThreadCode()
    {
        int timeToSleep = 32;

        while(MainClass.Running)
        {
            Thread.Sleep(timeToSleep);

            lock(ToRemove)
            {
                for(int i = 0; i < ToRemove.Count; ++i)
                {
                    var curr = ToRemove[i];

                    lock(curr)
                    {
                        if(curr.TickKill(timeToSleep))
                        {
                            curr.Dispose();
                            ToRemove.RemoveAt(i);
                            --i;
                        }
                    }
                }
            }
        }
    }

    private static bool threadStarted = false;

    public static void StartThread()
    {
        if(threadStarted) return;

        Thread thread = new Thread(ThreadCode);
        Thread thread2 = new Thread(RemoveThreadCode);
        thread.Start();
        thread2.Start();

        threadStarted = true;
    }

    public static void Close()
    {
        if(threadStarted)
        {
            operate.Set();
        }
    }
}