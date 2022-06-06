using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhenomenalViborg.MUCOSDK;
using PhenomenalViborg.MUCONet;

public class NetworkSample : MonoBehaviour
{
    public enum EMyPacketIdentifier : System.UInt32
    {
        MulticastSample,
        UnicastSample
    }

    private bool m_StaticallyInitialized = false;

    private void Awake()
    {
        if (!m_StaticallyInitialized)
        {
            ClientNetworkManager.GetInstance().Client.RegisterPacketHandler((System.UInt16)EMyPacketIdentifier.MulticastSample, HandleMulticastSample);
            ClientNetworkManager.GetInstance().Client.RegisterPacketHandler((System.UInt16)EMyPacketIdentifier.UnicastSample, HandleUnicastSample);
            m_StaticallyInitialized = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            using (MUCOPacket packet = new MUCOPacket((System.UInt16)EMyPacketIdentifier.MulticastSample))
            {
                packet.WriteString("Multicast payload");
                ClientNetworkManager.GetInstance().SendReplicatedMulticastPacket(packet);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NetworkUser receiver = ClientNetworkManager.GetInstance().GetNetworkUsers()[0];
            using (MUCOPacket packet = new MUCOPacket((System.UInt16)EMyPacketIdentifier.UnicastSample))
            {
                packet.WriteString("Unicast payload");
                ClientNetworkManager.GetInstance().SendReplicatedUnicastPacket(packet, receiver);
            }
        }
    }

    private static void HandleMulticastSample(MUCOPacket packet)
    {
        string payload = packet.ReadString();
        Debug.Log($"I recived the sample multicast packet! Payload: {payload}");
    }

    private static void HandleUnicastSample(MUCOPacket packet)
    {
        string payload = packet.ReadString();
        Debug.Log($"I recived the sample unicast packet! Payload: {payload}");
    }
}
