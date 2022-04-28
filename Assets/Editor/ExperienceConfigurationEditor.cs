using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PhenomenalViborg.MUCOSDK
{
    [CustomEditor(typeof(ExperienceConfiguration))]
    public class ExperienceConfigurationEditor : Editor
    {
        private ExperienceConfiguration m_ExperienceConfiguration;

        void OnEnable()
        {
            m_ExperienceConfiguration = (ExperienceConfiguration)target;
        }

        void GuiLine(int i_height = 1)

        {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height);

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));


        }
        public override void OnInspectorGUI()
        {
            GUIStyle headerStyle = new GUIStyle();
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = Color.white;
            headerStyle.fontSize = 18;
            headerStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.Space(32);
            EditorGUILayout.LabelField(m_ExperienceConfiguration.name, headerStyle);
            EditorGUILayout.Space(32);
            
            GuiLine();
            EditorGUILayout.Space(16);
            m_ExperienceConfiguration.Name = EditorGUILayout.TextField("Name", m_ExperienceConfiguration.Name);
            m_ExperienceConfiguration.Description = EditorGUILayout.TextField("Description", m_ExperienceConfiguration.Description);
            m_ExperienceConfiguration.SceneName = EditorGUILayout.TextField("Scene Name", m_ExperienceConfiguration.SceneName);
        }
    }
}
