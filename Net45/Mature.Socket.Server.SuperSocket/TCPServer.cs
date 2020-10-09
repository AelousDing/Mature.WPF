using Mature.Socket;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.TCP.Server
{
    public class TCPServer : ITCPServer
    {
        public IEnumerable<SessionInfo> GetAllSession()
        {
            throw new NotImplementedException();
        }

        public SessionInfo GetSessionByID(string sessionID)
        {
            throw new NotImplementedException();
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
    }
}
