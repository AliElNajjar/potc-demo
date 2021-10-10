using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NullSave.TOCK;
using NullSave.TOCK.Inventory;
using System.IO;

public class DemoSaveLoad : MonoBehaviour
{

    public InventoryCog inventory;
    public string filename = "inventory.sav";
    [TextArea(2,5)] public string extraText = "Wow, that's cool";

    public void Load()
    {
        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            inventory.InventoryStateLoad(fs);
            Debug.Log("Extra text: " + fs.ReadStringPacket());
        }
    }

    public void Save()
    {
        using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
        {
            inventory.InventoryStateSave(fs);
            fs.WriteStringPacket(extraText);
        }
    }

}
