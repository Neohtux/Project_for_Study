using Server_Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

public abstract class Packet
{
    public ushort p_size;
    public ushort p_id;
    public abstract ArraySegment<byte> Ser();
    public abstract void Des(ArraySegment<byte> buffer);

}
public class PlayerPacket : Packet
{
    public long player_id;
    public string message;

    private static unsafe void ToBytes(byte[] array, int offset, long value)
    {
        fixed (byte* ptr = &array[offset])
            *(long*)ptr = value;
    }

    //Deserializer
    public override void Des(ArraySegment<byte> buffer)
    {
        ushort size = 0;

        this.p_size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + size);
        size += sizeof(short);
        this.p_id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + size);
        size += sizeof(short);
        this.player_id = BitConverter.ToInt64(buffer.Array, buffer.Offset + size);
        size += sizeof(long);

        ulong messageLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + size);
        size += sizeof(short);
        this.message = Encoding.Unicode.GetString(buffer.Array, buffer.Offset + size, (int)messageLen);
        size += sizeof(short);

    }
    //Serializer
    public override ArraySegment<byte> Ser()
    {
        ArraySegment<byte> s = SendBufferHelper.Open(128);

        ushort _size = 0;


        ToBytes(s.Array, s.Offset + _size, p_size);
        _size += sizeof(short);
        ToBytes(s.Array, s.Offset + _size, p_id);
        _size += sizeof(short);
        ToBytes(s.Array, s.Offset + _size, player_id);
        _size += sizeof(long);



        //String Message
        ToBytes(s.Array, s.Offset + _size, message.Length * 2);
        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.message, 0, this.message.Length, s.Array, s.Offset + _size);
        _size += sizeof(ushort);
        _size += nameLen;
        return SendBufferHelper.Close(_size);
    }
}
