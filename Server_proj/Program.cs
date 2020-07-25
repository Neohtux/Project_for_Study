using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_proj
{
  
    class Program : Listener
    {
        static SocketAsyncEventArgs arg;

        
        static void Main(string[] args)
        {
            // Server PC hostname을 얻어온다.
            Listener listener = new Listener();
            listener.Init(arg);

            while (true) { };
        }
    }
}
