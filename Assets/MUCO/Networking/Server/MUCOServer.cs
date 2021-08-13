using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOServer
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, MUCOClient> Clients = new Dictionary<int, MUCOClient>();
        private static TcpListener m_TcpListener = null;

        public static event ServerLogEventHandler ServerLog;
        public delegate void ServerLogEventHandler(string message);
        public static void DebugLog(string message)
        {
            if (ServerLog != null)
            {
                ServerLog(message);
            }
        }

        public static void StartServer(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            DebugLog("Starting server...");

            InitializeServerData();

            m_TcpListener = new TcpListener(IPAddress.Any, Port);
            m_TcpListener.Start();
            m_TcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            DebugLog($"Server started on {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult asyncResult)
        {
            TcpClient client = m_TcpListener.EndAcceptTcpClient(asyncResult);
            m_TcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            DebugLog($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (Clients[i].TCP.Socket == null)
                {
                    Clients[i].TCP.Connect(client);
                    return;
                }
            }

            DebugLog($"{client.Client.RemoteEndPoint} failed to connect: Server is full.");
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new MUCOClient(i));
            }
        }
    }
}