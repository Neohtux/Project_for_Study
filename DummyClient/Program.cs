using System;
using System.Net;
using System.Text;
using System.Threading;
using Server_Core;
namespace DummyClient
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            byte[] buffer = Encoding.UTF8.GetBytes("Hi I'm Client !");
            Send(buffer);
            Console.WriteLine($"OnConnected : {endPoint}");
        }
        public override void OnDisconnect(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnReceive(byte[] buffer, int ByteTransferred)
        {
            string client_data = Encoding.UTF8.GetString(buffer, 0, ByteTransferred);
            Console.WriteLine($"[From Server] : {client_data}");
        }

        public override void OnSend(byte[] buffer)
        {
            Console.WriteLine($"Transferred bytes : {buffer.Length}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // client PC hostname을 얻어온다.
            string my_host = Dns.GetHostName();
            //MAC주소와 IP주소, hostname을 받아올 수 있는 hostEntry 생성

            IPHostEntry hostEntry = Dns.GetHostEntry(my_host);
            //네트워크 끝점을 AddressList[1] IP주소와, 포트번호8887로 나타낸다.
            IPEndPoint endpoint = new IPEndPoint(hostEntry.AddressList[1], 8887);

            while (true)
            {
                Connector connector = new Connector();
                GameSession session = new GameSession();
                connector.Connect(endpoint,session);

                Thread.Sleep(100);
            }

        }
    }

}
