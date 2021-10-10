using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class MessageManager : Singleton<MessageManager>
    {
        [SerializeField] private Text content;
        [SerializeField] private GameObject panel;

        public Action OnMessageEnd;

        public void TogglePanel(bool active, string panelContent)
        {
            if (panel.activeSelf)
            {
                panel.SetActive(!active);
                OnMessageEnd?.Invoke();
                return;
            }

            content.text = panelContent;
            panel.SetActive(active);
        }
    }
}
