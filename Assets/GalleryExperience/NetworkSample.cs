
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhenomenalViborg.MUCOSDK;
using PhenomenalViborg.MUCONet;

using PhenomenalViborg.Networking; // TMP

public class NetworkSample : MonoBehaviour
{
    public enum EMyPacketIdentifier : System.UInt32
    {
        MulticastSample,
        UnicastSample
    }

    private bool m_StaticallyInitialized = false;





    // API sample: Replicated variable.
    private ReplicatedVariable<float> m_MyReplicatedFloat = 132.123f; // Is Replicated<float> better?

    // API sample: Replicated event.
    private ReplicatedEvent m_MyReplicatedEvent;

    // API sample: Replicated event with custom arguments.
    public class MyCustomEventArgs : System.EventArgs
    {
        public MyCustomEventArgs(float a, int b)
        {
            this.A = a;
            this.B = b;
        }

        public float A;
        public int B;
    }

    private ReplicatedEvent<MyCustomEventArgs> m_MyOtherReplicatedEvent;





    private void Awake()
    {
        if (!m_StaticallyInitialized)
        {
            ClientNetworkManager.GetInstance().Client.RegisterPacketHandler((System.UInt16)EMyPacketIdentifier.MulticastSample, HandleMulticastSample);
            ClientNetworkManager.GetInstance().Client.RegisterPacketHandler((System.UInt16)EMyPacketIdentifier.UnicastSample, HandleUnicastSample);
            m_StaticallyInitialized = true;
        }

        // Replicated variable demo
        m_MyReplicatedFloat = 123.45f;
        m_MyReplicatedFloat += 0.5f;
        if (m_MyReplicatedFloat > 2.0f)
        {
            Debug.Log("m_MyReplicatedFloat is greater than 2.0f!");
        }

        Debug.Log(m_MyReplicatedFloat);
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
            NetworkUser receiver = ClientNetworkManager.GetInstance().GetLocalNetworkUser();
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
