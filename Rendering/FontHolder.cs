using System.Collections.Specialized;
using System.Collections.Generic;
using SFML.Graphics;
using System.Threading;
public static class FontHolder
{
    //Fuck, i want to play that ezgamestation game so bad, i heard of it and it sounds amazing.
    //gotta finish this though...

    #region Initialization

    static FontHolder ()
    {
        StartThread();
    }

    static void StartThread()
    {
        Thread thread = new Thread(ThreadCode);
        thread.Start();

        Engine.ExecuteOnCloseProgram(KillThis);
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Says to a thread "load font if nonexistent or i will murder you" kindly.
    /// </summary>
    public static void RegisterReference(string pathToFont)
    {
        CreateRequest(FontRequest.Register, new FontReqValue(pathToFont));
    }

    /// <summary>
    /// Says to the thread a particular object is no longer
    /// using this font.
    /// </summary>
    public static void UnregisterReference(string pathToFont)
    {
        CreateRequest(FontRequest.Unregister, new FontReqValue(pathToFont));
    }

    /// <summary>
    /// Gets cashed font, can return null if the font is still loading.
    /// </summary>
    public static FontRef GetFont(string pathToFont)
    {
        lock(FontCashe)
        {
            FontRef fRef;
            if(FontCashe.TryGetValue(pathToFont, out fRef))
            {
                return fRef;
            }
        }

        return null;
    }

    #endregion

    #region Actual thread stuff

    static GDictionaryOrdered<string, FontRef> FontCashe = new GDictionaryOrdered<string, FontRef>();    

    static Queue<KeyValuePair<FontRequest, FontReqValue>> ThreadRequests = new Queue<KeyValuePair<FontRequest, FontReqValue>>(50);

    static ManualResetEvent ProcessWait = new ManualResetEvent(false);

    static bool Alive = true;

    static void KillThis()
    {
        Alive = false;
        ProcessWait.Set();
    }

    static void ThreadCode()
    {
        while(Alive)
        {
            int reqCount = 0;
            lock(ThreadRequests) reqCount = ThreadRequests.Count;

            if(reqCount == 0)
            {
                if(!Alive) break;
                ProcessWait.WaitOne();
                ProcessWait.Reset();
            }

            if(!Alive) break;

            for(int i = 0; i< reqCount; ++i)
            {
                KeyValuePair<FontRequest, FontReqValue> tReq = default(KeyValuePair<FontRequest, FontReqValue>);
                lock(ThreadRequests) tReq = ThreadRequests.Dequeue();

                var request = tReq.Key;
                var reqValue = tReq.Value;

                FontRef fRef = null;

                bool refMissing = true;

                lock(FontCashe)
                {
                    refMissing = !FontCashe.TryGetValue(reqValue.Path, out fRef);
                }

                if(refMissing)
                {
                    Font f = new Font(reqValue.Path);
                    fRef = new FontRef(f, reqValue.Path);
                    lock(FontCashe) FontCashe.Add(reqValue.Path, fRef);
                }

                switch(request)
                {
                    case FontRequest.Register:
                    lock(fRef) fRef.RefCount += 1;
                    break;
                    case FontRequest.Unregister:
                    lock(fRef)
                    {
                        fRef.RefCount -= 1;
                        if(fRef.RefCount <= 0)
                        {
                            fRef.ActualFont.Dispose();
                            fRef.Disposed = true;
                            lock(FontCashe) FontCashe.Remove(fRef.Path);
                        }
                    }
                    break;
                }
            }
        }
    }

    #endregion

    static void CreateRequest(FontRequest request, FontReqValue value)
    {
        lock(ThreadRequests)
        ThreadRequests.Enqueue(new KeyValuePair<FontRequest, FontReqValue>(request, value));

        ProcessWait.Set();
    }

    public class FontRef
    {
        public Font ActualFont;
        public int RefCount = 0;
        public string Path;

        public bool Disposed = false;

        public FontRef(Font font, string path)
        {
            ActualFont = font;
            Path = path;
        }
    }

    private class FontReqValue
    {
        public string Path;

        public FontReqValue(string path)
        {
            Path = path;
        }
    }

    private enum FontRequest
    {
        Register,
        Unregister
    }
}