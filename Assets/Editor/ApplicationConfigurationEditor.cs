using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PhenomenalViborg.MUCOSDK
{
    [CustomEditor(typeof(ApplicationConfiguration))]
    public class ApplicationConfigurationEditor : Editor
    {
        private ApplicationConfiguration m_ExperienceConfiguration;

        void OnEnable()
        {
            m_ExperienceConfiguration = (ApplicationConfiguration)target;
        }

        void GuiLine(int i_height = 1)
        {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height);

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));

        }

        public override void OnInspectorGUI()
        {
           
        }
    }
}
