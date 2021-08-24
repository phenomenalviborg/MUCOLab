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

        public static void SpawnPlayer(MUCOPacket packet)
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();

            MUCOManager.s_Instance.SpawnPlayer(id, position, rotation);
        }

        public static void PlayerMovement(MUCOPacket packet)
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();

            MUCOManager.Players[id].transform.position = position;
            MUCOManager.Players[id].transform.rotation = rotation;
        }
    }
}

