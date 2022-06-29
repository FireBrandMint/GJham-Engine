
using System;
using System.Text;
public class ByteReader : IDisposable
{
    byte[] Data;

    int index = 0;

    bool _disposed = false;

    public ByteReader (byte[] bytes)
    {
        Data = bytes;
    }

    public Int32 ReadInt32 ()
    {
        var value = BitConverter.ToInt32(Data, index);
        index += 4;
        return value;
    }

    public Int64 ReadInt64 ()
    {
        var value = BitConverter.ToInt64(Data, index);
        index += 8;
        return value;
    }

    public bool ReadBool ()
    {
        var value = Data[index] == 1;
        index += 1;
        return value;
    }

    public byte ReadByte ()
    {
        var value = Data[index];
        index += 1;
        return value;
    }

    public string ReadString ()
    {
        var length = BitConverter.ToInt32(Data, index);
        index += 4;

        var value = Encoding.ASCII.GetString(Data, index, length);

        index += length;

        return value;
    }

    public FInt ReadFInt ()
    {
        Int64 value = ReadInt64();

        return FInt.Create(value, false);
    }

    public float ReadFloat ()
    {
        var value = BitConverter.ToSingle(Data, index);
        index += 4;
        return value;
    }

    public double ReadDouble ()
    {
        var value = BitConverter.ToDouble(Data, index);
        index += 8;
        return value;
    }

    public int Count () => Data.Length;

    public int BytesUnread () => Data.Length - (index + 1);

    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose (bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Data = null;
        }

        _disposed = true;
    }
}