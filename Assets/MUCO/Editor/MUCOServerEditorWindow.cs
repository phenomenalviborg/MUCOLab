#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

namespace PhenomenalViborg.MUCO
{
    [ExecuteAlways]
    public class MUCOServerEditorWindow : OdinEditorWindow
    {
        private MUCOEditorConsole m_Console = new MUCOEditorConsole();

        private float m_NextServerUpdateTime = 0.0f;

        [MenuItem("MUCO/Dedicated Server")]
        private static void OpenWindow()
        {
            MUCOServerEditorWindow window = GetWindow<MUCOServerEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
            window.titleContent = new GUIContent("MUCO | Dedicated Server");
        }

        [OnInspectorGUI]
        private void Draw()
        {
            // Start/Stop server buttons
            GUILayout.BeginHorizontal();
            GUI.enabled = !Networking.MUCOServer.IsRunning();
            if (GUILayout.Button("Start Server"))
            {
                Networking.MUCOServer.ServerLog += m_Console.WriteLine;

                Networking.MUCOServer.Start(32, 26950);
            }

            GUI.enabled = Networking.MUCOServer.IsRunning();
            if (GUILayout.Button("Stop Server"))
            {
                Networking.MUCOServer.Stop();

                Networking.MUCOServer.ServerLog -= m_Console.WriteLine;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // Server info
            GUILayout.Label("Server Info");

            GUILayout.FlexibleSpace();

            m_Console.Draw();
        }

        void Update()
        {
            Debug.Log($"{Time.time} - {m_NextServerUpdateTime}");
            if (Time.time > m_NextServerUpdateTime)
            {
                m_NextServerUpdateTime = Time.time + Time.fixedDeltaTime;

                Debug.Log("tick");
                Networking.MUCOThreadManager.UpdateMain();
            }
        }

        class MUCOEditorConsole
        {
            private OdinMenuTree m_MenuTree = null;

            public void Draw()
            {
                if (m_MenuTree == null)
                {
                    m_MenuTree = new OdinMenuTree();
                    m_MenuTree.Config.EXPERIMENTAL_INTERNAL_DrawFlatTreeFastNoLayout = true;
                    m_MenuTree.Config.DefaultMenuStyle.SetHeight(18);
                }

                GUILayout.FlexibleSpace();

                GUILayout.Label("Console");
                MUCOEditorUtils.DrawDividerLine();
                GUILayout.BeginVertical(GUILayoutOptions.Height(200));
                m_MenuTree.DrawMenuTree();
                GUILayout.EndVertical();
                MUCOEditorUtils.DrawDividerLine();
            }

            public void WriteLine(string msg)
            {
                if (m_MenuTree != null)
                {
                    OdinMenuItem newOdinMenuItem = new OdinMenuItem(m_MenuTree, msg, null);
                    newOdinMenuItem.Icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Sirenix/Odin Inspector/Assets/Editor/Odin Inspector Logo.png", typeof(Texture2D));
                    m_MenuTree.MenuItems.Add(newOdinMenuItem);

                    m_MenuTree.MarkDirty();
                }
                else
                {
                    Debug.LogError("m_MenuTree was null!");
                }
            }
        }
    }
}

public static class MUCOEditorUtils
{
    public static void DrawDividerLine()
    {
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.345f, 0.345f, 0.345f, 1.0f));
    }
}

public class MUCOCustomFixedUpdate
{
    private float m_FixedDeltaTime;
    private float m_ReferenceTime = 0;
    private float m_FixedTime = 0;
    private float m_MaxAllowedTimestep = 0.3f;
    private System.Action m_FixedUpdate;
    private System.Diagnostics.Stopwatch m_Timeout = new System.Diagnostics.Stopwatch();

    public MUCOCustomFixedUpdate(float aFixedDeltaTime, System.Action aFixecUpdateCallback)
    {
        m_FixedDeltaTime = aFixedDeltaTime;
        m_FixedUpdate = aFixecUpdateCallback;
    }

    public bool Update(float aDeltaTime)
    {
        m_Timeout.Reset();
        m_Timeout.Start();

        m_ReferenceTime += aDeltaTime;
        while (m_FixedTime < m_ReferenceTime)
        {
            m_FixedTime += m_FixedDeltaTime;
            if (m_FixedUpdate != null)
                m_FixedUpdate();
            if ((m_Timeout.ElapsedMilliseconds / 1000.0f) > m_MaxAllowedTimestep)
                return false;
        }
        return true;
    }

    public float FixedDeltaTime
    {
        get { return m_FixedDeltaTime; }
        set { m_FixedDeltaTime = value; }
    }
    public float MaxAllowedTimestep
    {
        get { return m_MaxAllowedTimestep; }
        set { m_MaxAllowedTimestep = value; }
    }
    public float ReferenceTime
    {
        get { return m_ReferenceTime; }
    }
    public float FixedTime
    {
        get { return m_FixedTime; }
    }
}

#endif
