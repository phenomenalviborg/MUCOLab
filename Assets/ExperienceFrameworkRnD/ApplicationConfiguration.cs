using UnityEngine;
using UnityEditor;

namespace PhenomenalViborg.MUCOSDK
{
    [CreateAssetMenu(fileName = "NewApplicationConfiguration", menuName = "MUCOSDK/Application Configuration")]
    public class ApplicationConfiguration : ScriptableObject
    {
        public SceneAsset EntryScene;
        public SceneAsset MenuScene;

        public ExperienceConfiguration[] ExperienceConfigurations = new ExperienceConfiguration[0];
    }
}

