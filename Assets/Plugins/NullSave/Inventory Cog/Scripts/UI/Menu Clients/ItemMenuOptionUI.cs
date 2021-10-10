using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class ItemMenuOptionUI : MonoBehaviour
    {

        #region Variables

        public UnityEvent onSelected, onUnSelected, onSubmit;
        public OptionUIClick onClick;

        #endregion

        #region Public Methods

        public void Click()
        {
            onClick?.Invoke(this);
        }

        #endregion

    }
}