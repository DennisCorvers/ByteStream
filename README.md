# ByteStream
A blazing fast byte (de)serializer

## What is ByteStream?
Bytestream is a small library that enables blazing fast serialization of a collection of types to raw bytes. Either in the form of a byte array `byte[]` or unmanaged memory `IntPtr`.
The library performs no memory allocation on its own, so you are free to use your own memory allocator!

## Supported types:
- All types that adhere to the [managed constraint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types) can be written to, and read from the buffers.
- Byte arrays
- ANSI strings (1-byte per character)
- UTF16 strings (2-byte per character, default in C#)

*User-defined structs may change their layout when used on another system. Don't serialize user-defined structs unless you are absolutely certain the layout will be the same everywhere it is read back.*
