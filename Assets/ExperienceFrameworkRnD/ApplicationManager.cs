using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Antilatency;

namespace PhenomenalViborg.MUCOSDK
{
    public class ApplicationManager : PhenomenalViborg.MUCOSDK.IManager<ApplicationManager>
    {
        [SerializeField] private ApplicationConfiguration m_ApplicationConfiguration;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene(m_ApplicationConfiguration.MenuScene.name);
            
            string serverAddress = TrackingManager.GetInstance().GetStringPropertyFromAdminNode("ServerAddress");
            int serverPort = int.Parse(TrackingManager.GetInstance().GetStringPropertyFromAdminNode("ServerPort"));
            NetworkManager.GetInstance().Connect(serverAddress, serverPort);
        }
    }
}