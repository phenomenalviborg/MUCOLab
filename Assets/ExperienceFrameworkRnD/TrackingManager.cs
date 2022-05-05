using System.Linq;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class TrackingManager : PhenomenalViborg.MUCOSDK.IManager
    {
        [SerializeField] private string m_EnvironmentCode;

        private Antilatency.SDK.DeviceNetwork m_DeviceNetwork;
        private Antilatency.SDK.AltEnvironmentCode m_Environment;
        private Antilatency.SDK.AltEnvironmentMarkersDrawer m_AltEnvironmentMarkersDrawer;

        private Antilatency.Alt.Tracking.ILibrary m_TrackingLibrary;

        private Antilatency.DeviceNetwork.NodeHandle m_AdminNodeHandle;
        private Antilatency.DeviceNetwork.NodeHandle m_UserNodeHandle;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            m_DeviceNetwork = gameObject.AddComponent<Antilatency.SDK.DeviceNetwork>();
            
            m_Environment = gameObject.AddComponent<Antilatency.SDK.AltEnvironmentCode>();
            m_Environment.EnvironmentCode = m_EnvironmentCode;

            m_AltEnvironmentMarkersDrawer = gameObject.AddComponent<Antilatency.SDK.AltEnvironmentMarkersDrawer>();
            m_AltEnvironmentMarkersDrawer.Environment = m_Environment;

            m_TrackingLibrary = Antilatency.Alt.Tracking.Library.load();
            if (m_TrackingLibrary == null)
            {
                Debug.LogError("Failed to create tracking library");
                return;
            }

            m_AdminNodeHandle = new Antilatency.DeviceNetwork.NodeHandle();
            m_UserNodeHandle = new Antilatency.DeviceNetwork.NodeHandle();
        }

        private void Start()
        {
            m_AdminNodeHandle = GetIdleTrackerNodesBySocketTag("Admin")[0];
            m_UserNodeHandle = GetUsbConnectedIdleIdleTrackerNodesBySocketTag("User")[0];

            Debug.Log($"AdminNodeHandle: {m_AdminNodeHandle}");
            Debug.Log($"UserNodeHandle: {m_UserNodeHandle}");
        }

        protected Antilatency.DeviceNetwork.NodeHandle[] GetUsbConnectedIdleIdleTrackerNodesBySocketTag(string socketTag)
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = m_DeviceNetwork.NativeNetwork;
            if (nativeNetwork == null)
            {
                return new Antilatency.DeviceNetwork.NodeHandle[0];
            }

            using (Antilatency.Alt.Tracking.ITrackingCotaskConstructor cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetParent(nativeNetwork.nodeGetParent(v)) == Antilatency.DeviceNetwork.NodeHandle.Null &&
                        nativeNetwork.nodeGetStringProperty(nativeNetwork.nodeGetParent(v), "Tag") == socketTag &&
                        nativeNetwork.nodeGetStatus(v) == Antilatency.DeviceNetwork.NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }

        private Antilatency.DeviceNetwork.NodeHandle[] GetIdleTrackerNodesBySocketTag(string socketTag)
        {
            Antilatency.DeviceNetwork.INetwork nativeNetwork = m_DeviceNetwork.NativeNetwork;
            if (nativeNetwork == null)
            {
                return new Antilatency.DeviceNetwork.NodeHandle[0];
            }

            using (Antilatency.Alt.Tracking.ITrackingCotaskConstructor cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetStringProperty(nativeNetwork.nodeGetParent(v), "Tag") == socketTag &&
                        nativeNetwork.nodeGetStatus(v) == Antilatency.DeviceNetwork.NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }
    }
}
