using System;
using SFML.Audio;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

public class AudioPlay
{
    //note to self any action on this should lock it
    AudioMaster AMaster;

    public AudioPlay (string path, AudioType type)
    {
        AMaster = new AudioMaster(path, type);

        CreateThreadRequest(AudioRequestType.Register, AMaster);

        if(ThreadInactive) StartThread();
    }

    public void Play()
    {
        CreateThreadRequest(AudioRequestType.Play, AMaster);
    }

    bool Disposed = false;

    public void Dispose ()
    {
        if(Disposed) return;
        Disposed = true;

        lock(AMaster)
        {
            CreateThreadRequest(AudioRequestType.Remove, AMaster);
        }
    }

    private void CreateThreadRequest (AudioRequestType reqType, AudioMaster AMaster)
    {
        lock (ThreadRequests) ThreadRequests.Enqueue(new KeyValuePair<AudioRequestType, AudioMaster>(reqType, AMaster));
    }
    #region Thread

    static bool ThreadInactive = true;

    static bool ProgramAlive = true;
    static Queue<KeyValuePair<AudioRequestType, AudioMaster>> ThreadRequests = new Queue<KeyValuePair<AudioRequestType, AudioMaster>>(50);

    static void StartThread()
    {
        ThreadInactive = false;

        Engine.ExecuteOnCloseProgram(OnProgramEnd);

        Thread thread = new Thread(ThreadCode);

        thread.Start();
    }

    static void ThreadCode ()
    {
        int sleepMS = 16;

        //List of audio buffers in memory.
        GDictionaryOrdered<string, SBufferReference> TheList = new GDictionaryOrdered<string, SBufferReference>();

        try
        {
        while(ProgramAlive)
        {
            int requestsCount = 0;

            lock (ThreadRequests) requestsCount = ThreadRequests.Count;

            if(requestsCount == 0)
            {
                Thread.Sleep(sleepMS);
                continue;
            }

            while(requestsCount > 0)
            {
                --requestsCount;
                
                KeyValuePair<AudioRequestType, AudioMaster> request = default(KeyValuePair<AudioRequestType, AudioMaster>);

                lock(ThreadRequests) request = ThreadRequests.Dequeue();

                AudioRequestType requestType = request.Key;

                AudioMaster master = request.Value;

                //Very ugly else if chain because i'm too scared to use
                //switch and lose the ability to break the loop
                //even though i will probably never use it.

                //If request type is register.
                if(requestType == AudioRequestType.Register)
                {
                    string path = "";

                    AudioType type = AudioType.Sound;

                    lock(master)
                    {
                        path = master.Path;

                        type = master.Type;
                    }

                    if(type == AudioType.Sound)
                    {
                        SBufferReference bufferRef;

                        if(TheList.ContainsKey(path))
                        {
                            bufferRef = TheList[path];
                        }
                        else
                        {
                            bufferRef = new SBufferReference(new SoundBuffer(path));
                            TheList.Add(path, bufferRef);
                        }

                        bufferRef.RefCount += 1;

                        Sound sound = new Sound(bufferRef.Buffer);

                        lock(master)
                        {
                            master.SetSound(sound);
                            master.Loaded = true;
                        }
                    }
                    else if(type == AudioType.Music)
                    {
                        Music mus = new Music(path);

                        lock(master)
                        {
                            master.SetMusic(mus);
                            master.Loaded = true;
                        }
                    }
                }
                //If request type is remove.
                else if(requestType == AudioRequestType.Remove)
                {
                    AudioType type = AudioType.Sound;

                    string path = "";

                    lock(master)
                    {
                        type = master.Type;
                        path = master.Path;
                    }

                    if(type == AudioType.Sound)
                    {
                        lock(master) master.Dispose();

                        SBufferReference bufferRef = TheList[path];

                        bufferRef.RefCount -= 1;

                        if(bufferRef.RefCount <= 0)
                        {
                            bufferRef.Buffer.Dispose();
                            TheList.Remove(path);
                        }
                    }
                    else if(type == AudioType.Music)
                    {
                        lock(master) master.Dispose();
                    }
                }
                else if(requestType == AudioRequestType.Play)
                {
                    lock(master)
                    {
                        AudioType type = master.Type;
                        if(type == AudioType.Sound)
                        {
                            master.AudioSound.Play();
                        }
                        else if(type == AudioType.Music)
                        {
                            master.AudioMusic.Play();
                        }
                    }
                }
            }
        }
        }
        catch (Exception e)
        {
            Console.WriteLine("EXCEPTION ON AUDIO THREAD!" + "\n" + e.Message + e.StackTrace);
        }
    }

    static void OnProgramEnd()
    {
        ProgramAlive = false;
    }

    #endregion

    private class AudioMaster
    {
        public bool Loaded = false;

        public readonly AudioType Type;

        public string Path;

        public Music AudioMusic = null;

        public Sound AudioSound = null;

        public AudioMaster (string path, AudioType type)
        {
            Path = path;
            Type = type;
        }

        public void SetMusic(Music music) => AudioMusic = music;

        public void SetSound(Sound sound) => AudioSound = sound;

        public void Dispose()
        {
            if(Type == AudioType.Music) AudioMusic.Dispose();
            else if(Type == AudioType.Sound) AudioSound.Dispose();
        }
    }

    private class SBufferReference
    {
        public int RefCount = 0;

        public SoundBuffer Buffer;

        public SBufferReference (SoundBuffer buffer)
        {
            Buffer = buffer;
        }
    }

    public enum AudioType
    {
        Sound = 0,
        Music = 1
    }

    private enum AudioRequestType
    {
        Register,
        Remove,
        Play
    }
}