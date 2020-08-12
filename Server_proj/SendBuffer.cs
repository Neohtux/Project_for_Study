using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_Core
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> Current_Buffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int ChunkSize { get; set; } = 4096;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if(Current_Buffer.Value == null)
            {
                Current_Buffer.Value = new SendBuffer(ChunkSize);
            }

            //남은공간(FreeSize)보다 만드려는 공간의 크기(reserveSize)가 큰경우
            if (Current_Buffer.Value.FreeSize < reserveSize) 
                Current_Buffer.Value = new SendBuffer(ChunkSize);

            return Current_Buffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return Current_Buffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer
    {
        //[][][_u][][][]
        byte[] _buffer;
        int _usedSize = 0;
        
        //사용 가능한 버퍼의 크기 
        public int FreeSize { get{ return _buffer.Length - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }
        public static ArraySegment<byte> Empty { get; }

        
        public ArraySegment<byte> Open(int reserveSize)
        {
            //예약된 공간이, 남아있는 공간보다 작거나 같은경우
            // _usedSize ~ reserveSize 크기의 byte ArrraySegment를 반환한다
            if (reserveSize <= FreeSize)
                return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);

            return Empty;
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;

            return segment;
        }
    }
}
