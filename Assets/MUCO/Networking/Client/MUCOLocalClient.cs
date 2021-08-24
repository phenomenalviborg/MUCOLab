using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

using Sirenix.OdinInspector;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOLocalClient : MonoBehaviour
    {
        public static MUCOLocalClient s_Instance;
        public static int s_DataBufferSize = 4096;
        public string ServerIP = "127.0.0.1";
        public int ServerPort = 26950;
        public int ClientID = 0;
        public MUCOClientTCP TCP;
        public MUCOUDP UDP;

        private delegate void PacketHandler(MUCOPacket packet);
        private static Dictionary<int, PacketHandler> s_PacketHandlers;

        private bool m_IsConnected = false;

        // TMP
        [Button("Connect To Server")]
        private void OnConnectToServerButtonPressed()
        {
            ConnectToServer();
        }

        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else if (s_Instance != this)
            {
                Debug.LogWarning("Instance already exists, destroying component!");
                Destroy(this);
            }
        }

        private void Start()
        {
            TCP = new MUCOClientTCP();
            UDP = new MUCOUDP();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public void ConnectToServer()
        {
            InitializeClientData();
            m_IsConnected = true;
            TCP.Connect();
        }

        public class MUCOClientTCP
        {
            public TcpClient Socket;

            private NetworkStream m_NetworkStream;
            private MUCOPacket m_ReceiveData;
            private byte[] m_ReceiveBuffer;

            public void Connect()
            {
                Socket = new TcpClient
                {
                    ReceiveBufferSize = s_DataBufferSize,
                    SendBufferSize = s_DataBufferSize
                };

                m_ReceiveBuffer = new byte[s_DataBufferSize];
                Socket.BeginConnect(s_Instance.ServerIP, s_Instance.ServerPort, ConnectCallback, Socket);
            }

            private void ConnectCallback(IAsyncResult asyncResult)
            {
                Socket.EndConnect(asyncResult);

                if (!Socket.Connected)
                {
                    return;
                }

                m_NetworkStream = Socket.GetStream();

                m_ReceiveData = new MUCOPacket();

                m_NetworkStream.BeginRead(m_ReceiveBuffer, 0, s_DataBufferSize, ReceiveCallback, null);
            }

            private void ReceiveCallback(IAsyncResult asyncResult)
            {
                try
                {
                    int byteLength = m_NetworkStream.EndRead(asyncResult);
                    if (byteLength <= 0)
                    {
                        s_Instance.Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(m_ReceiveBuffer, data, byteLength);

                    // HandleData returns of the Packet should be reset; there might be more data in the buffer, TCP will sometimes send half packages.
                    m_ReceiveData.Reset(HandleData(data));

                    m_NetworkStream.BeginRead(m_ReceiveBuffer, 0, s_DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Error recieving TCP data: {exception}");
                    Disconnect();
                }
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
                    Debug.Log($"Error sending data to server via TCP: {exception}");
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
                            s_PacketHandlers[packetID](packet);
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

            private void Disconnect()
            {
                s_Instance.Disconnect();

                m_NetworkStream = null;
                m_ReceiveData = null;
                m_ReceiveBuffer = null;
                Socket = null;
            }
        }

        public class MUCOUDP
        {
            public UdpClient Socket;
            public IPEndPoint EndPoint;

            public MUCOUDP()
            {
                EndPoint = new IPEndPoint(IPAddress.Parse(s_Instance.ServerIP), s_Instance.ServerPort);
            }

            public void Connect(int localPort)
            {
                Socket = new UdpClient(localPort);

                Socket.Connect(EndPoint);
                Socket.BeginReceive(ReceiveCallback, null);

                using (MUCOPacket packet = new MUCOPacket())
                {
                    SendData(packet);
                }
            }

            public void SendData(MUCOPacket packet)
            {
                try
                {
                    packet.InsertInt(s_Instance.ClientID);
                    if (Socket != null)
                    {
                        Socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log($"Error sending data to server via UDP: {exception}");

                }
            }

            private void ReceiveCallback(IAsyncResult asyncResult)
            {
                try
                {
                    byte[] data = Socket.EndReceive(asyncResult, ref EndPoint);
                    Socket.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 4)
                    {
                        s_Instance.Disconnect();
                        return;
                    }

                    HandleData(data);
                }
                catch
                {
                    Disconnect();
                }
            }

            private void HandleData(byte[] data)
            {
                using (MUCOPacket packet = new MUCOPacket(data))
                {
                    int packetLength = packet.ReadInt();
                    data = packet.ReadBytes(packetLength);
                }

                MUCOThreadManager.ExecuteOnMainThread(() =>
                {
                    using (MUCOPacket packet = new MUCOPacket(data))
                    {
                        int packetId = packet.ReadInt();
                        s_PacketHandlers[packetId](packet);
                    }
                });
            }

            private void Disconnect()
            {
                s_Instance.Disconnect();

                EndPoint = null;
                Socket = null;
            }
        }

        private void InitializeClientData()
        {
            s_PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ServerPackets.welcome, MUCOClientHandle.Welcome },
                { (int)ServerPackets.spawnPlayer, MUCOClientHandle.SpawnPlayer },
                { (int)ServerPackets.playerMovement, MUCOClientHandle.PlayerMovement }

            };
            Debug.Log("[CLIENT] Initialized packets.");
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 50, 50), "Connect to server."))
            {
                ConnectToServer();
            }
        }

        private void Disconnect()
        {
            if (m_IsConnected)
            {
                m_IsConnected = false;
                TCP.Socket.Close();
                UDP.Socket.Close();

                Debug.Log("Disconnected from server.");
            }
        }
    }
}

// https://github.com/tom-weiland/tcp-udp-networking/tree/tutorial-part5