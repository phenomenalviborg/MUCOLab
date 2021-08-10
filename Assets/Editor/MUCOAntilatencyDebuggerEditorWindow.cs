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

        private ILibrary m_Library;
        private INetwork m_NativeNetwork;
        private uint m_LastUpdateId = 0;

        private double m_TimeAtLastRefresh = 0.0f;
        private float m_RefreshRate = 1.0f;

        private List<AntilatencyDeviceInfo> m_AntilatencyDeviceInfoStructs = new List<AntilatencyDeviceInfo>();
        // TODO: Consider moveing this to MUCOAntilatencyUtils.
        private struct AntilatencyDeviceInfo
        {
            [ReadOnly]
            public string fullPath;

            [ReadOnly]
            public string name;

            public AntilatencyDeviceInfo(NodeHandle nodeHandle, INetwork network)
            {
                fullPath = MUCOAntilatencyUtils.GetFullNodePathRecursive(nodeHandle, network);
                name = network.nodeGetStringProperty(nodeHandle, "sys/HardwareName");
            }
        }

        public void Refresh(bool hardRefresh = false)
        {
            // Refresh m_AntilatencyDevices
            NodeHandle[] nodes = m_NativeNetwork.getNodes();
            if (hardRefresh || nodes.Length != m_AntilatencyDeviceInfoStructs.Count)
            {
                m_AntilatencyDeviceInfoStructs.Clear();
                m_AntilatencyDeviceInfoStructs = new List<AntilatencyDeviceInfo>();

                HashSet<string> nodePaths = new HashSet<string>();
                foreach (NodeHandle node in nodes)
                {
                    m_AntilatencyDeviceInfoStructs.Add(new AntilatencyDeviceInfo(node, m_NativeNetwork));
                }
            }

            if (hardRefresh)
            {
                TerminateAntilatency();
                InitializeAntilatency();
            }

            ForceMenuTreeRebuild();
        }

        protected override void Initialize()
        {
            base.Initialize();

            InitializeAntilatency();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            TerminateAntilatency();
        }

        protected void OnInspectorUpdate()
        {
            if (EditorApplication.timeSinceStartup - m_RefreshRate > m_TimeAtLastRefresh)
            {
                m_TimeAtLastRefresh = EditorApplication.timeSinceStartup;
                Refresh(false); // Soft refresh
            }

            UpdateAntilatency();
        }

        private void InitializeAntilatency()
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
        }

        private void TerminateAntilatency()
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
        }

        private void UpdateAntilatency()
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
                tree.Add("ConnectedDevices/"+antilatencyDeviceNodeInfo.fullPath, antilatencyDeviceNodeInfo);
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

            private MUCOAntilatencyDebuggerEditorWindow m_ParentEditor;

            public Settings(MUCOAntilatencyDebuggerEditorWindow parentEditor)
            {
                m_ParentEditor = parentEditor;
            }
        }
    }
}
#endif
