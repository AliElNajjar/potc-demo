using UnityEngine;

namespace NullSave.TOCK
{
    public class MessageList : MonoBehaviour
    {

        #region Variables

        public MessageListItem itemPrefab;
        public Transform listContainer;

        #endregion

        #region Public Methods

        public void AddMessage(Sprite icon, string message)
        {
            MessageListItem item = Instantiate(itemPrefab, listContainer);
            item.SetMessage(icon, message);
        }

        #endregion

    }
}