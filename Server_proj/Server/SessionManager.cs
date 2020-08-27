using Server_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    
    class SessionManager
    {
        #region Singleton
        static SessionManager _session;
  

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
        public static Dictionary<int, Session> _sessions = new Dictionary<int, Session>();
        private static int sessionid = 0;
        object _lock = new object();


       
        public Session Generate()
        {
            lock(_lock)
            {
                int s_id = ++sessionid;
                ClientSession s = new ClientSession();
                s.SessionID = s_id;

                _sessions.Add(s_id, s);
                return s;
            }
        }

       
    }
}
