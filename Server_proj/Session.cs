using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_proj
{
    class Session
    {
        Socket client_socket;
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        Queue<byte[]> sendMessage_queue = new Queue<byte[]>();

        bool Is_send_wait = false;
        object _lock = new object();

        public void Start_Session(Socket socket)
        {
            client_socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);

            byte[] send_buff = new byte[1024];
            send_buff = Encoding.UTF8.GetBytes("Welcome to Server !");
            recvArgs.SetBuffer(new byte[1024], 0,1024);
            Start_Receive(recvArgs);
            Send(send_buff);
        }
        void Start_Receive(SocketAsyncEventArgs args)
        {
            //리시브 비동기 호출
            bool pending = client_socket.ReceiveAsync(args);

            if(pending ==false)
            {
                OnRecvCompleted(null, args);
            } 
            
        }
        void Send(byte[] Buffer)
        {
            lock (_lock)
            {
                sendArgs.SetBuffer(Buffer, 0, Buffer.Length);
                sendMessage_queue.Enqueue(Buffer);

                if (Is_send_wait == false)
                    Start_Send(sendArgs);
            }
        }
       
        void Start_Send(SocketAsyncEventArgs args)
        {
            Is_send_wait = true;
            byte[] buff = sendMessage_queue.Dequeue();
            args.SetBuffer(buff, 0, buff.Length);

            bool pending = client_socket.SendAsync(args);

            if (pending == false)
            {
                OnSendCompleted(null, args);
            }
        }
        public void Disconnect()
        {
            client_socket.Shutdown(SocketShutdown.Both);
            client_socket.Close();
        }
        #region Send/Receive
        void OnSendCompleted(object obj, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                Is_send_wait = false;
            }
         
        }
        //멀티 스레드 작동 부분(danger zone!)
        void OnRecvCompleted(object obj,SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                string client_data = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                Console.WriteLine($"[From client] : {client_data}");
                Start_Receive(args); //Receive 예약.
            }
            else Disconnect();
        }
        #endregion
    }
}
