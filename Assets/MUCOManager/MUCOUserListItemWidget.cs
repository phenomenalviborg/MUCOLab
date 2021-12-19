using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PhenomenalViborg.MUCOManager
{
    public class MUCOUserListItemWidget : MonoBehaviour
    {
        [SerializeField] private Text m_Text;

        public void SetInfoTMP(string info)
        {
            m_Text.text = info;
        }
    }
}