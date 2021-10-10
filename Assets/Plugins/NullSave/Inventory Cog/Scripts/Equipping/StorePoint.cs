using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("store_point", false)]
    public class StorePoint : MonoBehaviour
    {

        #region Variables

        public string pointId = "StorePoint";
        public bool drawGizmo = true;
        public float gizmoScale = 0.1f;

        public List<EquipPoint> forceUnstore;

        private InventoryItem _item;

        #endregion

        #region Properties

        public InventoryItem Item { get; internal set; }

        public GameObject ObjectReference { get; internal set; }

        #endregion

        #region Unity Methods

        private void OnDrawGizmosSelected()
        {
            if (drawGizmo)
            {
                Gizmos.matrix = this.transform.localToWorldMatrix;
                Gizmos.color = new Color(0.808f, 0.882f, 0.184f, 0.5f);
                Gizmos.DrawCube(Vector3.zero, new Vector3(gizmoScale, gizmoScale, gizmoScale));
            }
        }

        #endregion

    }
}