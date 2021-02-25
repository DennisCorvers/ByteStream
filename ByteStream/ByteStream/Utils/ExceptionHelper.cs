using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Utils
{
    internal static class ExceptionHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowReadBufferExceeded()
        {
            throw new InvalidOperationException("Read operation exceeds buffer size.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowFixedBufferExceeded()
        {
            throw new InvalidOperationException("Buffer is set to a fixed size and cannot resize automatically.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowWriteBufferExceeded()
        {
            throw new InvalidOperationException("Write operation exceeds buffer size.");
        }
    }
}
