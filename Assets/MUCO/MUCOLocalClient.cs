using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

using Sirenix.OdinInspector;

public class MUCOLocalClient : MonoBehaviour
{
    public static MUCOLocalClient s_Instance;
    public static int DataBufferSize = 4096;
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 26950;
    public int ClientID = 0;
    public MUCOClientTCP TCP;
    
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
    }

    public void ConnectToServer()
    {
        TCP.Connect();
    }

    public class MUCOClientTCP
    {
        public TcpClient Socket;
        private NetworkStream m_NetworkStream;
        private byte[] m_ReceiveBuffer;

        public void Connect()
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            m_ReceiveBuffer = new byte[DataBufferSize];
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

            m_NetworkStream.BeginRead(m_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
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
                Debug.LogError($"Error recieving TCP data: {exception}");
                // TODO: disconnect
            }
        }
    }
}
