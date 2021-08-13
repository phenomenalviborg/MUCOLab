#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

using PhenomenalViborg.MUCO.Networking;

namespace PhenomenalViborg.MUCO
{
    public class MUCOServerEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("MUCO/Dedicated Server")]
        private static void OpenWindow()
        {
            MUCOServerEditorWindow window = GetWindow<MUCOServerEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
            window.titleContent = new GUIContent("MUCO | Dedicated Server");
        }

        private void StartServer()
        {
            MUCOServer.ServerLog += OnServerLog;
            MUCOServer.StartServer(32, 26950);
        }

        private static void OnServerLog(String msg)
        {
            Debug.Log($"[SERVER] {msg}");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            tree.Add("Server", new ServerTab(this));

            return tree;
        }

        public class ServerTab
        {
            private MUCOServerEditorWindow m_ParentEditor;

            [Button("Start Server")]
            private void OnStartServerButtonPressed()
            {
                m_ParentEditor.StartServer();
            }

            public ServerTab(MUCOServerEditorWindow parentEditor)
            {
                m_ParentEditor = parentEditor;
            }
        }
    }
}
#endif
