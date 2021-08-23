using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOClientHandle : MonoBehaviour
    {
        public static void Welcome(MUCOPacket packet)
        {
            string message = packet.ReadString();
            int clientID = packet.ReadInt();

            Debug.Log($"[CLIENT] Message from server: {message}");
            MUCOLocalClient.s_Instance.ClientID = clientID;
            MUCOClientSend.WelcomeReceived();

            MUCOLocalClient.s_Instance.UCP.Connect(((IPEndPoint)MUCOLocalClient.s_Instance.TCP.Socket.Client.LocalEndPoint).Port);
        }

        public static void UDPTest(MUCOPacket packet)
        {
            string msg = packet.ReadString();

            Debug.Log($"[CLIENT] Received packet via UDP. Contains message: {msg}");
            MUCOClientSend.UDPTestReceived();
        }
    }
}

