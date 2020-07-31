using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_proj
{
    abstract class Session
    {
        Socket client_socket; //접속한 클라이언트 소켓.
        SocketAsyncEventArgs sendArgs; //Send용 비동기 이벤트 
        SocketAsyncEventArgs recvArgs; //Receive용 비동기 이벤트
        Queue<byte[]> sendMessage_queue = new Queue<byte[]>();
       
        bool Is_send_wait = false;
        // Send 버퍼에 동시접근을 제어 하기위해 락을 사용.
        object _lock = new object(); 

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnReceive(byte[] buffer, int ByteTransferred);
        public abstract void OnDisconnect(EndPoint endPoint);
        public abstract void OnSend(byte[] buffer);

        public void Start_Session(Socket socket)
        {
            client_socket = socket;
            sendArgs = new SocketAsyncEventArgs();
            recvArgs = new SocketAsyncEventArgs();

            //콜백 이벤트 연결. 
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            byte[] buffer = Encoding.UTF8.GetBytes("Welcome to Server !");
            Receive();
            Send(buffer);
        }
        
        #region Send/Receive
        void Receive()
        {
            //리시브 비동기 호출
            bool pending = client_socket.ReceiveAsync(recvArgs);

            if (pending == false)
            {
                OnRecvCompleted(null, recvArgs);
            }
        }
        void Send(byte[] Buffer)
        {
            lock (_lock) //버퍼에는 순서대로 접근하되 내용은 한번에 담아서 보낸다.
            {
                sendArgs.SetBuffer(Buffer, 0, Buffer.Length);
                sendMessage_queue.Enqueue(Buffer);

                if (Is_send_wait == false)
                    Start_Send();
            }
        }
       
        void Start_Send()
        {
            Is_send_wait = true;
            byte[] buff = sendMessage_queue.Dequeue();
            sendArgs.SetBuffer(buff, 0, buff.Length);
            
            //Send 워커스레드 호출. 비동기 샌드
            bool pending = client_socket.SendAsync(sendArgs);

            if (pending == false)
            {
                OnSendCompleted(null, sendArgs);
            }
        }
        public void Disconnect()
        {
            OnDisconnect(client_socket.RemoteEndPoint);
            client_socket.Shutdown(SocketShutdown.Both);
            client_socket.Close();
        }
        // Send 워커 스레드 (Danger zone)
        void OnSendCompleted(object obj, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                OnSend(args.Buffer);
                Is_send_wait = false;
            }
            else Disconnect();
         
        }
        // Recv 워커 스레드 (Danger zone)
        void OnRecvCompleted(object obj,SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                OnReceive(args.Buffer, args.BytesTransferred);
                Receive(); //Receive 예약.

            }
            else Disconnect();

        }
        #endregion
    }
}
