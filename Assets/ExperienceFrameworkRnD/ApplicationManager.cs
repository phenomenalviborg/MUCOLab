using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{
    public class ApplicationManager : MonoBehaviour
    {
        [SerializeField] private ApplicationConfiguration m_ApplicationConfiguration;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene(m_ApplicationConfiguration.MenuScene.name);
        }
    }
}