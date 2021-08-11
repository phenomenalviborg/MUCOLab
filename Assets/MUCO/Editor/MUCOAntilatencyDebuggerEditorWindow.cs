#if UNITY_EDITOR
namespace PhenomenalViborg
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEditor;

    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;

    using Antilatency.DeviceNetwork;

    public class MUCOAntilatencyDebuggerEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("MUCO/Antilatency Debugger")]
        private static void OpenWindow()
        {
            MUCOAntilatencyDebuggerEditorWindow window = GetWindow<MUCOAntilatencyDebuggerEditorWindow>();
            // Nifty little trick to quickly position the window in the middle of the editor.
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
            window.titleContent = new GUIContent("MUCO | Antilatency Debugger");
        }

        private ILibrary m_Library = null;
        private INetwork m_NativeNetwork = null;
        private uint m_LastUpdateId = 0;

        private double m_TimeAtLastRefresh = 0.0f;
        private float m_RefreshRate = 1.0f;

        private List<AntilatencyDeviceInfo> m_AntilatencyDeviceInfoStructs = new List<AntilatencyDeviceInfo>();
        // TODO: Consider moveing this to MUCOAntilatencyUtils.
        private struct AntilatencyDeviceInfo
        {
            [ReadOnly] public string Path;

            [Header("System")]
            [ReadOnly] public string HardwareName;
            [ReadOnly] public string HardwareVersion;
            [ReadOnly] public string HardwareSerialNumber;
            [ReadOnly] public string FirmwareName;
            [ReadOnly] public string FirmwareVersion;

            [Header("AltTag Specific")]
            [ShowIf("@this.HardwareName == \"AltTag\"")]
            [ReadOnly] public string Tag;
            [ReadOnly] public string ChannelsMask;

            public AntilatencyDeviceInfo(NodeHandle nodeHandle, INetwork network)
            {
                Path = MUCOAntilatencyUtils.GetFullNodePathRecursive(nodeHandle, network, "", true);
                HardwareName = network.nodeGetStringProperty(nodeHandle, "sys/HardwareName");
                HardwareVersion = network.nodeGetStringProperty(nodeHandle, "sys/HardwareVersion");
                HardwareSerialNumber = network.nodeGetStringProperty(nodeHandle, "sys/HardwareSerialNumber");
                FirmwareName = network.nodeGetStringProperty(nodeHandle, "sys/FirmwareName");
                FirmwareVersion = network.nodeGetStringProperty(nodeHandle, "sys/FirmwareVersion");

                Tag = "";
                ChannelsMask = "";

                switch (HardwareName)
                {
                    case "AltTag":
                        Tag = network.nodeGetStringProperty(nodeHandle, "Tag");
                        ChannelsMask = network.nodeGetStringProperty(nodeHandle, "ChannelsMask");
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected void Awake()
        {
            InitializeAntilatencyDeviceNetwork();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            TerminateAntilatencyDeviceNetwork();

        }

        protected void OnInspectorUpdate()
        {
            if (EditorApplication.timeSinceStartup - m_RefreshRate > m_TimeAtLastRefresh)
            {
                m_TimeAtLastRefresh = EditorApplication.timeSinceStartup;
                Refresh(false); // Soft refresh
            }

            UpdateAntilatencyDeviceNetwork();
        }

        private void Refresh(bool hardRefresh = false)
        {
            // Refresh m_AntilatencyDevices
            if (hardRefresh)
            {
                TerminateAntilatencyDeviceNetwork();
                InitializeAntilatencyDeviceNetwork();
            }

            if (m_NativeNetwork != null)
            {
                NodeHandle[] nodes = m_NativeNetwork.getNodes();
                if (hardRefresh || nodes.Length != m_AntilatencyDeviceInfoStructs.Count)
                {
                    m_AntilatencyDeviceInfoStructs.Clear();

                    HashSet<string> nodePaths = new HashSet<string>();
                    foreach (NodeHandle node in nodes)
                    {
                        m_AntilatencyDeviceInfoStructs.Add(new AntilatencyDeviceInfo(node, m_NativeNetwork));
                    }
                }
            }
            else
            {
                m_AntilatencyDeviceInfoStructs.Clear();
            }

            ForceMenuTreeRebuild();
        }

        private void InitializeAntilatencyDeviceNetwork()
        {
            // Initialize m_Library and m_NativeNetwork
            m_Library = Antilatency.DeviceNetwork.Library.load();
            if (m_Library == null)
            {
                Debug.LogError("Failed to load Antilatency Device Network library");
                return;
            }

            m_Library.setLogLevel(LogLevel.Info);

            var deviceFilter = m_Library.createFilter();
            UsbDeviceFilter[] supportedUsbDeviceTypes = new UsbDeviceFilter[] { new UsbDeviceFilter { vid = UsbVendorId.Antilatency, pid = 0x0000 } };
            foreach (var deviceType in supportedUsbDeviceTypes)
            {
                deviceFilter.addUsbDevice(deviceType);
            }

            m_NativeNetwork = m_Library.createNetwork(deviceFilter);

            if (m_NativeNetwork == null)
            {
                Debug.LogError("Failed to create Antilatency Device Network");
            }

            Refresh();
        }

        private void TerminateAntilatencyDeviceNetwork()
        {
            if (m_NativeNetwork != null)
            {
                m_NativeNetwork.Dispose();
                m_NativeNetwork = null;
            }

            if (m_Library != null)
            {
                m_Library.Dispose();
                m_Library = null;
            }

            Refresh();
        }

        private void UpdateAntilatencyDeviceNetwork()
        {
            if (m_NativeNetwork == null)
            {
                return;
            }

            var updateId = m_NativeNetwork.getUpdateId();
            if (updateId != m_LastUpdateId)
            {
                m_LastUpdateId = updateId;
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.Add("Settings", new Settings(this));

            foreach (AntilatencyDeviceInfo antilatencyDeviceNodeInfo in m_AntilatencyDeviceInfoStructs)
            {
                tree.Add("ConnectedDevices/" + antilatencyDeviceNodeInfo.Path, antilatencyDeviceNodeInfo);
            }

            return tree;
        }

        public class Settings
        {
            [Button("Refresh")]
            void OnRefreshButtonPressed()
            {
                m_ParentEditor.Refresh(true);
            }

            [EnableIf("@m_ParentEditor.m_NativeNetwork == null")]
            [Button("Initialize Antilatency Device Network")]
            void OnInitializeAntilatencyButtonPressed()
            {
                m_ParentEditor.InitializeAntilatencyDeviceNetwork();
            }

            [EnableIf("@m_ParentEditor.m_NativeNetwork != null")]
            [Button("Terminate Antilatency Device Network")]
            void OnTerminateAntilatencyRefreshButtonPressed()
            {
                m_ParentEditor.TerminateAntilatencyDeviceNetwork();
                
            }

            private MUCOAntilatencyDebuggerEditorWindow m_ParentEditor;

            public Settings(MUCOAntilatencyDebuggerEditorWindow parentEditor)
            {
                m_ParentEditor = parentEditor;
            }
        }
    }
}
#endif
