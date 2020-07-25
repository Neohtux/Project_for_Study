using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_proj
{
    class Program
    {
        
        static void Main(string[] args)
        {
            // Server PC hostname을 얻어온다.
            string my_host = Dns.GetHostName();

            //MAC주소와 IP주소, hostname을 받아올 수 있는 hostEntry 생성
            IPHostEntry hostEntry = Dns.GetHostEntry(my_host);

            //네트워크 끝점을 AddressList[1] IP주소와, 포트번호8887로 나타낸다.
            IPEndPoint endpoint = new IPEndPoint(hostEntry.AddressList[1], 8887);

            //Socket 생성 (TCP로 설정, TCP시 소켓타입은 스트림)
            Socket server_socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //소켓 바인드
            server_socket.Bind(endpoint);

            server_socket.Listen(10);
            //Listen 
            while (true)
            {       
                //Accept
                Socket client_socket = server_socket.Accept();

                //receive
                byte[] recv_buff = new byte[1024];

                int client_data_length = client_socket.Receive(recv_buff);
                string client_data = Encoding.UTF8.GetString(recv_buff, 0, client_data_length);
                Console.WriteLine($"[From Client ] : {client_data}");

                byte[] send_message = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
                client_socket.Send(send_message);
            }
        }
    }
}
