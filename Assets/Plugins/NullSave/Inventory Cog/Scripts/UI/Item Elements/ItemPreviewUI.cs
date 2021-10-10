using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("view", false)]
    public class ItemPreviewUI : MonoBehaviour
    {

        #region Variables

        public Transform previewContainer;

        private GameObject loadedPreview;

        #endregion

        #region Public Methods

        public virtual void ClearPreview()
        {
            if (loadedPreview != null)
            {
                Destroy(loadedPreview);
                loadedPreview = null;
            }
        }

        public virtual void LoadPreview(InventoryItem item)
        {
            if (loadedPreview != null)
            {
                Destroy(loadedPreview);
                loadedPreview = null;
            }

            if (item != null && item.previewObject != null)
            {
                loadedPreview = Instantiate(item.previewObject, previewContainer);
                previewContainer.localScale = new Vector3(item.previewScale, item.previewScale, item.previewScale);

                // Check for attachments
                foreach (AttachPoint ap in loadedPreview.GetComponentsInChildren<AttachPoint>())
                {
                    foreach (AttachmentSlot slot in item.Slots)
                    {
                        if (slot.AttachedItem != null && slot.AttachedItem.attachObject != null && slot.AttachPoint.pointId == ap.pointId)
                        {
                            // Display attachment
                            GameObject attachGo = Instantiate(slot.AttachedItem.attachObject, ap.transform);
                            attachGo.transform.localPosition = Vector3.zero;
                            attachGo.layer = loadedPreview.layer;
                            for (int i = 0; i < attachGo.transform.childCount; i++)
                            {
                                attachGo.transform.GetChild(i).gameObject.layer = loadedPreview.layer;
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }
}