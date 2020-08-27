using Server_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
        }


        public override void OnDisconnect(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffer, int ByteTransferred)
        {
            PlayerPacket packet = new PlayerPacket();
            packet.Des(buffer);
            Console.WriteLine($"[From Server] : {packet.message}");

            return buffer.Count;
        }

        public override void OnSend(int length)
        {
            Console.WriteLine($"[Client]  전송 bytes : {length}");
        }

    }
}
