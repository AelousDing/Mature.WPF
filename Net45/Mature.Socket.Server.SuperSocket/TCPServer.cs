using Mature.Socket;
using Mature.Socket.Common.SuperSocket;
using Mature.Socket.Common.SuperSocket.Compression;
using Mature.Socket.Common.SuperSocket.Validation;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.SuperSocket
{
    public class TCPServer : ITCPServer
    {
        public IEnumerable<SessionInfo> GetAllSession()
        {
            if (bootstrap != null && bootstrap.AppServers != null && bootstrap.AppServers.Count() > 0)
            {
                var sessions = (bootstrap.AppServers.First() as AppServer).GetAllSessions();
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
        }

        public SessionInfo GetSessionByID(string sessionID)
        {
            var session = (bootstrap?.AppServers?.First() as AppServer).GetAllSessions()?.FirstOrDefault(p => p.SessionID == sessionID);
            return session == null ? null : new SessionInfo()
            {
                SessionID = session.SessionID,
                LastActiveTime = session.LastActiveTime,
                StartTime = session.StartTime,
                LocalEndPoint = session.LocalEndPoint,
                RemoteEndPoint = session.RemoteEndPoint
            };
        }

        IBootstrap bootstrap;

        public event EventHandler<SessionInfo> NewSessionConnected;
        public event EventHandler<SessionInfo> SessionClosed;

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

        private void RegisterSessionStateChanged(AppServer appserver)
        {
            if (bootstrap.AppServers != null)
            {
                appserver.NewSessionConnected -= TCPServer_NewSessionConnected;
                appserver.NewSessionConnected += TCPServer_NewSessionConnected;
                appserver.SessionClosed -= TCPServer_SessionClosed;
                appserver.SessionClosed += TCPServer_SessionClosed;
                appserver.NewRequestReceived -= TCPServer_NewRequestReceived;
                appserver.NewRequestReceived -= TCPServer_NewRequestReceived;
            }
        }

        private void TCPServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            IContentBuilder contentBuilder = new ContentBuilder(new GZip(), new MD5DataValidation());
            Console.WriteLine($"接收到消息，Key：{requestInfo.Key} Body:{requestInfo.Body} MessageId:{requestInfo.GetFirstParam()}");

            var data = contentBuilder.Builder(requestInfo.Key, requestInfo.Body, requestInfo.GetFirstParam());
            session.Send(data, 0, data.Length);
        }

        private void TCPServer_SessionClosed(AppSession session, CloseReason value)
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

        private void TCPServer_NewSessionConnected(AppSession session)
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

        public bool Start()
        {
            bootstrap = BootstrapFactory.CreateBootstrap();
            if (!bootstrap.Initialize())
            {
                Console.WriteLine("Failed to initialize SuperSocket ServiceEngine! Please check error log for more information!");
                return false;
            }
            Console.WriteLine("Starting...");

            var result = bootstrap.Start();

            Console.WriteLine("-------------------------------------------------------------------");

            foreach (var server in bootstrap.AppServers)
            {
                if (server.State == ServerState.Running)
                {
                    Console.WriteLine("- {0} has been started", server.Name);
                }
                else
                {
                    Console.WriteLine("- {0} failed to start", server.Name);
                }
            }

            Console.WriteLine("-------------------------------------------------------------------");

            switch (result)
            {
                case (StartResult.None):
                    Console.WriteLine("No server is configured, please check you configuration!");
                    return false;

                case (StartResult.Success):
                    Console.WriteLine("The SuperSocket ServiceEngine has been started!");
                    break;

                case (StartResult.Failed):
                    Console.WriteLine("Failed to start the SuperSocket ServiceEngine! Please check error log for more information!");
                    return false;

                case (StartResult.PartialSuccess):
                    Console.WriteLine("Some server instances were started successfully, but the others failed! Please check error log for more information!");
                    break;
            }
            return true;
        }

        public void Stop()
        {
            if (bootstrap != null)
            {
                bootstrap.Stop();
            }
        }

        public void Notify()
        {
            var sessions = (bootstrap?.AppServers?.First() as AppServer)?.GetAllSessions();
            IContentBuilder contentBuilder = new ContentBuilder(new GZip(), new MD5DataValidation());
            string messageId = Guid.NewGuid().ToString().Replace("-", "");
            var data = contentBuilder.Builder("Notify", "Notify Test Server封装不了啊，分离不出来连接对象", messageId);
            if (sessions == null)
            {
                return;
            }
            foreach (var item in sessions)
            {
                item.Send(data, 0, data.Length);
            }
        }
    }
}
