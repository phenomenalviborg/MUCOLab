using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhenomenalViborg.MUCONet;
using PhenomenalViborg.MUCOSDK;

namespace PhenomenalViborg.MUCOManager
{
    public class MUCOPlayAreaWidget : MonoBehaviour
    {
        [SerializeField] private GameObject m_UserWidgetPrefab;

        private Dictionary<MUCOUser, MUCOPlayAreaUserWidget> m_UserWidgets = new Dictionary<MUCOUser, MUCOPlayAreaUserWidget>();

        private void FixedUpdate()
        {
            foreach (MUCOUser user in FindObjectsOfType<MUCOUser>())
            {
                if (!m_UserWidgets.ContainsKey(user))
                {
                    GameObject newUserWidgetGameObject = Instantiate(m_UserWidgetPrefab, transform);
                    MUCOPlayAreaUserWidget newUserWidget = newUserWidgetGameObject.GetComponent<MUCOPlayAreaUserWidget>();
                    m_UserWidgets.Add(user, newUserWidget);
                }

                m_UserWidgets[user].gameObject.transform.localPosition = user.gameObject.transform.position;
            }
        }
    }
}
