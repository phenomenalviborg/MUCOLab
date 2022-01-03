using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PhenomenalViborg.MUCOSDK
{

    public class MUCODatasheetWidget : MonoBehaviour
    {
        [SerializeField] TMP_Text m_ServerColumn;

        void FixedUpdate()
        {
            string ServerDataText = "<b>SERVER DATA</b>";

            if (MUCOServerNetworkManager.Instance.Server != null)
            {
                ServerDataText += " - <b>Server Configuration</b>";
                ServerDataText += "    • ";
            }

            m_ServerColumn.SetText(ServerDataText);
        }
    }
}