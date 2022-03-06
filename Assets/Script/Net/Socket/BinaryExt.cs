using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public static class BinaryExt
{
    private static bool LittleEndian = true;
    public static void SetLittleEndian(bool little)
    {
        LittleEndian = little;
    }

    public static short ReadShort(this BinaryReader reader)
    {
        if (LittleEndian)
            return reader.ReadInt16();
        return IPAddress.HostToNetworkOrder(reader.ReadInt16());
    }

    public static int ReadInt(this BinaryReader reader)
    {
        if (LittleEndian)
            return reader.ReadInt32();
        return IPAddress.HostToNetworkOrder(reader.ReadInt32());
    }

    public static long ReadLong(this BinaryReader reader)
    {
        if (LittleEndian)
            return reader.ReadInt64();
        return IPAddress.HostToNetworkOrder(reader.ReadInt64());
    }

    public static void WriteShort(this BinaryWriter writer,short value)
    {
        if (LittleEndian)
            writer.Write(value);
        else
            writer.Write(IPAddress.HostToNetworkOrder(value));
    }

    public static void WriteInt(this BinaryWriter writer, int value)
    {
        if (LittleEndian)
            writer.Write(value);
        else
            writer.Write(IPAddress.HostToNetworkOrder(value));
    }

    public static void WriteLong(this BinaryWriter writer, long value)
    {
        if (LittleEndian)
            writer.Write(value);
        else
            writer.Write(IPAddress.HostToNetworkOrder(value));
    }
}
