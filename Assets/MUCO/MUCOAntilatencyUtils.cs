using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Antilatency.DeviceNetwork;

namespace PhenomenalViborg.MUCO
{
    public static class MUCOAntilatencyUtils
    {
        /// <summary>
        /// Recursively calculates the path for a given NodeHandle.
        /// </summary>
        /// <param name="nodeHandle"></param>
        /// <param name="deviceNetwork">The active network.</param>
        /// <param name="calculatedPath">This field should only be specified from inside the function.</param>
        /// <returns>The full path of the speficied nodeHandle using HardwareNames</returns>
        public static string GetFullNodePathRecursive(NodeHandle nodeHandle, INetwork deviceNetwork, string calculatedPath = "", bool serialSuffix = false)
        {
            NodeHandle parent = deviceNetwork.nodeGetParent(nodeHandle);
            string newPath = deviceNetwork.nodeGetStringProperty(nodeHandle, "sys/HardwareName") + (serialSuffix ? "-" + deviceNetwork.nodeGetStringProperty(nodeHandle, "sys/HardwareSerialNumber") : "") + (calculatedPath != "" ? "/" : "") + calculatedPath;

            if (parent == NodeHandle.Null)
            {
                return newPath;
            }
            else
            {
                return GetFullNodePathRecursive(parent, deviceNetwork, newPath, serialSuffix);
            }
        }

        /// <summary>
        /// Searches for idle tracking nodes which socket is marked with <paramref name="socketTag"/>.
        /// </summary>
        /// <param name="socketTag">Socket "tag" property value.</param>
        /// <returns>The array of idle tracking nodes connected to sockets marked with <paramref name="socketTag"/>.</returns>
        public static NodeHandle[] GetIdleTrackerNodesBySocketTag(string socketTag, INetwork deviceNetwork, Antilatency.Alt.Tracking.ILibrary trackingLibrary)
        {
            if (deviceNetwork == null)
            {
                Debug.LogError("DeviceNetwork was null.");
                return new NodeHandle[0];
            }

            if (trackingLibrary == null)
            {
                Debug.LogError("TrackingLibrary was null.");
                return new NodeHandle[0];
            }

            Antilatency.Alt.Tracking.ITrackingCotaskConstructor cotaskConstructor = trackingLibrary.createTrackingCotaskConstructor();
            NodeHandle[] nodes = cotaskConstructor.findSupportedNodes(deviceNetwork);
            nodes = nodes.Where(v => deviceNetwork.nodeGetStringProperty(deviceNetwork.nodeGetParent(v), "Tag") == socketTag && deviceNetwork.nodeGetStatus(v) == NodeStatus.Idle).ToArray();

            return nodes;
        }

        /// <summary>
        /// Searches for the idle tracking node which socket is <paramref name="socketTag"/>.
        /// </summary>
        /// <param name="socketTag">Socket "tag" property value.</param>
        /// <returns>The first idle tracking nodes connected to socket marked with <paramref name="socketTag"/>.</returns>
        public static NodeHandle GetFirstIdleTrackerNodeBySocketTag(string socketTag, INetwork deviceNetwork, Antilatency.Alt.Tracking.ILibrary trackingLibrary)
        {
            if (deviceNetwork == null)
            {
                Debug.LogError("DeviceNetwork was null.");
                return new NodeHandle();
            }

            if (trackingLibrary == null)
            {
                Debug.LogError("TrackingLibrary was null.");
                return new NodeHandle();
            }

            NodeHandle[] nodes = MUCOAntilatencyUtils.GetIdleTrackerNodesBySocketTag(socketTag, deviceNetwork, trackingLibrary);

            if (nodes.Length == 0)
            {
                return new NodeHandle();
            }

            return nodes[0];
        }
    }
}