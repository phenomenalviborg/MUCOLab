using System;
using UnityEngine;
using UnityEditor;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOProjectSettingsWindow : EditorWindow
    {
        [MenuItem("MUCOSDK/MUCO Project Settings")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MUCOProjectSettingsWindow));
        }
        void GuiLine(int i_height = 1)
        {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height);

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        enum EExperienceTemplate
        {
            Blank,
            Gallery
        }

        private void OnGUI()
        {   
            GUIStyle headerStyle = new GUIStyle();
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = Color.white;
            headerStyle.fontSize = 18;
            headerStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.Space(32);
            EditorGUILayout.LabelField("MUCO Project Settings", headerStyle);
            EditorGUILayout.Space(32);

            // Setup project settings and other setup
            GuiLine();
            EditorGUILayout.Space(16);
            GUIStyle setupButtonStyle = new GUIStyle(GUI.skin.button);
            setupButtonStyle.richText = true;
            if (GUILayout.Button("<b>Magic setup button (<i>pssst... artist, click me!</i>)</b>", setupButtonStyle, GUILayout.Height(100)))
            {

            }
            EditorGUILayout.Space(16);

            // Project generator
            GuiLine();
            EditorGUILayout.Space(16);
            EExperienceTemplate selectedExperienceTemplate = EExperienceTemplate.Blank;
            string[] experienceTemplateNames = Enum.GetNames(typeof(EExperienceTemplate));
            selectedExperienceTemplate = (EExperienceTemplate)EditorGUILayout.Popup("Project Template", (int)selectedExperienceTemplate, experienceTemplateNames);
            if (GUILayout.Button("Generate Project"))
            {
            }
        }
    }
}
