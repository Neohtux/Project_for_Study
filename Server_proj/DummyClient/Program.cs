using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

            //Socket 생성 (TCP로 설정, TCP시 소켓타입은 스트림)
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //receive buff
            byte[] recv_buff = new byte[1024];

            //send data
            byte[] send_data = Encoding.UTF8.GetBytes("I'm Client! :)");
    
            //Connect
            socket.Connect(endpoint);

            //Send
            socket.Send(send_data);

            //Receive
            int server_data_length = socket.Receive(recv_buff);
            string server_data = Encoding.UTF8.GetString(recv_buff,0,server_data_length);

            Console.WriteLine($"[From to Server] : {server_data}");


        }
    }
}
