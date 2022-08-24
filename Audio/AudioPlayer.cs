using System;
using SFML.Audio;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

public class AudioPlayer
{
    static float _MasterVolumeSound;
    public static float MasterVolumeSound
    {
        get => _MasterVolumeSound;
        set
        {
            _MasterVolumeSound = value;
            lock (ThreadRequests) ThreadRequests.Enqueue(new KeyValuePair<AudioRequestType, AudioMaster>(AudioRequestType.SetMasterVolume, null));
        }
    }

    static float _MasterVolumeMusic;
    public static float MasterVolumeMusic
    {
        get => _MasterVolumeMusic;
        set
        {
            _MasterVolumeMusic = value;
            lock (ThreadRequests) ThreadRequests.Enqueue(new KeyValuePair<AudioRequestType, AudioMaster>(AudioRequestType.SetMasterVolume, null));
        }
    }

    //note to self any action on AMaster should lock it
    AudioMaster AMaster;

    public bool Loaded
    {
        get
        {
            lock(AMaster) return AMaster.Loaded;
        }
    }

    public float Volume
    {
        get
        {
            lock (AMaster)
            {
                return AMaster.Volume;
            }
        }

        set
        {
            lock (AMaster)
            {
                AMaster.Volume = value;
            }

            CreateThreadRequest(AudioRequestType.SetVolume, AMaster);
        }
    }

    public float Pitch
    {
        get
        {
            lock(AMaster)
            {
                return AMaster.Pitch;
            }
        }
        set
        {
            lock(AMaster)
            {
                AMaster.Pitch = value;
            }
            CreateThreadRequest(AudioRequestType.SetPitch, AMaster);
        }
    }

    public int TimeMS
    {
        get
        {
            lock(AMaster)
            {
                if(!AMaster.Loaded) return 0;

                AudioType type = AMaster.Type;

                if(type == AudioType.Sound)
                {
                    return AMaster.AudioSound.PlayingOffset.AsMilliseconds();
                }

                return AMaster.AudioMusic.PlayingOffset.AsMilliseconds();
            }
        }
        set
        {
            lock(AMaster)
            {
                AMaster.TimeMS = value;
            }
            CreateThreadRequest(AudioRequestType.SetTime, AMaster);
        }
    }

    public int LenghtMS
    {
        get
        {
            lock(AMaster)
            {
                return AMaster.LenghtMS;
            }
        }
    }

    public AudioPlayer (string path, AudioType type)
    {
        AMaster = new AudioMaster(path, type);

        CreateThreadRequest(AudioRequestType.Register, AMaster);

        if(ThreadInactive) StartThread();
    }

    public void Play()
    {
        CreateThreadRequest(AudioRequestType.Play, AMaster);
    }

    public void Pause()
    {
        CreateThreadRequest(AudioRequestType.Pause, AMaster);
    }

    public void Stop()
    {
        CreateThreadRequest(AudioRequestType.Stop, AMaster);
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

        //List of AudioMaster
        List<AudioMaster> AMasterList = new List<AudioMaster>(100);

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

                AudioType type = AudioType.Sound;

                lock(master)
                {
                    if(master != null) type = master.Type;
                }

                //Very ugly else if chain because i'm too scared to use
                //switch and lose the ability to break the loop
                //even though i will probably never use it.

                //If request type is register.
                if(requestType == AudioRequestType.Register)
                {
                    string path = "";

                    lock(master)
                    {
                        path = master.Path;
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
                            master.LenghtMS = bufferRef.Buffer.Duration.AsMilliseconds();

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

                    AMasterList.Add(master);
                }
                //If request type is remove.
                else if(requestType == AudioRequestType.Remove)
                {
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

                    AMasterList.Remove(master);
                }
                else if (requestType == AudioRequestType.Play)
                {
                    lock(master)
                    {
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
                else if (requestType == AudioRequestType.Pause)
                {
                    lock(master)
                    {
                        if(type == AudioType.Sound)
                        {
                            master.AudioSound.Pause();
                        }
                        else if(type == AudioType.Music)
                        {
                            master.AudioMusic.Pause();
                        }
                    }
                }
                else if (requestType == AudioRequestType.Stop)
                {
                    lock(master)
                    {
                        if(type == AudioType.Sound)
                        {
                            master.AudioSound.Stop();
                        }
                        else if(type == AudioType.Music)
                        {
                            master.AudioMusic.Stop();
                        }
                    }
                }
                else if (requestType == AudioRequestType.SetPitch)
                {
                    lock(master)
                    {
                        if(type == AudioType.Sound)
                        {
                            master.AudioSound.Pitch = master.Pitch;
                        }
                        else if(type == AudioType.Music)
                        {
                            master.AudioMusic.Pitch = master.Pitch;
                        }
                    }
                }
                else if (requestType == AudioRequestType.SetVolume)
                {
                    lock(master)
                    {
                        if(type == AudioType.Sound)
                        {
                            master.AudioSound.Volume = master.Volume * _MasterVolumeSound;
                        }
                        else if(type == AudioType.Music)
                        {
                            master.AudioSound.Volume = master.Volume * _MasterVolumeMusic;
                        }
                    }
                }
                else if(requestType == AudioRequestType.SetTime)
                {
                    lock(master)
                    {
                        if(type == AudioType.Sound)
                        {
                            master.AudioSound.PlayingOffset = SFML.System.Time.FromMicroseconds(master.TimeMS);
                        }
                        else if(type == AudioType.Music)
                        {
                            master.AudioSound.PlayingOffset = SFML.System.Time.FromMicroseconds(master.TimeMS);
                        }
                    }
                }
                else if(requestType == AudioRequestType.SetMasterVolume)
                {
                    int AMCount = AMasterList.Count;
                    for(int i = 0; i < AMCount; ++i)
                    {
                        var curr = AMasterList[i];

                        lock(curr)
                        {
                            if(type == AudioType.Sound)
                            {
                                curr.AudioSound.Volume = curr.Volume * _MasterVolumeSound;
                            }
                            else if(type == AudioType.Music)
                            {
                                curr.AudioSound.Volume = curr.Volume * _MasterVolumeMusic;
                            }
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

        public float Pitch = 1.0f;

        public float Volume = 1.0f;

        public int TimeMS = 0;

        public int LenghtMS = 0;

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
        Play,
        SetPitch,
        SetVolume,
        Pause,
        Stop,
        SetTime,
        SetMasterVolume,
    }
}