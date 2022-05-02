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

        public string ScenePath;

        public GameObject LocalUserPrefab;
        public GameObject RemoteUserPrefab;
    }
}

