﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_proj
{
    class Session
    {
        Socket client_socket;

        public void Start_Session(Socket socket)
        {
            client_socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);

            //byte[] recv_buff = new byte[1024];
            recvArgs.SetBuffer(new byte[1024], 0,1024);

            Start_Init(recvArgs);
        }
        void Start_Init(SocketAsyncEventArgs args)
        {
            //리시브 비동기 호출
            bool pending = client_socket.ReceiveAsync(args);

            if(pending ==false)
            {
                OnRecvCompleted(null, args);
            } 
            
        }
        public void Disconnect()
        {
            client_socket.Shutdown(SocketShutdown.Both);
            client_socket.Close();
        }
        #region Send/Receive

        //멀티 스레드 작동 부분(danger zone!)
        void OnRecvCompleted(object obj,SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                string client_data = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                Console.WriteLine($"[From client] : {client_data}");
            }
            Start_Init(args);
        }
        #endregion
    }
}