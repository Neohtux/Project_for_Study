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
    public ulong player_id;
    private static unsafe void ToBytes(byte[] array, int offset, ulong value)
    {
        fixed (byte* ptr = &array[offset])
            *(ulong*)ptr = value;
    }

    //Deserializer
    public override void Des(ArraySegment<byte> buffer)
    {
        ushort size = 0;


        this.p_size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + size);
		            size += sizeof(ushort);
		this.p_id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + size);
		            size += sizeof(ushort);
		this.player_id = BitConverter.ToUInt64(buffer.Array, buffer.Offset + size);
		            size += sizeof(ulong);

    }
    //Serializer
    public override ArraySegment<byte> Ser()
    {
        ArraySegment<byte> s = SendBufferHelper.Open(4096);

        ushort _size = 0;
          ToBytes(s.Array, s.Offset+_size, p_size);
		            _size += sizeof(ushort);
		  ToBytes(s.Array, s.Offset+_size, p_id);
		            _size += sizeof(ushort);
		  ToBytes(s.Array, s.Offset+_size, player_id);
		            _size += sizeof(ulong);
        
        return SendBufferHelper.Close(_size);
    }
}
