using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
     class SessionManager
    {
        #region Singleton
        static SessionManager _session = new SessionManager();

        public static SessionManager Instance
        {
            get
            {
                if (_session == null)
                    _session = new SessionManager();
                return _session;
            }
        }
        #endregion
        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();

        public void SendForEach()
        {
            lock(_lock)
            {
                foreach(ServerSession session in _sessions)
                {
                    PlayerPacket packet = new PlayerPacket();
                    packet.p_id = 1;
                    packet.player_id = 121;
                    packet.message = "Hi Server! I'm groot";
                    packet.p_size += (ushort)(packet.message.Length * 2 + 22);
                    ArraySegment<byte> segment = packet.Ser();

                    session.Send(segment);
                }
            }
        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }


    }
}
