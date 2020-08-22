using Server_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientSession : PacketSession
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
            PlayerPacket packet = new PlayerPacket();
            Console.WriteLine("되냐?");

            packet.Des(buffer);

            Console.WriteLine($"RecevPacket_Size : {packet.p_size},  packet_ID : {packet.p_id}");
            Console.WriteLine($"Message : {packet.message}");
            //
           // Console.WriteLine($"Player ID : {packet.player_id}");
            //Console.WriteLine($"Player Message : {packet.message}");
        }

        public override void OnSend(int length)
        {
            Console.WriteLine($"Transferred bytes : {length}");
        }
    }
}
