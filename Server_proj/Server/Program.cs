using Server_Core;
using System;
using System.Net;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Server PC hostname을 얻어온다.
            string my_host = Dns.GetHostName();
            //MAC주소와 IP주소, hostname을 받아올 수 있는 hostEntry 생성
            IPHostEntry hostEntry;
            //네트워크 끝점을 AddressList[1] IP주소와, 포트번호8887로 나타낸다.
            IPEndPoint endpoint;
            hostEntry = Dns.GetHostEntry(my_host);
            endpoint = new IPEndPoint(hostEntry.AddressList[1], 8887);

            Listener listener = new Listener();
            listener.Init(endpoint, ()=> { return new ClientSession(); } );

            while (true) { };
        }
    }
}