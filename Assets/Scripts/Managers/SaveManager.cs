using PixelCrushers;
using Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : Singleton<SaveManager>
{
    private SaveSystem saveSystem;
    private static SlotUISaver slotUISaver;
    private string saveTime;
    private static SavedGameData savedSlotData = new SavedGameData();
    private string[] saveSlotInfo = new string[5];

    public string[] SaveSlotInfo => saveSlotInfo;

    protected override void Awake()
    {
        base.Awake();
        saveSystem = GetComponent<SaveSystem>();
        slotUISaver = GetComponent<SlotUISaver>(); //register from the saver's script if many custom savers
        SaveSystem.ApplySavedGameData(LoadSlotsInfo());
    }

    public void GetData(string[] savedSlotInfo)
    {
        saveSlotInfo = savedSlotInfo;
    }

    public void SaveGame(int slotNumber)
    {
        SaveSlotsInfo();
        saveSystem.SaveGameToSlot(slotNumber);
        saveSlotInfo[slotNumber-1] = saveTime;
    }

    public void LoadGame(int slotNumber)
    {
        saveSystem.LoadGameFromSlot(slotNumber);
        GameManager.Instance.RunGame();
    }

    public string GetSaveTime()
    {
        saveTime = DateTime.Now.ToString();

        return saveTime;
    }

    public static SavedGameData RecordSlotData()
    {
        //savedSlotData.sceneName = string.Empty;
        var saver = slotUISaver;
        savedSlotData.SetData(saver.key, -1, saver.RecordData());
        return savedSlotData;
    }

    private void SaveSlotsInfo()
    {
        var s = SaveSystem.Serialize(RecordSlotData());
        DiskSavedGameDataStorer.WriteStringToFile(Application.persistentDataPath + "/data.dat", s);
    }

    private SavedGameData LoadSlotsInfo()
    {
        var s = DiskSavedGameDataStorer.ReadStringFromFile(Application.persistentDataPath + "/data.dat");
        return SaveSystem.Deserialize<SavedGameData>(s);
    }
}

public interface ISaveable
{
    void Save();
    void Load();
}
