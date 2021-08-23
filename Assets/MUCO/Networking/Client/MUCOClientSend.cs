using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOClientSend : MonoBehaviour
    {
        private static void SendTCPData(MUCOPacket packet)
        {
            packet.WriteLength();
            MUCOLocalClient.s_Instance.TCP.SendData(packet);
        }

        private static void SendUDPData(MUCOPacket packet)
        {
            packet.WriteLength();
            MUCOLocalClient.s_Instance.UCP.SendData(packet);
        }

        #region Packets
        public static void WelcomeReceived()
        {
            using (MUCOPacket packet = new MUCOPacket((int)ClientPackets.welcomeReceived))
            {
                packet.Write(MUCOLocalClient.s_Instance.ClientID);
                packet.Write("USERNAME");

                SendTCPData(packet);
            }
        }

        public static void UDPTestReceived()
        {
            using (MUCOPacket packet = new MUCOPacket((int)ClientPackets.udpTestReceived))
            {
                packet.Write("Received a UDP packet.");

                SendUDPData(packet);
            }
        }
        #endregion
    }

}

