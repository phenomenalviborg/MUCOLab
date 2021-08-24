using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCO
{
    public class MUCOTMPPlayerManager : MonoBehaviour
    {
        public int ID;

        private void FixedUpdate()
        {
            Networking.MUCOClientSend.PlayerMovement();
        }
    }
}
