using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Antilatency.DeviceNetwork;

namespace PhenomenalViborg.MUCO
{
    public class MUCOTrackerUSB : MUCOTracker
    {
        protected override Antilatency.DeviceNetwork.NodeHandle GetAvailableTrackingNode()
        {
            if (m_MUCOManager == null)
            {
                Debug.LogError("MUCOManager was null");
                return new NodeHandle();
            }

            Antilatency.DeviceNetwork.INetwork deviceNetwork = m_MUCOManager.GetDeviceNetwork();
            if (deviceNetwork == null)
            {
                Debug.LogError("DeviceNetwork was null!");
                return new NodeHandle();
            }
            var s = MUCOAntilatencyUtils.GetUsbConnectedFirstIdleTrackerNode(deviceNetwork, m_TrackingLibrary);
            return MUCOAntilatencyUtils.GetUsbConnectedFirstIdleTrackerNode(deviceNetwork, m_TrackingLibrary);
        }

        protected override UnityEngine.Pose GetPlacement()
        {
            UnityEngine.Pose placement = new UnityEngine.Pose();
            placement = m_TrackingLibrary.createPlacement("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            return placement;
        }

        protected override void Update()
        {
            base.Update();

            Antilatency.Alt.Tracking.State trackingState;

            if (!GetTrackingState(out trackingState))
            {
                return;
            }

            transform.localPosition = trackingState.pose.position;
            transform.localRotation = trackingState.pose.rotation;
        }
    }
}