using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PhenomenalViborg.MUCOSDK;

namespace PhenomenalViborg.MUCOManager
{
    public class MUCOUserList : MonoBehaviour
    {
        [SerializeField] private GameObject m_UserWidgetPrefab;

        private Dictionary<MUCOUser, MUCOUserListItemWidget> m_UserWidgets = new Dictionary<MUCOUser, MUCOUserListItemWidget>();

        private void FixedUpdate()
        {
            foreach (MUCOUser user in FindObjectsOfType<MUCOUser>())
            {
                if (!m_UserWidgets.ContainsKey(user))
                {
                    GameObject newUserWidgetGameObject = Instantiate(m_UserWidgetPrefab, transform);
                    MUCOUserListItemWidget newUserWidget = newUserWidgetGameObject.GetComponent<MUCOUserListItemWidget>();
                    m_UserWidgets.Add(user, newUserWidget);
                }

                m_UserWidgets[user].SetInfoTMP(user.UserIdentifier.ToString());
            }
        }
    }
}
