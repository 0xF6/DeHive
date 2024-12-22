namespace DeHive.Abstractions;

using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public unsafe struct HiveReader(BinaryReader accessor)
{
    /*static unsafe Stream CreateStreamFromAccessor(MemoryMappedViewAccessor accessor, long offset, long length)
       {
           var pointer = accessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + offset;
           return new UnmanagedMemoryStream((byte*)pointer, length, length, FileAccess.ReadWrite);
       }*/


    public T ReadNumeric<T>() where T : struct, INumber<T> => ReadStruct<T>();

    public T Read<T>() where T : struct, IHiveSerialization<T>
    {
        T t = default;
        T.OnDeserialize(ref this, ref t);
        return t;
    }

    public unsafe string ReadRaw(int size, Encoding encoding)
    {
        Span<byte> span = stackalloc byte[size];
        accessor.Read(span);
        return encoding.GetString(span);
    }

    public T ReadStruct<T>() where T : struct
    {
        Span<byte> span = stackalloc byte[Unsafe.SizeOf<T>()];
        Debug.Assert(accessor.Read(span) != 0);
        return MemoryMarshal.Read<T>(span);
    }

    public Ulid ReadId()
    {
        Span<byte> span = stackalloc byte[16];
        Debug.Assert(accessor.Read(span) != 0);
        return new Ulid(span);
    }

    public unsafe string ReadString(Encoding encoding)
    {
        var len = accessor.ReadInt32();

        Span<byte> bytes = stackalloc byte[len];

        Debug.Assert(accessor.Read(bytes) != 0);

        return encoding.GetString(bytes);
    }


    public string ReadString() => ReadString(Encoding.UTF8);
    public string WriteASCIIString() => ReadString(Encoding.ASCII);
}