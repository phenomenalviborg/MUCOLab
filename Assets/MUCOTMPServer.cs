using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCO
{
    public class MUCOTMPServer : MonoBehaviour
    {
        private void Start()
        {
            Networking.MUCOServer.Start(32, 26950);
        }

        private void OnApplicationQuit()
        {
            Networking.MUCOServer.Stop();
        }
    }
}
