using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            // Send clientinto game
            MUCOServer.Clients[fromClient].SendIntoGame();
        }

        public static void PlayerMovement(int fromClient, MUCOPacket packet)
        {
            Vector3 position = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();

            MUCOServerSend.PlayerMovement(fromClient, position, rotation);
        }
    }
}

