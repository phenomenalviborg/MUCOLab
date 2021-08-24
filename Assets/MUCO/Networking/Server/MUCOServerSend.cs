using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOServerSend
    {
        #region TCPSenders
        private static void SendTCPData(int toClient, MUCOPacket packet)
        {
            packet.WriteLength();
            MUCOServer.Clients[toClient].TCP.SendData(packet);
        }

        private static void SendTCPDataToAll(MUCOPacket packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= MUCOServer.MaxPlayers; i++)
            {
                MUCOServer.Clients[i].TCP.SendData(packet);
            }
        }

        private static void SendTCPDataToAll(int exceptClient, MUCOPacket packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= MUCOServer.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    MUCOServer.Clients[i].TCP.SendData(packet);
                }
            }
        }
        #endregion

        #region UDPSenders
        private static void SendUDPData(int toClient, MUCOPacket packet)
        {
            packet.WriteLength();
            MUCOServer.Clients[toClient].UDP.SendData(packet);
        }

        private static void SendUDPDataToAll(MUCOPacket packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= MUCOServer.MaxPlayers; i++)
            {
                MUCOServer.Clients[i].UDP.SendData(packet);
            }
        }

        private static void SendUDPDataToAll(int exceptClient, MUCOPacket packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= MUCOServer.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    MUCOServer.Clients[i].UDP.SendData(packet);
                }
            }
        }
        #endregion

        #region Packets
        public static void Welcome(int toClient, string message)
        {
            using (MUCOPacket packet = new MUCOPacket((int)ServerPackets.welcome))
            {
                packet.Write(message);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

        public static void SpawnPlayer(int toClient, MUCOPlayer player)
        {
            using (MUCOPacket packet = new MUCOPacket((int)ServerPackets.spawnPlayer))
            {
                packet.Write(player.ID);
                packet.Write(player.Position);
                packet.Write(player.Rotation);

                SendTCPData(toClient, packet);
            }
        }

        public static void PlayerMovement(int exceptClient, Vector3 position, Quaternion rotation)
        {
            using (MUCOPacket packet = new MUCOPacket((int)ServerPackets.playerMovement))
            {
                packet.Write(exceptClient);
                packet.Write(position);
                packet.Write(rotation);

                SendUDPDataToAll(exceptClient, packet);
            }
        }
        #endregion
    }
}

