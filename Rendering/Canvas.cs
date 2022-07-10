using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
//using System.Reflection;
using System.Linq;
using SFML.Window;
using System.Threading;
using SFML.Graphics;
using SFML.System;


public class Canvas
{

    private RenderWindow Window;

    ManualResetEvent AllowRendering = new ManualResetEvent(false);

    public int FPS = 0;

    // wether the window is close or not
    public bool IsClosed = false;

    //Wether or not this window is in the rendering process.
    public bool Updating = false;

    //This buffer stores the keys that are currently pressed.
    List <int> KeysPressed = new List<int>();

    //The list below is the buffer of things the screen needs to draw next in a _Render call.
    private DrawableObject[] ToDraw = new DrawableObject[0];

    //The actual lenght of the ToDraw array
    int ToDrawCount = 0;

    bool ToDrawOrganized = false;

    double Lerp = 1.0;

    string Status = "OPEN";

    Vector2u SIZE;
    string NAME;

    public Canvas (uint width, uint height, string name)
    {
        //Size = new Size(width, height);
        //Text = name;

        SIZE = new Vector2u(width, height);
        NAME = name;

        /*Window = new RenderWindow(new VideoMode(WIDTH, HEIGHT), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        Window.SetActive(true);

        Window.RequestFocus();*/

        TextureHolder.StartThread();

        Thread runThread = new Thread(Run);
        runThread.Priority = ThreadPriority.AboveNormal;
        runThread.Start();
    }

    void _Render ()
    {
        Updating = true;

        Stopwatch performanceData = null;
        if (FPS < 2 && !ToDrawOrganized)
        {
            performanceData = Stopwatch.StartNew();
        }

        Window.Clear(Color.Black);

        //Draws next frame
        DrawableObject[] toDraw;

        long organizeTime = 0;
        if (performanceData != null) organizeTime = performanceData.ElapsedTicks;

        lock (ToDraw)
        {

            //if (!ToDrawOrganized)
            //{
                toDraw = Organize(ToDraw, ToDrawCount);
            //}
            //else
            //{
            //    toDraw = new DrawableObject[ToDrawCount];
            //    Array.Copy(ToDraw, toDraw, ToDrawCount);
            //}
        }
        if (performanceData != null) organizeTime = performanceData.ElapsedTicks - organizeTime;

        long drawTime = 0;
        if (performanceData != null) drawTime = performanceData.ElapsedTicks;

        Vector2f wSize = (Vector2f)SIZE;

        float fLerp = (float) Lerp;

        bool optimizing = false;

        uint optIndex = 0;

        uint optCount = 0;

        DrawableObject optObject = null;

        uint tdLenght = (uint)toDraw.Length;

        RenderArgs args = new RenderArgs()
        {
            w = Window,
            lerp = fLerp,
            windowSize = wSize
        };

        for (uint i = 0; i< tdLenght; ++i)
        {
            DrawableObject tdr = toDraw[i];

            if (i + 1 == tdLenght)
            {
                tdr.Draw(args);

                if(optimizing)
                {
                    if(optObject.Optimizable(tdr)) ++optCount;
                    else tdr.Draw(args);

                    optObject.DrawOptimizables(args, toDraw,  optIndex, optCount);
                }
                else tdr.Draw(args);

                continue;
            }

            if (optimizing)
            {
                if(optObject.Optimizable(tdr))
                {
                    ++optCount;
                }
                else
                {
                    tdr.Draw(args);

                    optObject.DrawOptimizables(args, toDraw,  optIndex, optCount);

                    optimizing = false;
                }
            }
            else
            {
                if (tdr.Optimizable(toDraw[i + 1]))
                {
                    optIndex = i;
                    optCount = 1;
                    optimizing = true;

                    optObject = tdr;
                }
            }

            tdr.Draw(args);
        }
        

        Window.DispatchEvents();
        Window.Display();

        if (performanceData != null) drawTime = performanceData.ElapsedTicks - drawTime;

        //if(!ToDrawOrganized)
        //{
        //    ToDrawOrganized = true;

        //    ToDraw = toDraw;
        //}

        if (performanceData != null)
        {
            double ticksPassed = performanceData.ElapsedTicks;

            if (ticksPassed == 0) Console.WriteLine("Rendering took no time at all.");
            else
            {
                double MSPassed = (ticksPassed / (double) Stopwatch.Frequency) * 1000;

                long performance = (long)(1000d / MSPassed);

                Console.WriteLine($"Rendering took {MSPassed}MS, it could be executed {performance} times per second!");
                double dr = drawTime== 0.0 ? 0.0 : (double) drawTime / ticksPassed;
                double ot = organizeTime == 0.0 ? 0.0 : (double) organizeTime / ticksPassed;
                Console.WriteLine($"Draw time: {dr *100.0}%, OrganizeTime: {ot * 100.0}%");

            }

            performanceData.Stop();
        }
        
        ++FPS;
        Updating = false;
    }

    //Method that takes part in organizing the list of render objects
    private DrawableObject[] Organize(DrawableObject[] drawable, int size)
    {
        DrawableObject[] td = new DrawableObject[ToDrawCount];
        Array.Copy(ToDraw, td, size);

        return (from p in td
        orderby p.z
        select p).ToArray<DrawableObject>();
    }

    public int[] GetKeys ()
    {
        
        lock (KeysPressed)
        {
            return KeysPressed.ToArray();
        }
    }

    public void SetDraw (DrawableObject[] objs, int count)
    {
        //set objects to draw this frame
        lock (ToDraw)
        {
            if (count <= ToDraw.Length)
            {
                for(int i = 0; i< count; ++i)
                {
                    ToDraw[i] = objs[i];
                }

                //if(count > ToDraw.Length)
                //{
                //    for (int i = count; i < ToDraw.Length; ++i)
                //    {
                //        ToDraw[i] = null;
                //    }
                //}
            }
            else ToDraw = objs;

            ToDrawOrganized = false;
            ToDrawCount = count;
        }
    }

    public void SetLerp (double lerp)
    {
        Lerp = lerp;
    }

    void _FormClosed(object sender, EventArgs e)
    {
        Close();
    }

    void _KeyDown (object sender, KeyEventArgs e)
    {
        lock (KeysPressed) if (!KeysPressed.Contains((int) e.Code)) KeysPressed.Add((int) e.Code);
    }

    void _KeyUp (object sender, KeyEventArgs e)
    {
        
        lock (KeysPressed) if (KeysPressed.Contains((int) e.Code)) KeysPressed.Remove((int) e.Code);
        
    }

    void _LostFocus (object sender, System.EventArgs e)
    {
        lock (KeysPressed) KeysPressed.Clear();
    }

    public void Refresh ()
    {
        //_Render();

        AllowRendering.Set();
    }

    public void Run()
    {
        Window = new RenderWindow(new VideoMode(SIZE.X, SIZE.Y), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        Window.SetVerticalSyncEnabled(true);

        Window.RequestFocus();

        while(!IsClosed)
        {

            AllowRendering.WaitOne();

            lock(Status) if(Status == "CLOSED") break;

            if (Window.IsOpen) _Render();
            else break;
 
            lock(Status) if(Status == "CLOSED") break;

            AllowRendering.Reset();
        }

        IsClosed = true;

        lock(Status) Status = "CLOSED";

        Console.WriteLine("Ended window thread.");
    }

    public void Close()
    {
        Window.Close();
        IsClosed = true;

        lock (Status) Status = "CLOSED";
        AllowRendering.Set();
    }
}

public class TextureHolder
{
    private static Dictionary<int, TextureHolder> TextureDict = new System.Collections.Generic.Dictionary<int, TextureHolder>();

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

    private static void RegRefInternal(String path)
    {
        TextureHolder holder;

        if (TextureDict.TryGetValue(path.GetHashCode(), out holder))
        {
            lock (holder) holder.AddRef();
        }
        else
        {
            TextureDict.Add(path.GetHashCode(), new TextureHolder(ref path));
        }
    }

    private static void DeregRefInternal(String path)
    {
        int hash = path.GetHashCode();

        TextureHolder holder = TextureDict[hash];
        lock (holder)
        {
            if (holder.RemoveRef())
            {
                holder.Dispose();

                TextureDict.Remove(hash);
            }
        }
    }

    public static Texture GetTexture(ref String path)
    {
        lock (TextureDict)
        {
            TextureHolder holder;

            if(TextureDict.TryGetValue(path.GetHashCode(), out holder))
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

public class RenderArgs
{
    public RenderWindow w;

    public float lerp;

    public Vector2f windowSize;
}

//Old useless code for a windows forms problem.
/*internal static class NativeWinAPI
{
    internal static readonly int GWL_EXSTYLE = -20;
    internal static readonly int WS_EX_COMPOSITED = 0x02000000;

    [DllImport("user32")]
    internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32")]
    internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}*/