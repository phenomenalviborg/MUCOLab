using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using Antilatency.DeviceNetwork;

namespace PhenomenalViborg.MUCO
{
    public abstract class MUCOTracker : MonoBehaviour
    {
        protected MUCOManager m_MUCOManager = null;

        [Header("Tracker")]
        public UnityEvent<bool> OnTrackingTaskStateChanged = new UnityEvent<bool>();
        protected Antilatency.Alt.Tracking.ILibrary m_TrackingLibrary = null;
        [SerializeField] private float ExtrapolationTime = 0.0f;
        private Antilatency.DeviceNetwork.NodeHandle m_TrackingNode = new NodeHandle();
        private Antilatency.Alt.Tracking.ITrackingCotask m_TrackingCotask = null;
        private UnityEngine.Pose m_TrackingPose = new UnityEngine.Pose(); // I assume this is just unitys representation of a transformation matrix?

        private void Start()
        {
            InitializeAntilatencyTacking();
            OnDeviceNetworkChanged();

        }

        protected virtual void Update()
        {
            if (m_TrackingCotask != null && m_TrackingCotask.isTaskFinished())
            {
                StopTrackingTask();
            }
        }

        private void OnDeviceNetworkChanged()
        {
            if (m_TrackingCotask != null)
            {
                if (m_TrackingCotask.isTaskFinished())
                {
                    StopTrackingTask();
                }
                else
                {
                    return;
                }
            }

            if (m_TrackingCotask == null)
            {
                var node = GetAvailableTrackingNode();
                if (node != Antilatency.DeviceNetwork.NodeHandle.Null)
                {
                    StartTrackingTask(node);
                }
            }
        }

        private void StartTrackingTask(NodeHandle node)
        {
            if (m_MUCOManager == null)
            {
                Debug.LogError("MUCOManager was null");
                return;
            }

            Antilatency.DeviceNetwork.INetwork deviceNetwork = m_MUCOManager.GetDeviceNetwork();
            if (deviceNetwork == null) 
            {
                Debug.LogError("DeviceNetwork was null!");
                return;
            }

            Antilatency.Alt.Environment.IEnvironment environment = m_MUCOManager.GetEnvironment();
            if (environment == null)
            {
                Debug.LogError("Environment was null!");
                return;
            }

            if (deviceNetwork.nodeGetStatus(node) != NodeStatus.Idle)
            {
                Debug.LogError("Specified node is not in the Idle state");
                return;
            }


            m_TrackingPose = GetPlacement();

            using (var cotaskConstructor = m_TrackingLibrary.createTrackingCotaskConstructor())
            {
                m_TrackingCotask = cotaskConstructor.startTask(deviceNetwork, node, environment);

                if (m_TrackingCotask == null)
                {
                    StopTrackingTask();
                    Debug.LogWarning("Failed to start tracking task on node " + node.value);
                    return;
                }

                m_TrackingNode = node;
                OnTrackingTaskStateChanged.Invoke(true);
            }
        }

        public bool GetTrackingState(out Antilatency.Alt.Tracking.State state)
        {
            state = new Antilatency.Alt.Tracking.State();
            if (m_TrackingCotask == null)
            {
                return false;
            }

            state = m_TrackingCotask.getExtrapolatedState(m_TrackingPose, ExtrapolationTime);
            if (state.stability.stage == Antilatency.Alt.Tracking.Stage.InertialDataInitialization)
            {
                return false;
            }
            return true;
        }


        private void StopTrackingTask()
        {
            if (m_TrackingCotask == null)
            {
                return;
            }

            m_TrackingCotask.Dispose();
            m_TrackingCotask = null;
            m_TrackingNode = new NodeHandle();

            OnTrackingTaskStateChanged.Invoke(false);
        }

        private void InitializeAntilatencyTacking()
        {
            m_TrackingNode = new Antilatency.DeviceNetwork.NodeHandle();

            m_MUCOManager = Object.FindObjectOfType<MUCOManager>();
            if (m_MUCOManager == null)
            {
                Debug.LogError("Failed to find MUCOManager.");
                return;
            }

            m_TrackingLibrary = Antilatency.Alt.Tracking.Library.load();
            if (m_TrackingLibrary == null) {
                Debug.LogError("Failed to create tacking library.");
                return;
            }

            m_MUCOManager.OnDeviceNetworkChanged.AddListener(OnDeviceNetworkChanged);
        }

        private void TerminateAntilatencyTacking()
        {
            if (m_TrackingCotask != null)
            {
                m_TrackingCotask.Dispose();
                m_TrackingCotask = null;
            }

            if (m_TrackingLibrary != null)
            {
                m_TrackingLibrary.Dispose();
                m_TrackingLibrary = null;
            }

            m_MUCOManager.OnDeviceNetworkChanged.RemoveListener(OnDeviceNetworkChanged);
        }

        // To be implimented by inherited class
        protected abstract Antilatency.DeviceNetwork.NodeHandle GetAvailableTrackingNode();
        protected abstract UnityEngine.Pose GetPlacement();
    }
}
