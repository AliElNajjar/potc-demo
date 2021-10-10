using System;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class AttachmentSlot
    {

        #region Properties

        public InventoryItem AttachedItem { get; set; }

        public AttachPoint AttachPoint { get; set; }

        public GameObject ObjectReference { get; set; }

        public InventoryItem ParentItem { get; set; }

        #endregion

    }
}