using Antilatency.DeviceNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PhenomenalViborg.MUCO
{
    public class MUCOTrackerTag : MUCOTracker
    {
        [Header("Tag")]
        [SerializeField] private string HardwareTag;
        
        protected override NodeHandle GetAvailableTrackingNode()
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

            Antilatency.Alt.Environment.IEnvironment environment = m_MUCOManager.GetEnvironment();
            if (environment == null)
            {
                Debug.LogError("Environment was null!");
                return new NodeHandle();
            }

            return MUCOAntilatencyUtils.GetFirstIdleTrackerNodeBySocketTag(HardwareTag, deviceNetwork, m_TrackingLibrary);
        }

        protected override Pose GetPlacement()
        {
            return new Pose(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }

        /// <summary>
        /// Apply tracking data to a component's GameObject transform.
        /// </summary>
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