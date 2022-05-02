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

        ApplicationConfiguration m_ApplicationConfiguration;

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
                // TODO: Check if an ApplicationConfiguration file is ready; a refernce will also be need to add newly created experience configuration to.

                // Create application configuration
                // TODO: Get scenes in some from some sort of constant MUCOSDK config file.
                string relativeApplicationConfigurationPath = $"Assets/ApplicationConfiguration.asset";
                ApplicationConfiguration applicationConfiguration = ScriptableObject.CreateInstance<ApplicationConfiguration>();
                applicationConfiguration.EntryScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/ExperienceFrameworkRnD/S_Entry.unity"); // TODO: Better solution for path
                applicationConfiguration.MenuScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/ExperienceFrameworkRnD/S_Menu.unity"); // TODO: Better solution for path
                AssetDatabase.CreateAsset(applicationConfiguration, relativeApplicationConfigurationPath);

                // Add scenes to build settings
                var originalBuildScenes = EditorBuildSettings.scenes;
                var newBuildScenes = new EditorBuildSettingsScene[originalBuildScenes.Length + 2];
                System.Array.Copy(originalBuildScenes, newBuildScenes, originalBuildScenes.Length);
                newBuildScenes[newBuildScenes.Length - 2] = new EditorBuildSettingsScene("Assets/ExperienceFrameworkRnD/S_Entry.unity", true); // TODO: Check in the scene is pressent before adding
                newBuildScenes[newBuildScenes.Length - 1] = new EditorBuildSettingsScene("Assets/ExperienceFrameworkRnD/S_Menu.unity", true); // TODO: Check in the scene is pressent before adding
                EditorBuildSettings.scenes = newBuildScenes;

                m_ApplicationConfiguration = applicationConfiguration;
            }
             
            EditorGUILayout.Space(8);
            m_ApplicationConfiguration = EditorGUILayout.ObjectField("Application Configuration", m_ApplicationConfiguration, typeof(ApplicationConfiguration), false) as ApplicationConfiguration;
           
            EditorGUILayout.Space(16);

            // Project generator
            GuiLine();
            EditorGUILayout.Space(16);
            experienceName = EditorGUILayout.TextField("Experience Name", experienceName);
            selectedExperienceTemplate = (EExperienceTemplate)EditorGUILayout.Popup("Project Template", (int)selectedExperienceTemplate, Enum.GetNames(typeof(EExperienceTemplate)));
            if (GUILayout.Button("Generate Project"))
            {
                string relativeProjectPath = $"Assets/{experienceName}";
                string absoluteProjectPath = $"{Application.dataPath}/{experienceName}";
                Debug.Log($"Generating project at: '{absoluteProjectPath}'");
                if (!Directory.Exists(absoluteProjectPath))
                {
                    // Create directory
                    Directory.CreateDirectory(absoluteProjectPath);
                    AssetDatabase.Refresh();

                    // Create experiece scene, this should most likely be duplicating a template scene? 
                    string relativeExperienceScenePath = $"{relativeProjectPath}/S_{experienceName}.unity";
                    Scene experieceScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                    EditorSceneManager.SaveScene(experieceScene, relativeExperienceScenePath);
                    AssetDatabase.Refresh();

                    // Add scene to build settings
                    var originalBuildScenes = EditorBuildSettings.scenes;
                    var newBuildScenes = new EditorBuildSettingsScene[originalBuildScenes.Length + 1];
                    System.Array.Copy(originalBuildScenes, newBuildScenes, originalBuildScenes.Length);
                    newBuildScenes[newBuildScenes.Length - 1] = new EditorBuildSettingsScene(relativeExperienceScenePath, true); // TODO: Check in the scene is pressent before adding
                    EditorBuildSettings.scenes = newBuildScenes;

                    // Create experience configuration
                    string relativeExperienceConfigurationPath = $"{relativeProjectPath}/{experienceName}Configuration.asset";
                    ExperienceConfiguration experienceConfiguration = ScriptableObject.CreateInstance<ExperienceConfiguration>();
                    experienceConfiguration.Name = experienceName;
                    experienceConfiguration.Scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(relativeExperienceScenePath);
                    // Assign player prefabs, TODO: This should also be taken from some sort of template config file native to MUCOSDK.
                    experienceConfiguration.LocalUserPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ExperienceFrameworkRnD/P_LocalUser.prefab"); // TODO: Better solution for path
                    experienceConfiguration.RemoteUserPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ExperienceFrameworkRnD/P_RemoteUser.prefab"); // TODO: Better solution for path
                    AssetDatabase.CreateAsset(experienceConfiguration, relativeExperienceConfigurationPath);

                    if (m_ApplicationConfiguration)
                    {
                        var originalExperienceConfigurations =m_ApplicationConfiguration.ExperienceConfigurations;
                        var newExperienceConfigurations = new ExperienceConfiguration[originalExperienceConfigurations.Length + 1];
                        System.Array.Copy(originalExperienceConfigurations, newExperienceConfigurations, originalExperienceConfigurations.Length);
                        newExperienceConfigurations[newExperienceConfigurations.Length - 1] = experienceConfiguration;
                        m_ApplicationConfiguration.ExperienceConfigurations = newExperienceConfigurations;
                    }
                    else
                    {
                        Debug.LogError("m_ApplicationConfiguration was undefined!");
                    }
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
