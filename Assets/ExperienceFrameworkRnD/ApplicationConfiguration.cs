using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    [CreateAssetMenu(fileName = "NewApplicationConfiguration", menuName = "MUCOSDK/Application Configuration")]
    public class ApplicationConfiguration : ScriptableObject
    {
        public ExperienceConfiguration[] ExperienceConfigurations = new ExperienceConfiguration[0];
    }
}

