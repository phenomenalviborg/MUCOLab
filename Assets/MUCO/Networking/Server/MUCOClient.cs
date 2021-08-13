using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOClient
    {
        public static int DataBufferSize = 4096;
        public int ID;
        public MUCOTcp TCP;

        public MUCOClient(int clientID)
        {
            ID = clientID;
            TCP = new MUCOTcp(ID);
        }

        public class MUCOTcp
        {
            public TcpClient Socket;
            private readonly int m_ID;
            private NetworkStream m_NetworkStream;
            private byte[] m_ReceiveBuffer;

            public MUCOTcp(int id)
            {
                m_ID = id;
            }

            public void Connect(TcpClient socket)
            {
                Socket = socket;
                Socket.ReceiveBufferSize = DataBufferSize;
                Socket.SendBufferSize = DataBufferSize;

                m_NetworkStream = Socket.GetStream();

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
                        // TODO: disconnect
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(m_ReceiveBuffer, data, byteLength);

                    // TODO: handle data

                    m_NetworkStream.BeginRead(m_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception exception)
                {
                    MUCOServer.DebugLog($"Error recieving TCP data: {exception}");
                    // TODO: disconnect
                }
            }
        }
        
    }
}
