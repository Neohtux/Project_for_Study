using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_Core
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            byte[] buffer = Encoding.UTF8.GetBytes("Welcome to Server !");
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
            Console.WriteLine($"[From client] : {client_data}");
        }

        public override void OnSend(byte[] buffer)
        {
            Console.WriteLine($"Transferred bytes : {buffer.Length}");
        }
    }
    public class Connector
    {
        //클라이언트 to 서버 연결 시작.

        public void Connect(IPEndPoint iPEndPoint)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = iPEndPoint;
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);

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
                GameSession session = new GameSession();
                session.Start_Session(e.ConnectSocket);
                session.OnConnected(e.RemoteEndPoint);
            }
        }

    }
}
