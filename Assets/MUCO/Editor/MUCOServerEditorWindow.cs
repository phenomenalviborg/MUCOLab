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
    [ExecuteAlways]
    public class MUCOServerEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("MUCO/Dedicated Server")]
        private static void OpenWindow()
        {
            MUCOServerEditorWindow window = GetWindow<MUCOServerEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
            window.titleContent = new GUIContent("MUCO | Dedicated Server");
        }

        private void Update()
        {
            // TODO: Create new networking thread "part 2"
            MUCOThreadManagerNONMONO.UpdateMain();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();

            tree.Add("Server", new ServerTab());

            return tree;
        }

        public class ServerTab
        {
            private static ServerTab s_Instnace = null;
            private OdinMenuTree m_ConsoleTree = null;

            [Button("Start Server")]
            private void OnStartServerButtonPressed()
            {
                s_Instnace = this;
                MUCOServer.ServerLog += OnServerConsoleWriteLine;
                MUCOServer.StartServer(32, 26950);
            }

            [Button("Stop Server")]
            private void OnStopServerButtonPressed()
            {
                MUCOServer.StopServer();
            }

            private static void OnServerConsoleWriteLine(String msg)
            {
                s_Instnace.ConsoleWriteLine(msg);
            }

            public void ConsoleWriteLine(string msg)
            {
                OdinMenuItem newOdinMenuItem = new OdinMenuItem(m_ConsoleTree, msg, null);
                newOdinMenuItem.Icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Sirenix/Odin Inspector/Assets/Editor/Odin Inspector Logo.png", typeof(Texture2D));
                m_ConsoleTree.MenuItems.Add(newOdinMenuItem);

                m_ConsoleTree.MarkDirty();
            }

            [OnInspectorGUI]
            private void DrawTree()
            {
                if (m_ConsoleTree == null)
                {
                    m_ConsoleTree = new OdinMenuTree();
                    m_ConsoleTree.Config.EXPERIMENTAL_INTERNAL_DrawFlatTreeFastNoLayout = true;
                    m_ConsoleTree.Config.DefaultMenuStyle.SetHeight(18);
                }

                GUILayout.FlexibleSpace();

                GUILayout.Label("Console");
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.345f, 0.345f, 0.345f, 1.0f));
                GUILayout.BeginVertical(GUILayoutOptions.Height(200));
                m_ConsoleTree.DrawMenuTree();
                GUILayout.EndVertical();
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.345f, 0.345f, 0.345f, 1.0f));
            }

            [Button("Clear Console")]
            private void ClearConsole()
            {
                // The tree will be re-calculated on tnext DrawTree call.
                m_ConsoleTree = null;
            }
        }
    }
}
#endif
