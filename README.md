# ByteStream
A blazing fast byte (de)serializer for C#. 

Available on NuGet via ```Install-Package ByteStream```

## What is ByteStream?
Bytestream is a small library that enables blazing fast serialization of a collection of types to raw bytes. Either in the form of a byte array `byte[]` or unmanaged memory `IntPtr`.
The library performs no memory allocation on its own, so you are free to use your own memory allocator!

ByteStream is written in NetStandard2.0 making it compatible with .Net, .Net Core and Unity3D among things!

*__Carefully read the Usage section__ for a brief introduction on how to use the library.*

## What does ByteStream offer?
Bytestream offers 2 sets of serializers and deserializers, called writers and readers. They are solely used for manual serialization and deserialization of the supported types.
- Easy to understand, simple syntax.
- Lightweight, no memory allocation.
- Extremely fast, using pointer conversion to read and write data.
- Uses memory copy to quickly copy large blocks of memory to and from the buffers.
- Comes in a managed and unmanaged variant.
- Auto-resizable, managed buffer (optional).
- Automatically keeps track of offsets and buffer boundaries.
- Prefixes strings and memory blocks with a 2-byte length (optional).

## Supported types:
- All types that adhere to the [unmanaged constraint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types) can be written to, and read from the buffers.
- Byte arrays
- ANSI strings (1-byte per character)
- UTF16 strings (2-byte per character, default in C#)
- Any string that's supported by .NET's Encoding class.

*User-defined structs may change their layout when used on another system. Don't serialize user-defined structs unless you are absolutely certain the layout will be the same everywhere it is read back.*

## Drawbacks
- The ByteWriter and ByteReader makes heavy use of memory pinning. This can make the unmanaged variant preferable when possible.
- The writers and readers always make use of pointer conversion. In the case of writing single-byte values to a `byte[]` buffer, consider directly writing these values to the buffer instead.

# Technical specifications:

## Comparison to other methods:
Writing of 1024 integer (4-byte) values to a buffer.

|          Method |      Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|
|       ByteWrite |  2.202 us | 0.0035 us | 0.0033 us |  0.57 |    0.00 |      - |         - |
|        PtrWrite |  1.774 us | 0.0130 us | 0.0122 us |  0.46 |    0.00 |      - |         - |
|   ManagedStream |  2.134 us | 0.0022 us | 0.0020 us |  0.55 |    0.00 |      - |         - |
| UnmanagedStream |  1.757 us | 0.0015 us | 0.0014 us |  0.45 |    0.00 |      - |         - |
|    ArraySegment |  3.888 us | 0.0081 us | 0.0072 us |  1.00 |    0.00 |      - |         - |
|      BitConvert | 10.996 us | 0.1466 us | 0.1371 us |  2.83 |    0.04 | 5.1880 |   32768 B |
|           Union |  9.191 us | 0.0105 us | 0.0098 us |  2.36 |    0.00 |      - |         - |


Reading of 1024 integer (4-byte) values from a buffer.

|          Method |     Mean |     Error |    StdDev | Ratio | Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------- |---------:|----------:|----------:|------:|------:|------:|------:|----------:|
|       ByteWrite | 1.834 us | 0.0014 us | 0.0013 us |  0.49 |     - |     - |     - |         - |
|        PtrWrite | 1.764 us | 0.0065 us | 0.0060 us |  0.47 |     - |     - |     - |         - |
|   ManagedStream | 2.028 us | 0.0017 us | 0.0015 us |  0.54 |     - |     - |     - |         - |
| UnmanagedStream | 1.694 us | 0.0081 us | 0.0076 us |  0.46 |     - |     - |     - |         - |
|    ArraySegment | 3.721 us | 0.0053 us | 0.0044 us |  1.00 |     - |     - |     - |         - |
|      BitConvert | 1.556 us | 0.0112 us | 0.0105 us |  0.42 |     - |     - |     - |         - |
|           Union | 9.025 us | 0.0466 us | 0.0389 us |  2.43 |     - |     - |     - |         - |

When using ArraySegment, BitConverter and Unioning structs the user needs to keep track of the offsets themselves. Manual bitshifting may also be required to read and write values.

# Usage

Because the API is almost identical, the managed and unmanaged readers and writers function nearly identical. Be warned that when using the unmanaged reader or writer, the provided length must be no longer than the actual length of the memory block provided.

It is important that you write and read back the values in the same order to keep data consistent (as shown in the write and read example).

Because the readers and writers are value types, they must be passed along using the `ref` keyword.

**The readers and writers are not thread safe! Beware of accessing buffers concurrently from multiple threads and modifying the supplied buffers during operation!**


### Writing

Writing of values is really easy. Just wrap a buffer and you can start writing!

```C#
//Creating a simple buffer that resizes when the limit it met.
ByteWriter writer = new ByteWriter(64, false);

//Wrapping a pre-existing buffer
byte[] buffer = new byte[128];
ByteWriter writer = new ByteWriter(buffer);

//Writing an ANSI string with a prefixed length.
writer.WriteANSI("Writing an integer value...", true);
//Writing an integer value.
writer.Write(591823);

//Copying the internal buffer to a new byte array.
byte[] copy = new byte[32];
writer.CopyTo(copy);

//Gets the original buffer (is equal to the above defined "buffer")
byte[] originalBuffer = writer.Buffer;

```

### Reading

Reading values is just as easy as writing them! Again, just wrap a buffer and you can start reading.

```C#
byte[] buffer; //Some earlier defined buffer that holds data.
ByteReader reader = new ByteReader(buffer);

//Read an ANSI string (automatically grabs the prefixed size).
string stringValue = reader.ReadANSI();
//Read an integer value.
int intValue = reader.Read<int>();
```

### Defining custom types

We can create extension methods to allow serializing and deserializing of user-defined types (even classes!).
__Be sure to add the `ref` keyword to ensure the offset gets incremented!__

```C#
public static void WritePoint(ref this ByteWriter writer, Point point)
{
    writer.Write(point.X); writer.Write(point.Y);
}

public static Point ReadPoint(ref this ByteReader reader)
{
    return new Point(reader.Read<int>(), reader.Read<int>());
}

//We can then use these extension methods from anywhere else
ByteWriter writer = new ByteWriter(buffer);
writer.WritePoint(new Point(1, 2));

ByteReader reader = new ByteReader(buffer);
var point = reader.ReadPoint();
```

## Streaming classes

The ManagedSteam and the UnmanagedStream make serialization of objects even easier. Below is an example of how to serialize a class using the IByteStream interface that comes with both of the aforementioned Streams.

The Stream can simply be passed to the Serialize method. Depending if the Stream is in Write or Read mode, the object is serialized or deserialized automatically.

```C#
public class PlayerData
{
    public string Name;
    public int Health;
    public float Speed;
    public PlayerInventory Inventory;

    public void Serialize(IByteStream stream)
    {
        stream.SerializeString(ref Name, Encoding.ASCII);
        stream.Serialize(ref Health);      
        stream.Serialize(ref Speed);

        Inventory.Serialize(stream);
    }
}
```

Extending either of the Stream classes (or the IByteStream Interface) is equally as easy as using them to serialize. Below is an example of a Serialize method for a Vector3 by extending IByteStream.

```C#
internal static class Extension
{
    public static void Serialize(this IByteStream stream, ref Vector3 vector)
    {
        stream.Serialize(ref vector.X);
        stream.Serialize(ref vector.Y);
        stream.Serialize(ref vector.Z);     
    }
}
```
