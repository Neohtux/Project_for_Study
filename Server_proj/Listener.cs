using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_Core
{
    public class Listener
    {
        public Listener() { }
 
        Socket listen_socket;
        Func<Session> _sessionFactory;
        
        public void Init(IPEndPoint iPEnd, Func<Session> sessionFactory)
        {
            //Socket 생성 (TCP로 설정, TCP시 소켓타입은 스트림)
            listen_socket = new Socket(iPEnd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _sessionFactory += sessionFactory;
            listen_socket.Bind(iPEnd);

            //backlog : 최대대기수
            listen_socket.Listen(11);
            SocketAsyncEventArgs args = null;
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
                Session _session = _sessionFactory.Invoke();
                _session.Start_Session(args.AcceptSocket);
                _session.OnConnected(args.AcceptSocket.RemoteEndPoint);

            }
            Start_Accept(args);

        }
    }
}
