using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public class NetworkManager : PhenomenalViborg.MUCOSDK.IManager<NetworkManager>
    {
        public void Connect(string address, int port)
        {
            Debug.Log($"Connecting: {address}:{port}");
        }
    }
}
