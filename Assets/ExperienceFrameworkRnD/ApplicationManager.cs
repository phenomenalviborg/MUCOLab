using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class ApplicationManager : MonoBehaviour
    {

        [Serializable]
        public struct PlayerStats
        {
            public int movementSpeed;
            public int hitPoints;
            public bool hasHealthPotion;
        }

        [SerializeField] private PlayerStats m_PlayerStats;
        [SerializeField] private GameObject m_UserPrefab;
    }
}