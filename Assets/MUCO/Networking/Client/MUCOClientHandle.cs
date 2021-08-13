using System.Collections;
using System.Collections.Generic;
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
        }
    }
}

