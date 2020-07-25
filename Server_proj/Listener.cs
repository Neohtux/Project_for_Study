using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_proj
{
    class Listener
    {
        public Listener() { }
        string my_host = Dns.GetHostName();

        //MAC주소와 IP주소, hostname을 받아올 수 있는 hostEntry 생성
        IPHostEntry hostEntry;
        //네트워크 끝점을 AddressList[1] IP주소와, 포트번호8887로 나타낸다.
        IPEndPoint endpoint;
        //Socket 생성 (TCP로 설정, TCP시 소켓타입은 스트림)
        Socket listen_socket;
        //소켓 바인드


        public void Init(SocketAsyncEventArgs Async_arg)
        {
            hostEntry = Dns.GetHostEntry(my_host);
            endpoint = new IPEndPoint(hostEntry.AddressList[1], 8887);
            listen_socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listen_socket.Bind(endpoint);
            listen_socket.Listen(10);
            
            //Accept();
            Start_Accept(Async_arg);
        }
        public void Start_Accept(SocketAsyncEventArgs asyncEventArgs)
        {
     
            if (asyncEventArgs == null)
            {
                asyncEventArgs = new SocketAsyncEventArgs();
                asyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else asyncEventArgs.AcceptSocket = null;
            
            bool pending = listen_socket.AcceptAsync(asyncEventArgs);

            if(!pending)
            {
                OnAcceptCompleted(null, asyncEventArgs);
            }
        }
        public void OnAcceptCompleted(object obj, SocketAsyncEventArgs args)
        {
            if (args.LastOperation == SocketAsyncOperation.Accept)
            {
                Socket client_socket = args.AcceptSocket;
                ProcessReceive(args,client_socket);
                ProcessSend(args,client_socket);
            }
            Start_Accept(args);

        }
       
        private void ProcessReceive(SocketAsyncEventArgs args,Socket client_Socket)
        {
            byte[] recv_buff = new byte[1024];

            int client_data_length = client_Socket.Receive(recv_buff);
            string client_data = Encoding.UTF8.GetString(recv_buff, 0, client_data_length);
            Console.WriteLine($"[From Client ] : {client_data}");
        }

        private void ProcessSend(SocketAsyncEventArgs args, Socket client_socket)
        {
            byte[] send_message = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
            client_socket.Send(send_message);
        }
    }
}
