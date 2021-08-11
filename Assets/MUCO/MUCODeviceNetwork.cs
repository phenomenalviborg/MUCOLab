using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Antilatency.SDK;
using Antilatency.DeviceNetwork;

namespace PhenomenalViborg
{
    public class MUCODeviceNetwork : DeviceNetwork
    {
        [SerializeField] private bool m_Debug = false;

        private void OnGUI()
        {
            if (m_Debug)
            {
                GUILayout.BeginArea(new Rect(10, 10, 200, 600));
                GUILayout.BeginVertical();

                NodeHandle[] nodes = NativeNetwork.getNodes();
                GUILayout.Box("Device count:" + nodes.Length);
                GUILayout.Space(8);
                foreach (NodeHandle node in nodes)
                {
                    GUILayout.Label(NativeNetwork.nodeGetPhysicalPath(node));
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
    }
}

