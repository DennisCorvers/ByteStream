# ByteStream
A blazing fast byte (de)serializer for C#

Available on Nuget via ```Install-Package ByteStream```

## What is ByteStream?
Bytestream is a small library that enables blazing fast serialization of a collection of types to raw bytes. Either in the form of a byte array `byte[]` or unmanaged memory `IntPtr`.
The library performs no memory allocation on its own, so you are free to use your own memory allocator!

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

*User-defined structs may change their layout when used on another system. Don't serialize user-defined structs unless you are absolutely certain the layout will be the same everywhere it is read back.*

## Drawbacks
- The ByteWriter and ByteReader makes heavy use of memory pinning. This can make the unmanaged variant preferable when possible.
- The writers and readers always make use of pointer conversion. In the case of writing single-byte values to a `byte[]` buffer, consider directly writing these values to the buffer instead.

# Technical specifications:

## Comparison to other methods:
Writing of 1024 integer (4-byte) values to a buffer.

|       Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------- |----------:|----------:|----------:|------:|--------:|
|   ByteWriter |  2.174 us | 0.0277 us | 0.0259 us |  0.36 |    0.00 |
|    PtrWriter |  2.529 us | 0.0538 us | 0.0503 us |  0.42 |    0.01 |
| ArraySegment |  6.018 us | 0.0226 us | 0.0211 us |  1.00 |    0.00 |
| BitConverter | 16.774 us | 0.5962 us | 0.6122 us |  2.79 |    0.11 |
|        Union | 11.818 us | 0.0375 us | 0.0332 us |  1.96 |    0.01 |

**Note that the BitConverter generates garbage on every write operation!**

Reading of 1024 integer (4-byte) values from a buffer.

|       Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------- |----------:|----------:|----------:|------:|--------:|
|   ByteWriter |  2.641 us | 0.0161 us | 0.0150 us |  0.40 |    0.00 |
|    PtrWriter |  2.338 us | 0.0193 us | 0.0171 us |  0.35 |    0.00 |
| ArraySegment |  6.663 us | 0.0531 us | 0.0497 us |  1.00 |    0.00 |
| BitConverter |  2.048 us | 0.0941 us | 0.1120 us |  0.31 |    0.02 |
|        Union | 12.960 us | 0.0463 us | 0.0433 us |  1.95 |    0.02 |

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
