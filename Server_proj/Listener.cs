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
 
        Socket listen_socket;
        Session _session = new Session();
        SocketAsyncEventArgs args;

        public void Init(IPEndPoint iPEnd)
        {
            //Socket 생성 (TCP로 설정, TCP시 소켓타입은 스트림)
            listen_socket = new Socket(iPEnd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listen_socket.Bind(iPEnd);

            //backlog : 최대대기수
            listen_socket.Listen(10);

            //Accept();
            Start_Accept(args);
        }
        //클라이언트로 부터 커넥션 요청에 의한 수용 작업을 시작
        public void Start_Accept(SocketAsyncEventArgs asyncEventArgs)
        {
            if (asyncEventArgs == null) //개체 생성이 안된경우.
            {
                asyncEventArgs = new SocketAsyncEventArgs();
                asyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else asyncEventArgs.AcceptSocket = null; //사용할 소켓 비우기
            
            bool pending = listen_socket.AcceptAsync(asyncEventArgs);

            if(!pending)
            {
                OnAcceptCompleted(null, asyncEventArgs);
            }
        }
        //멀티 스레드 작동 부분(danger zone!)
        void OnAcceptCompleted(object obj, SocketAsyncEventArgs args)
        {
            if (args.LastOperation == SocketAsyncOperation.Accept)
            {
                 _session.Start_Session(args.AcceptSocket);
                
                //ProcessSend(args, args.AcceptSocket);
            }
            Start_Accept(args);

        }
       
        private void ProcessSend(SocketAsyncEventArgs args, Socket client_socket)
        {
            byte[] send_message = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
            client_socket.Send(send_message);
        }
    }
}
