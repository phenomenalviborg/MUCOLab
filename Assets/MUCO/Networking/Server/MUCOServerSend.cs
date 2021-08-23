using System.Collections;
using System.Collections.Generic;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOServerSend
    {
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

        #region packets
        public static void Welcome(int toClient, string message)
        {
            using (MUCOPacket packet = new MUCOPacket((int)ServerPackets.welcome))
            {
                packet.Write(message);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

        public static void UDPTest(int toClient)
        {
            using (MUCOPacket packet = new MUCOPacket((int)ServerPackets.udpTest))
            {
                packet.Write("A test packet for UDP.");

                SendUDPData(toClient, packet);
            }
        }
        #endregion
    }
}

