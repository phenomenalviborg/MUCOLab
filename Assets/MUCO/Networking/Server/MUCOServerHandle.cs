using System;
using System.Collections;
using System.Collections.Generic;

namespace PhenomenalViborg.MUCO.Networking
{
    public class MUCOServerHandle
    {
        public static void WelcomeRecived(int fromClient, MUCOPacket packet)
        {
            int clientID = packet.ReadInt();
            string clientUsername = packet.ReadString();

            MUCOServer.DebugLog($"{MUCOServer.Clients[fromClient].TCP.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
            if (fromClient != clientID)
            {
                MUCOServer.DebugLog($"Player \"{clientUsername}\"(ID: {fromClient}) has assumed the wrong client ID ({clientID})!");
            }

            // TODO: Send player into game
        }
    }
}

