using Server_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {

        }
        public override void OnDisconnect(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            if (buffer.Count == 0) return;
            PlayerPacket packet = new PlayerPacket();
            packet.Des(buffer);
            packet.message += "  " +SessionID+"번째 그루트";

            
            Console.WriteLine($"[From Client message] : {packet.message}");

        }

     
        public override void OnSend(int length)
        {
            Console.WriteLine($"Transferred bytes : {length}");
        }

       
    }
    
}
