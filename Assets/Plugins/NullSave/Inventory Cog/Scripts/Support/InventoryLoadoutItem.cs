using System.IO;

namespace NullSave.TOCK.Inventory
{
    public class InventoryLoadoutItem
    {

        #region Variables

        public string itemName;
        public string instanceId;
        public string equipPoint;
        public bool stored;
        public string skillSlot;
        public int count;

        #endregion

        #region Constructors

        public InventoryLoadoutItem() { }

        public InventoryLoadoutItem(InventoryItem item)
        {
            equipPoint = item.CurrentEquipPoint == null ? null : item.CurrentEquipPoint.pointId;
            instanceId = item.InstanceId;
            itemName = item.name;
            skillSlot = null;
            stored = item.EquipState == EquipState.Stored;
        }

        #endregion

        #region Public Methods

        public void Load(Stream stream)
        {
            itemName = stream.ReadStringPacket();
            instanceId = stream.ReadStringPacket();
            equipPoint = stream.ReadStringPacket();
            stored = stream.ReadBool();
            skillSlot = stream.ReadStringPacket();
            count = stream.ReadInt();
        }

        public void Save(Stream stream)
        {
            stream.WriteStringPacket(itemName);
            stream.WriteStringPacket(instanceId);
            stream.WriteStringPacket(equipPoint);
            stream.WriteBool(stored);
            stream.WriteStringPacket(skillSlot);
            stream.WriteInt(count);
        }

        #endregion

    }
}