using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Core
{
    public class RecvBuffer
    {
        //[][][r][][w][][][][][]
        ArraySegment<byte> _buffer;

        int _readPos; // 처리해야할 데이터 유효 범위  (_writePos - _readPos)
        int _writePos; //몇 바이트까지 받았는지, 다음에 받아야할 위치

        public RecvBuffer(int buffserSize)
        {
            _buffer = new ArraySegment<byte>(new byte[buffserSize], 0, buffserSize);
        }

        //버퍼에 받은 데이터 범위
        public int DataSize { get { return _writePos - _readPos; } }

        //버퍼에 남은 유효 범위
        public int FreeSize { get { return _buffer.Count - _writePos; } }
        
        // DataSegment (받은 데이터 범위 크기의 ArraySegment<byte>를 반환
        // 어디서 부터(_readPos) ~ 어디 까지(DataSize) 읽어야 되는지
        public ArraySegment<byte> DataSegment { get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset+_readPos, DataSize); } }


        // 사용할 수 있는 버퍼의 유효 범위 RecvSegment 
        // 다음 Receive에서 어디서 부터(_writePos) ~ 어디까지가 유효범위인지  (FreeSize)
        public ArraySegment<byte> FreeSegment { get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset+_writePos, FreeSize); } }


        // Clean()r , w 가 버퍼 끝까지 밀릴 수 있으므로 r,w Pos를 당겨주는 역할

        public void Clean()
        {
            int datasize = DataSize;
            
            if(datasize == 0) //[rw] 겹쳐 있는 상황 모든 데이터를 이상없이 처리한 상태
            {
                //남은 데이터가 없으면 복사하지 않고 커서를 원위치(0)로
                _readPos = _writePos = 0;
            }
            else //클라이언트로 부터 데이터를 일부만 받은 상태.
            {

                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, 
                           _buffer.Array, _buffer.Offset, DataSize);

                _readPos = 0;
                _writePos = DataSize;
            }
        }

        //_readPos , _writePos 커서 이동
        public bool OnRead(int transferredSize)
        {
            if (transferredSize > DataSize)
                return false;

            _readPos += transferredSize;
            return true;
        }
        public bool OnWrite(int transferredSize)
        {
            if (transferredSize > FreeSize)
                return false;

            _writePos += transferredSize;
            return true;
        }

    }
}
