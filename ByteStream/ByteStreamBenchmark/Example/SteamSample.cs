using ByteStream.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ByteStreamBenchmark.Example
{
    public class SteamSample
    {

    }



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

    public class PlayerInventory
    {
        public void Serialize(IByteStream stream) { }
    }

    internal static class Extension
    {
        public static void Serialize(this IByteStream stream, ref Vector3 vector)
        {
            stream.Serialize(ref vector.X);
            stream.Serialize(ref vector.Y);
            stream.Serialize(ref vector.Z);
        }
    }
}
