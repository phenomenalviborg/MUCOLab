using System;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class ServerGUI : MonoBehaviour
    {
        private string m_PortString = "4960";

        private void OnGUI()
        {
            /*GUILayout.BeginVertical();
            GUILayout.Label(string.Format("<b>Server</b>\n" +
                $"Address: {Server.GetAddress()}\n" +
                $"Port: {Server.GetPort()}\n" +
                $"Active Connections: {Server.GetConnectionCount()}\n" +
                $"Packets Sent: {Server.GetPacketsSendCount()}\n" +
                $"Packets Received: {Server.GetPacketsReceivedCount()}\n"));

            foreach (MUCOServer.MUCORemoteClient clientInfo in Server.ClientInfo.Values)
            {
                GUILayout.Label(string.Format($"<b>Client {clientInfo.UniqueIdentifier}</b>\n" +
                    $"Address: {clientInfo.GetAddress()}\n" +
                    $"Port: {clientInfo.GetPort()}\n" +
                    $"Packets Sent: {Server.ClientStatistics[clientInfo.UniqueIdentifier].PacketsSent}\n" +
                    $"Packets Received: {Server.ClientStatistics[clientInfo.UniqueIdentifier].PacketsReceived}\n"));
            }
            
            GUILayout.EndVertical();*/
            GUILayout.Window(0, new Rect(20, 20, Screen.width - 40, Screen.height - 40), RenderServerWindow, "Server"); ;
        }

        private void RenderServerWindow(int windowID)
        {
            GUI.enabled = !ServerNetworkManager.GetInstance().IsStarted();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Server"))
            {

                if (Int32.TryParse(m_PortString, out int port))
                {
                    ServerNetworkManager.GetInstance().StartServer(port);
                }
                else
                {
                    Debug.LogError("Port string could not be parsed.");
                }
            }
            m_PortString = GUILayout.TextField(m_PortString, 8);
            GUILayout.EndHorizontal();



            GUI.enabled = ServerNetworkManager.GetInstance().IsStarted();
            if (GUILayout.Button("Stop Server"))
            {
                ServerNetworkManager.GetInstance().StopServer();
            }
        }
    }
}