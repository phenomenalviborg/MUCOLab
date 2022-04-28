using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{
    [CreateAssetMenu(fileName = "NewExperienceConfiguration", menuName = "MUCOSDK/Experience Configuration")]
    public class ExperienceConfiguration : ScriptableObject
    {
        public string Name;
        public string Description;

        public string SceneName;
        
        private void OnValidate()
        {
            if (true) { Debug.LogError($"Invalid scene name '{SceneName}'. Please verify that the scene name is included in the 'Build Settings/Scenes in build' list."); };
        }
    }
}

