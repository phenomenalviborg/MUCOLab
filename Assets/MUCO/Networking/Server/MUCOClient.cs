using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOClient
    {
        public static int DataBufferSize = 4096;
        public int ID;
        public MUCOTcp TCP;
        public MUCOUDP UDP;
        public MUCOPlayer Player;

        public MUCOClient(int clientID)
        {
            ID = clientID;
            TCP = new MUCOTcp(ID);
            UDP = new MUCOUDP(ID);
        }

        public class MUCOTcp
        {
            public TcpClient Socket;

            private readonly int m_ID;
            private NetworkStream m_NetworkStream;
            private MUCOPacket m_ReceiveData;
            private byte[] m_ReceiveBuffer;

            public MUCOTcp(int id)
            {
                m_ID = id;
            }

            public void Connect(TcpClient socket)
            {
                MUCOServer.DebugLog("Registering new client on server.");

                Socket = socket;
                Socket.ReceiveBufferSize = DataBufferSize;
                Socket.SendBufferSize = DataBufferSize;

                m_NetworkStream = Socket.GetStream();

                m_ReceiveData = new MUCOPacket();
                m_ReceiveBuffer = new byte[DataBufferSize];

                m_NetworkStream.BeginRead(m_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

                MUCOServerSend.Welcome(m_ID, "Welcome to the server!");
            }

            public void SendData(MUCOPacket packet)
            {
                try
                {
                    if (Socket != null)
                    {
                        m_NetworkStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception exception)
                {
                    MUCOServer.DebugLog($"Error sending data to client {m_ID} via TCP: {exception}");
                }
            }

            private void ReceiveCallback(IAsyncResult asyncResult)
            {
                try
                {
                    int byteLength = m_NetworkStream.EndRead(asyncResult);
                    if (byteLength <= 0)
                    {
                        MUCOServer.Clients[m_ID].Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(m_ReceiveBuffer, data, byteLength);

                    m_ReceiveData.Reset(HandleData(data));

                    m_NetworkStream.BeginRead(m_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception exception)
                {
                    MUCOServer.DebugLog($"Error recieving TCP data: {exception}");
                    MUCOServer.Clients[m_ID].Disconnect();
                }
            }

            private bool HandleData(byte[] data)
            {
                int packetLenght = 0;

                m_ReceiveData.SetBytes(data);

                // If we have more than 4 bytes we know we have the start of a packet because an int consist of 4 bytes and the first data of any packet is an int representing the lenght of the packet.
                if (m_ReceiveData.UnreadLength() >= 4)
                {
                    packetLenght = m_ReceiveData.ReadInt();
                    if (packetLenght <= 0)
                    {
                        return true;
                    }
                }

                while (packetLenght > 0 && packetLenght <= m_ReceiveData.UnreadLength())
                {
                    byte[] packetBytes = m_ReceiveData.ReadBytes(packetLenght);
                    MUCOThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (MUCOPacket packet = new MUCOPacket(packetBytes))
                        {
                            int packetID = packet.ReadInt();
                            MUCOServer.s_PacketHandlers[packetID](m_ID, packet);
                        }
                    });

                    packetLenght = 0;

                    if (m_ReceiveData.UnreadLength() >= 4)
                    {
                        packetLenght = m_ReceiveData.ReadInt();
                        if (packetLenght <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (packetLenght <= 1)
                {
                    return true;
                }

                return false;
            }

            public void Disconnect()
            {
                Socket.Close();
                m_NetworkStream = null;
                m_ReceiveData = null;
                m_ReceiveBuffer = null;
                Socket = null;
            }
        }

        public class MUCOUDP
        {
            public IPEndPoint EndPoint;

            private int m_ID;

            public MUCOUDP(int id)
            {
                m_ID = id;
            }

            public void Connect(IPEndPoint endPoint)
            {
                EndPoint = endPoint;
            }

            public void SendData(MUCOPacket packet)
            {
                MUCOServer.SendUDPData(EndPoint, packet);
            }

            public void HandleData(MUCOPacket packet)
            {
                int packetLength = packet.ReadInt();
                byte[] packetBytes = packet.ReadBytes(packetLength);

                MUCOThreadManager.ExecuteOnMainThread(() =>
                {
                    using (MUCOPacket packet = new MUCOPacket(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        MUCOServer.s_PacketHandlers[packetId](m_ID, packet);
                    }
                });
            }

            public void Disconnect()
            {
                EndPoint = null;
            }
        }

        public void SendIntoGame()
        {
            Player = new MUCOPlayer(ID, new Vector3(0.0f, 0.0f, 0.0f));

            // Spawn other players that are already in the game for this client.
            foreach (MUCOClient client in MUCOServer.Clients.Values)
            {
                if (client.Player != null)
                {
                    if (client.ID != ID)
                    {
                        MUCOServerSend.SpawnPlayer(ID, client.Player);
                    }
                }
            }

            // Spawn this player for all other clients, includeing this client.
            foreach (MUCOClient client in MUCOServer.Clients.Values)
            {
                if (client.Player != null)
                {
                    MUCOServerSend.SpawnPlayer(client.ID, Player);
                }
            }

        }
       
        private void Disconnect()
        {
            Debug.Log($"{TCP.Socket.Client.RemoteEndPoint} has disconnected.");

            MUCOThreadManager.ExecuteOnMainThread(() =>
            { 
                Player = null;
            });

            TCP.Disconnect();
            UDP.Disconnect();
        }

    }
}
