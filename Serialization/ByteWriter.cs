using System.Collections.Generic;
using System;
using System.Text;

public class ByteWriter: IDisposable
{
    List<byte> Data;

    bool _disposed = false;

    public ByteWriter ()
    {
        Data = new List<byte>();
    }

    public ByteWriter(byte[] bytes)
    {
        Data = new List<byte> (bytes);
    }

    public void WriteInt32 (Int32 value)
    {
        Data.AddRange(BitConverter.GetBytes(value));
    }

    public void WriteInt64 (Int64 value)
    {
        Data.AddRange(BitConverter.GetBytes(value));
    }

    public void WriteBool (bool value)
    {
        Data.Add(value? (byte)1 : (byte)0);
    }

    public void WriteByte (byte value)
    {
        Data.Add(value);
    }

    public void WriteString (string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);

        Data.AddRange(BitConverter.GetBytes(bytes.Length));
        Data.AddRange(bytes);
    }

    public void WriteFloat (float value)
    {
        Data.AddRange(BitConverter.GetBytes(value));
    }

    public void WriteDouble (double value)
    {
        Data.AddRange(BitConverter.GetBytes(value));
    }

    public byte[] ToArray ()
    {
        return Data.ToArray();
    }

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