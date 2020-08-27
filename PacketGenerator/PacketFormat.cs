using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    class PacketFormat
    {
        public static string fileFomrat =
@"using Server_Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;";
        //{0} 패킷 이름
        //{1} 멤버 변수
        //{2} 멤버 변수 Des
        //{3} 멤버 변수 Ser
        public static string packetFormat =
@"
 public abstract class Packet
{{
     public ushort p_size;
        public ushort p_id;
        public abstract ArraySegment<byte> Ser();
        public abstract void Des(ArraySegment<byte> buffer);
   
}}
public class {0} : Packet
{{
    public ulong player_id;
    public string message;
    
    private static unsafe void ToBytes(byte[] array, int offset, ulong value)
    {{
        fixed (byte* ptr = &array[offset])
            *(ulong*)ptr = value;
    }}

    //Deserializer
    public override void Des(ArraySegment<byte> buffer)
    {{
        ushort size = 0;


        {3}
        ulong messageLen = BitConverter.ToUInt16(buffer.Array, buffer.Offset + size);
        size += sizeof(short);
        this.message = Encoding.Unicode.GetString(buffer.Array, buffer.Offset + size, (int)messageLen);

    }}
    //Serializer
    public override ArraySegment<byte> Ser()
    {{
        ArraySegment<byte> s = SendBufferHelper.Open(128);

        ushort _size = 0;
        {2}
        
        //String Message
        ToBytes(s.Array, s.Offset + _size, message.Length * 2);
        _size += sizeof(short);
        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.message, 0, this.message.Length, s.Array, s.Offset + _size);
        _size += sizeof(long);
        _size += nameLen; 
        return SendBufferHelper.Close(_size);
    }}
}}
";
        //{0} 변수 형식
        //{1} 변수 이름
        public static string memberFormat =
@"public {0} {1}";

        //{0} 변수 이름
        //{1} To~ 변수형식
        //{2} 변수 형식
        public static string desFormat=
@"this.{0} = BitConverter.{1}(buffer.Array, buffer.Offset + size);
            size += sizeof({2});";

        //{0} 변수 이름
        //{1} 변수 형식
        public static string serFormat =
@"  ToBytes(s.Array, s.Offset+_size, {0});
            _size += sizeof({1});";

        public static string messageFormat =
"@ToBytes(s.Array,s.Offset+_size,{0});";
             


    }
}
