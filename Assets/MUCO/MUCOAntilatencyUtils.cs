using System.Collections;
using System.Collections.Generic;
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
        /// <param name="network">The active network.</param>
        /// <param name="calculatedPath">This field should only be specified from inside the function.</param>
        /// <returns></returns>
        public static string GetFullNodePathRecursive(NodeHandle nodeHandle, INetwork network, string calculatedPath = "", bool serialSuffix = false)
        {
            NodeHandle parent = network.nodeGetParent(nodeHandle);
            string newPath = network.nodeGetStringProperty(nodeHandle, "sys/HardwareName") + (serialSuffix ? "-" + network.nodeGetStringProperty(nodeHandle, "sys/HardwareSerialNumber") : "") + (calculatedPath != "" ? "/" : "") + calculatedPath;

            if (parent == NodeHandle.Null)
            {
                return newPath;
            }
            else
            {
                return GetFullNodePathRecursive(parent, network, newPath, serialSuffix);
            }
        }
    }
}