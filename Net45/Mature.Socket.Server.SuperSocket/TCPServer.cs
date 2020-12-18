using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mature.Socket.Server.SuperSocket
{
    public class TCPServer : ITCPServer
    {
        public IEnumerable<SessionInfo> GetAllSession()
        {
            var sessions = server?.GetAllSessions();
            if (sessions != null && sessions.Count() > 0)
            {
                foreach (var item in sessions)
                {
                    yield return new SessionInfo()
                    {
                        SessionID = item.SessionID,
                        LastActiveTime = item.LastActiveTime,
                        StartTime = item.StartTime,
                        LocalEndPoint = item.LocalEndPoint,
                        RemoteEndPoint = item.RemoteEndPoint
                    };
                }
            }
        }

        public SessionInfo GetSessionByID(string sessionID)
        {
            var session = server?.GetAllSessions()?.FirstOrDefault(p => p.SessionID == sessionID);
            return session == null ? null : new SessionInfo()
            {
                SessionID = session.SessionID,
                LastActiveTime = session.LastActiveTime,
                StartTime = session.StartTime,
                LocalEndPoint = session.LocalEndPoint,
                RemoteEndPoint = session.RemoteEndPoint
            };
        }

        public event EventHandler<SessionInfo> NewSessionConnected;
        public event EventHandler<SessionInfo> SessionClosed;
        public event Action<ISessionWrapper, StringPackageInfo> NewRequestReceived;

        private void OnNewSessionConnected(SessionInfo sessionInfo)
        {
            if (NewSessionConnected != null)
            {
                NewSessionConnected(this, sessionInfo);
            }
        }
        private void OnSessionClosed(SessionInfo sessionInfo)
        {
            if (SessionClosed != null)
            {
                SessionClosed(this, sessionInfo);
            }
        }
        private void OnNewRequestReceived(ISessionWrapper session, StringPackageInfo requestInfo)
        {
            if (NewRequestReceived != null)
            {
                NewRequestReceived(session, requestInfo);
            }
            else
            {
                //命令模式
            }
        }
        MatureServer server;
        public bool Start()
        {
            server = new MatureServer();
            var serverConfig = new ServerConfig
            {
                Port = 2020,
                KeepAliveInterval = 5,
                KeepAliveTime = 60
            };
            if (!server.Setup(serverConfig))
            {
                Console.WriteLine("Failed to setup!");
                return false;
            }
            if (!server.Start())
            {
                Console.WriteLine("Failed to setup!");
                return false;
            }
            server.NewRequestReceived += Server_NewRequestReceived;
            server.NewSessionConnected += Server_NewSessionConnected;
            server.SessionClosed += Server_SessionClosed;
            Console.WriteLine("The server started successfully!");
            return true;
        }

        private void Server_SessionClosed(MatureSession session, CloseReason value)
        {
            OnSessionClosed(new SessionInfo
            {
                SessionID = session.SessionID,
                StartTime = session.StartTime,
                LastActiveTime = session.LastActiveTime,
                LocalEndPoint = session.LocalEndPoint,
                RemoteEndPoint = session.RemoteEndPoint
            });
        }

        private void Server_NewSessionConnected(MatureSession session)
        {
            OnNewSessionConnected(new SessionInfo
            {
                SessionID = session.SessionID,
                StartTime = session.StartTime,
                LastActiveTime = session.LastActiveTime,
                LocalEndPoint = session.LocalEndPoint,
                RemoteEndPoint = session.RemoteEndPoint
            });
        }

        private void Server_NewRequestReceived(MatureSession session, StringRequestInfo requestInfo)
        {
            OnNewRequestReceived(new SessionWrapper(session), new StringPackageInfo
            {
                Key = requestInfo.Key,
                Body = requestInfo.Body,
                MessageId = requestInfo.Parameters[0]
            });
        }

        public void Stop()
        {
            if (server != null)
            {
                server.Stop();
            }
        }
    }
}
