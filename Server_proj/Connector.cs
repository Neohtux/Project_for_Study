using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_Core
{
  
    public class Connector
    {
        Session _session;
        public void Connect(IPEndPoint iPEndPoint, Session session)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = iPEndPoint;
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
            _session = session;
            Socket socket = new Socket(args.RemoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            args.UserToken = socket;

            Start_Connect(args);
        }

        //소켓 args 전달, 비동기 커넥트
        private void Start_Connect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            bool pending = socket.ConnectAsync(args);

            if (pending == false)
                OnConnectCompleted(null, args);

        }
        //커넥트 이벤트 
        private void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _session.Start_Session(e.ConnectSocket);
                _session.OnConnected(e.RemoteEndPoint);
            }
        }

    }
}

