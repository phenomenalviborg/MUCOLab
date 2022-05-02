using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

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

        string experienceName = "NewExperience";
        EExperienceTemplate selectedExperienceTemplate = EExperienceTemplate.Blank;

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
                // Create application configuration
                // TODO: Get scenes in some from some sort of constant MUCOSDK config file.
                string relativeApplicationConfigurationPath = $"Assets/ApplicationConfiguration.asset";
                ApplicationConfiguration applicationConfiguration = ScriptableObject.CreateInstance<ApplicationConfiguration>();
                applicationConfiguration.EntryScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/ExperienceFrameworkRnD/S_Entry.unity");
                applicationConfiguration.MenuScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/ExperienceFrameworkRnD/S_Entry.unity");
                AssetDatabase.CreateAsset(applicationConfiguration, relativeApplicationConfigurationPath);
            }
            EditorGUILayout.Space(16);

            // Project generator
            GuiLine();
            EditorGUILayout.Space(16);
            experienceName = EditorGUILayout.TextField("Experience Name", experienceName);
            selectedExperienceTemplate = (EExperienceTemplate)EditorGUILayout.Popup("Project Template", (int)selectedExperienceTemplate, Enum.GetNames(typeof(EExperienceTemplate)));
            if (GUILayout.Button("Generate Project"))
            {
                string absoluteProjectPath = $"{Application.dataPath}/{experienceName}";
                Debug.Log($"Generating project at: '{absoluteProjectPath}'");
                if (!Directory.Exists(absoluteProjectPath))
                {
                    // Create directory
                    Directory.CreateDirectory(absoluteProjectPath);
                    AssetDatabase.Refresh();

                    // Create experiece scene, this should most likely be duplicating a template scene? 
                    string absoluteExperienceScenePath = $"{absoluteProjectPath}/S_{experienceName}.unity";
                    Scene experieceScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                    EditorSceneManager.SaveScene(experieceScene, absoluteExperienceScenePath);
                    AssetDatabase.Refresh();

                    // Create experience configuration
                    string relativeExperienceConfigurationPath = $"Assets/{experienceName}/{experienceName}Configuration.asset";
                    ExperienceConfiguration experienceConfiguration = ScriptableObject.CreateInstance<ExperienceConfiguration>();
                    experienceConfiguration.name = experienceName;
                    AssetDatabase.CreateAsset(experienceConfiguration, relativeExperienceConfigurationPath);
                }
                else
                {
                    Debug.LogError($"Failed to create project, a directory at '{absoluteProjectPath}' already exists.");
                    return;
                }
            }
        }
    }
}
