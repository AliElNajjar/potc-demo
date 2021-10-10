using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class RenamePrompt : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI currentName;
        public string currentNameFormat = "Rename {name} to...";
        public InputField newName;
        public TMP_InputField newName_TMP;
        public Button okButton, cancelButton;

        public UnityEvent onRename, onCancel;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem Item { get; set; }

        #endregion

        #region Public Methods

        public void Cancel()
        {
            onCancel?.Invoke();
            Inventory.CloseRenamePrompt();
        }

        public void Rename(string newName)
        {
            Item.CustomName = newName;
            onRename?.Invoke();
            Inventory.CloseRenamePrompt();
        }

        public void SetUI()
        {
            if (currentName != null)
            {
                currentName.text = currentNameFormat.Replace("{name}", Item.DisplayName);
            }

            if (newName != null)
            {
                newName.text = Item.DisplayName;
            }

            if (newName_TMP != null)
            {
                newName_TMP.text = Item.DisplayName;
            }

            if (okButton != null)
            {
                okButton.onClick.RemoveAllListeners();
                if (newName != null) okButton.onClick.AddListener(() => Rename(newName.text));
                if (newName_TMP != null) okButton.onClick.AddListener(() => Rename(newName_TMP.text));
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(Cancel);
            }
        }

        #endregion

    }
}