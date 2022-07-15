using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SFML.Graphics;
using SFML.System;

public class TextureHolder
{
    private static GDictionary<string, TextureHolder> TextureDict = new GDictionary<string, TextureHolder>();

    public Texture texture;

    int refCount = 0;

    bool disposed = false;

    static ManualResetEvent operate = new ManualResetEvent(false);

    static Object lockOperate = new Object();

    static Queue<string> RegQueue = new Queue<string>();

    static Queue<string> DeregQueue = new Queue<string>();

    private TextureHolder(ref String path)
    {
        try
        {
            texture = new Texture(path);
        }
        catch(Exception e)
        {
            Console.WriteLine($"Couldn't load texture \"{path}\", error message: {e.Message}");
        }

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

    void Dispose()
    {
        if (disposed) return;

        texture.Dispose();

        disposed = true;
    }

    public static void RegisterTextureRef(ref String path)
    {
        lock(RegQueue) RegQueue.Enqueue(path);
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
    private static void RegRefInternal(String path)
    {
        TextureHolder holder;

        if (TextureDict.TryGetValue(path, out holder))
        {
            lock (holder) holder.AddRef();
        }
        else
        {
            TextureDict.Add(path, new TextureHolder(ref path));
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
        lock (holder)
        {
            if (holder.RemoveRef())
            {
                holder.Dispose();

                TextureDict.Remove(path);
            }
        }
    }

    ///<summary>
    ///Gets texture by its path to file,
    ///Can return null in very rare ocasions
    ///or if there's no reference to the texture.
    ///</summary>
    public static Texture GetTexture(ref String path)
    {
        lock (TextureDict)
        {
            TextureHolder holder;

            if(TextureDict.TryGetValue(path, out holder))
            {
                lock (holder) return holder.texture;
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
                string curr = null;
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
                    RegRefInternal(curr);

                    goto RepReg;
                }

                RepDereg:

                hasValue = false;

                lock(DeregQueue)
                {
                    if(DeregQueue.Count > 0)
                    {
                        curr = DeregQueue.Dequeue();
                        hasValue = true;
                    }
                }

                if (hasValue)
                {
                    DeregRefInternal(curr);

                    goto RepDereg;
                }

                lock (RegQueue) if(RegQueue.Count > 0) goto RepReg;
                lock (DeregQueue) if(DeregQueue.Count > 0) goto RepDereg;
            }
        }

        Console.WriteLine("Ended texture holder thread.");
    }

    private static bool threadStarted = false;

    public static void StartThread()
    {
        if(threadStarted) return;

        Thread thread = new Thread(ThreadCode);
        thread.Start();

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