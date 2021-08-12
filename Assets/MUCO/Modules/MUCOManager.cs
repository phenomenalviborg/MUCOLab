using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

using Antilatency.DeviceNetwork;
using Antilatency.Alt.Environment;

namespace PhenomenalViborg.MUCO
{
    public class MUCOManager : MonoBehaviour
    {
        [Header("DeviceNetwork")]
        private Antilatency.DeviceNetwork.INetwork m_DeviceNetwork = null;
        private Antilatency.DeviceNetwork.ILibrary m_DeviceNetworkLibrary = null;
        private uint m_LastUpdateId = 0;
        public UnityEvent OnDeviceNetworkChanged = new UnityEvent();

        [Header("Environment")]
        public string EnvironmentCode = null;
        [SerializeField] private bool m_DrawEnvironmentMarkers = true;
        [ShowIf("m_DrawEnvironmentMarkers")] [SerializeField] private GameObject m_EnvironmentMarkerPrefab = null;
        private Antilatency.Alt.Environment.IEnvironment m_Environment = null;
        private Antilatency.Alt.Environment.Selector.ILibrary m_EnvironmentSelectorILibrary = null;

        public Antilatency.DeviceNetwork.INetwork GetDeviceNetwork()
        {
            if (m_DeviceNetwork == null)
            {
                Debug.Log("Device network was null! Make sure the device network has been initalized before trying to access it!");
                return null;
            }

            return m_DeviceNetwork;
        }

        public Antilatency.Alt.Environment.IEnvironment GetEnvironment()
        {
            if (m_Environment == null)
            {
                Debug.Log("Environment was null! Make sure the environment has been initalized before trying to access it!");
                return null;
            }

            return m_Environment;
        }

        private void Awake()
        {
            MUCOManager[] managers = Object.FindObjectsOfType<MUCOManager>();
            if (managers.Length > 1)
            {
                Debug.LogError("Multiple MUCOManagers detected!");
                return;
            }

            InitializeAntilatencyDeviceNetwork();
            InitializeAntilatencyEnvironment();
            if (m_DrawEnvironmentMarkers) DrawAntilatencyEnvironmentMarkers();
        }

        private void Update()
        {
            Debug.Log(m_DeviceNetwork.getNodes().Length);
            UpdateAntilatencyDeviceNetwork();
        }

        private void OnDestroy()
        {
            TerminateAntilatencyDeviceNetwork();
        }

        // AntilatencyDeviceNetwork
        private void InitializeAntilatencyDeviceNetwork()
        {
            m_DeviceNetworkLibrary = Antilatency.DeviceNetwork.Library.load();
            if (m_DeviceNetworkLibrary == null)
            {
                Debug.LogError("Failed to load Antilatency Device Network library!");
                return;
            }
            m_DeviceNetworkLibrary.setLogLevel(Antilatency.DeviceNetwork.LogLevel.Info);

            Antilatency.DeviceNetwork.IDeviceFilter deviceFilter = m_DeviceNetworkLibrary.createFilter();
            deviceFilter.addUsbDevice(new Antilatency.DeviceNetwork.UsbDeviceFilter { vid = Antilatency.DeviceNetwork.UsbVendorId.Antilatency, pid = 0x0000 });

            m_DeviceNetwork = m_DeviceNetworkLibrary.createNetwork(deviceFilter);

            if (m_DeviceNetwork == null)
            {
                Debug.LogError("Failed to create Antilatency Device Network!");
                return;
            }
        }

        private void UpdateAntilatencyDeviceNetwork()
        {
            if (m_DeviceNetwork == null)
            {
                return;
            }

            var updateId = m_DeviceNetwork.getUpdateId();
            if (updateId != m_LastUpdateId)
            {
                m_LastUpdateId = updateId;
                OnDeviceNetworkChanged.Invoke();
            }
        }

        private void TerminateAntilatencyDeviceNetwork()
        {
            if (m_DeviceNetwork != null)
            {
                m_DeviceNetwork.Dispose();
                m_DeviceNetwork = null;
            }

            if (m_DeviceNetwork != null)
            {
                m_DeviceNetwork.Dispose();
                m_DeviceNetwork = null;
            }
        }

        // AntilatencyEnvironment
        private void InitializeAntilatencyEnvironment()
        {
            if (m_Environment != null)
            {
                Debug.LogError("An envrionment already exists.");
                return; 
            }

            m_EnvironmentSelectorILibrary = Antilatency.Alt.Environment.Selector.Library.load();
            if (m_EnvironmentSelectorILibrary == null)
            {
                Debug.LogError("Failed to Alt Environment Selector library.");
                return;
            }

            m_Environment = m_EnvironmentSelectorILibrary.createEnvironment(EnvironmentCode);
            if (m_Environment == null)
            {
                Debug.LogError("Failed to create environment.");
                return;
            }
        }

        private void DrawAntilatencyEnvironmentMarkers()
        {

            if (m_Environment == null)
            {
                Debug.LogError("Failed to draw markers, environment doesn't exist.");
                return;
            }

            if (m_EnvironmentMarkerPrefab == null)
            {
                Debug.LogError("Marker perfab is not assigned.");
                return;
            }


            Vector3[] markerPositions = m_Environment.getMarkers();
            foreach (Vector3 markerPosition in markerPositions)
            {
                GameObject marker = Instantiate(m_EnvironmentMarkerPrefab, markerPosition, Quaternion.identity, this.transform);
            }
        }
    }
}