using UnityEngine;
using System;
using PixelCrushers.DialogueSystem;
using System.Collections.Concurrent;

namespace PixelCrushers
{
    public class SlotUISaver : Saver
    {
        [Serializable]
        public class Data
        {
            public string[] savedSlotInfo;
        }

        private Data m_data = new Data();

        
        public override string RecordData()
        {
            var value = SaveManager.Instance.SaveSlotInfo;
            m_data.savedSlotInfo = value;
            return SaveSystem.Serialize(m_data);
        }

        public override void ApplyData(string s)
        {
            var data = SaveSystem.Deserialize<Data>(s, m_data);
            if (data == null || data.savedSlotInfo == null) return;
            m_data = data;
            SaveManager.Instance.GetData(data.savedSlotInfo);
        }
    }
}
