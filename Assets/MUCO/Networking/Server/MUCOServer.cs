using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOServer
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, MUCOClient> Clients = new Dictionary<int, MUCOClient>();

        private static TcpListener m_TcpListener = null;
        private static UdpClient m_UDPListener = null;

        public static event ServerLogEventHandler ServerLog;
        public delegate void ServerLogEventHandler(string message);
        public static void DebugLog(string message)
        {
            Debug.Log(message);
            if (ServerLog != null)
            {
                ServerLog(message);
            }
        }

        public delegate void PacketHandler(int fromClient, MUCOPacket packet);
        public static Dictionary<int, PacketHandler> s_PacketHandlers = new Dictionary<int, PacketHandler>();

        public static void StartServer(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            DebugLog("Starting server...");

            InitializeServerData();

            m_TcpListener = new TcpListener(IPAddress.Any, Port);
            m_TcpListener.Start();
            m_TcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            m_UDPListener = new UdpClient(Port);
            m_UDPListener.BeginReceive(UDPReceiveCallback, null);

            DebugLog($"Server started on {Port}.");
        }

        public static void StopServer()
        {
            // TODO: Disconnect all clients

            m_TcpListener.Stop();
            m_UDPListener.Close();

            foreach (Delegate listener in ServerLog.GetInvocationList())
            {
                ServerLog -= (ServerLogEventHandler)listener;
            }
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

        private static void UDPReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = m_UDPListener.EndReceive(asyncResult, ref clientEndPoint);
                m_UDPListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (MUCOPacket packet = new MUCOPacket(data))
                {
                    int clientId = packet.ReadInt();

                    if (clientId == 0)
                    {
                        // This should neaver be true, but if it is, this block will prevent a server crash.
                        return;
                    }

                    if (Clients[clientId].UDP.EndPoint == null)
                    {
                        Clients[clientId].UDP.Connect(clientEndPoint);
                        return;
                    }

                    if (Clients[clientId].UDP.EndPoint.ToString() == clientEndPoint.ToString())
                    {
                        Clients[clientId].UDP.HandleData(packet);
                    }
                }
            }
            catch (Exception exception)
            {
                DebugLog($"Error receiving UDP data: {exception}");
            }
        }

        public static void SendUDPData(IPEndPoint clientEndPoint, MUCOPacket packet)
        {
            try
            {
                if(clientEndPoint != null)
                {
                    m_UDPListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception exception)
            {
                DebugLog($"Error sending data to {clientEndPoint} via UDP: {exception}");
            }
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new MUCOClient(i));
            }

            s_PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, MUCOServerHandle.WelcomeRecived },
                { (int)ClientPackets.playerMovement, MUCOServerHandle.PlayerMovement },
            };
            MUCOServer.DebugLog("Initialized packets.");
        }
    }
}