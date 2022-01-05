using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Net.Sockets;

namespace PhenomenalViborg.MUCOSDK
{

    public class MUCODatasheetWidget : MonoBehaviour
    {
        [SerializeField] TMP_Text m_ServerColumn;
        [SerializeField] TMP_Text m_UserColumn;

        void FixedUpdate()
        {
            // Fill server column
            string serverDataText = "<b>SERVER DATA</b><br>";

            if (MUCOServerNetworkManager.Instance.Server != null)
            {
                serverDataText += " - <b>Server Configuration</b><br>";
                serverDataText += $"    • Port: {MUCOServerNetworkManager.Instance.Server.GetPort()}<br>";
                serverDataText += " - <b>Server Runtime</b><br>";
                serverDataText += $"    • Active Users: {MUCOServerNetworkManager.Instance.Server.ClientInfo.Count}<br>";
                serverDataText += $"    • Packets Sent: {MUCOServerNetworkManager.Instance.Server.ServerStatistics.PacketsSent}<br>";
                serverDataText += $"    • Packets Received: {MUCOServerNetworkManager.Instance.Server.ServerStatistics.PacketsReceived}<br>";
            }

            m_ServerColumn.SetText(serverDataText);

            // Fill user column
            string userDataText = "<b>ACTIVE USERS</b><br>";

            if (MUCOServerNetworkManager.Instance.Server != null)
            {
                foreach (MUCONet.MUCOServer.MUCOClientInfo clientInfo in MUCOServerNetworkManager.Instance.Server.ClientInfo.Values)
                {
                    userDataText += $" - <b>User {clientInfo.UniqueIdentifier}</b><br>";
                    userDataText += $"    • Unique Identifier: {clientInfo.UniqueIdentifier}<br>";
                    userDataText += $"    • Port: {((IPEndPoint)clientInfo.RemoteSocket.RemoteEndPoint).Port}<br>";
                    userDataText += $"    • Address: {((IPEndPoint)clientInfo.RemoteSocket.RemoteEndPoint).Address}<br>";
                 
                    if (MUCOServerNetworkManager.Instance.Server.ClientStatistics.ContainsKey(clientInfo.UniqueIdentifier))
                    {
                        MUCONet.MUCOServer.MUCOClientStatistics clientStatistics = MUCOServerNetworkManager.Instance.Server.ClientStatistics[clientInfo.UniqueIdentifier];
                        userDataText += $"    • Packets Sent(to): {clientStatistics.PacketsSent}<br>";
                        userDataText += $"    • Packets Received(from): {clientStatistics.PacketsReceived}<br>";
                    }
                }
            }

            m_UserColumn.SetText(userDataText);
        }
    }
}