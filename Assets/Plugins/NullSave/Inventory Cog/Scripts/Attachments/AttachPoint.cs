using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class AttachPoint : MonoBehaviour
    {

        #region Variables

        public string pointId = "AttachPoint";
        public Sprite slotIcon;

        public bool drawGizmo = true;
        [Range(0, 1)] public float gizmoScale = 0.1f;

        public ItemChanged onAttachmentAdded, onAttachmentRemoved;

        #endregion

        #region Unity Methods

        private void OnDrawGizmosSelected()
        {
            if (drawGizmo)
            {
                Gizmos.color = new Color(1, 0.559f, 0.027f, 0.5f);
                Gizmos.DrawSphere(transform.position, gizmoScale);
            }
        }

        #endregion

    }
}