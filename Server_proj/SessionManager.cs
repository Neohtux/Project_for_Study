using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server_Core
{
    
    public class SessionManager
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
        static Dictionary<int, Session> _sessions = new Dictionary<int, Session>();
        private static int sessionid = 0;
        object _lock = new object();

        

        public Session Generate(Session session)
        {
            lock(_lock)
            {
                int s_id = ++sessionid;
                Session s = session;
                s.SessionID = s_id;

                _sessions.Add(s_id, s);
                return s;
            }
        }

       
    }
}
