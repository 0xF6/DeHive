using DeHive.Abstractions;

namespace DeHive.Creation;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public struct HiveWriter(Stream stream)
{
    public unsafe void WriteNumeric<T>(T value) where T : struct, INumber<T>
    {
        Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<T>()];
        MemoryMarshal.Write(bytes, in value);
        stream.Write(bytes);
    }

    public void Write<T>(ref readonly T value) where T : struct, IHiveSerialization<T>
        => T.OnSerialize(ref this, in value);

    public unsafe void WriteRaw<T>(T* t, uint size) where T : unmanaged
    {
        var span = new Span<byte>((byte*)t, (int)(size * Unsafe.SizeOf<T>()));
        stream.Write(span);
    }

    public unsafe void WriteRaw(string str, Encoding encoding)
    {
        var size = encoding.GetByteCount(str);
        Span<byte> span = stackalloc byte[size];
        encoding.GetBytes(str, span);
        stream.Write(span);
    }

    public void WriteStruct<T>(T value) where T : struct
    {
        Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<T>()];
        MemoryMarshal.Write(bytes, in value);
        stream.Write(bytes);
    }

    public void WriteId(Ulid id)
    {
        Span<byte> span = stackalloc byte[16];
        Debug.Assert(id.TryWriteBytes(span));
        stream.Write(span);
    }

    public void WriteString(string str) => WriteString(str, Encoding.UTF8);
    public void WriteASCIIString(string str) => WriteString(str, Encoding.ASCII);

    public void WriteString(string str, Encoding encoding)
    {
        var len = encoding.GetByteCount(str);
        Span<byte> span = stackalloc byte[sizeof(int) + len];
        encoding.GetBytes(str, span.Slice(sizeof(int), span.Length - sizeof(int)));
        MemoryMarshal.Write(span.Slice(0, sizeof(int)), in len);
        stream.Write(span);
    }

}