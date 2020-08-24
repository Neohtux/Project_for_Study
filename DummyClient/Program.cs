using System;
using System.Net;
using System.Text;
using System.Threading;
using Server_Core;
namespace DummyClient
{
    
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
            SessionManager Instance = new SessionManager();


            //connector.Connect(endpoint, () => { return new ServerSession(); });
            Connector connector = new Connector();
            
            ServerSession session = new ServerSession();
            connector.Connect(endpoint, ()=> { return new ServerSession(); });
            
            

            while (true) {  }


        }
    }

}
