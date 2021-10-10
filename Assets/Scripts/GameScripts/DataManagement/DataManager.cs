using PixelCrushers.DialogueSystem;
using Scripts.Managers;
using System;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Action<Unit> OnPartyMemberAdded;
    public Action<Unit> OnPartyMemberRemoved;
    public Action<LevelData> OnLevelDataUpdated; 
    
    private PlayerData playerData;
    private LevelData LevelData;

    public PlayerData PlayerData => playerData;

    protected override void Awake()
    {
        base.Awake();
        SubscribeToEvents();
        playerData = GetComponent<PlayerData>();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.OnLevelLoaded += HandleNewLevel;
        OnPartyMemberAdded += AddUnitToParty;
        OnPartyMemberRemoved += RemoveUnitFromParty;
    }

    private void HandleNewLevel()
    {
        GetLevelData();
    }

    private void GetLevelData()
    {
        var newData = GameObject.FindWithTag("Level Manager")?.GetComponent<LevelData>();
        if (newData == null) return;
        LevelData = newData;
        OnLevelDataUpdated.Invoke(LevelData);
    }

    private void AddUnitToParty(Unit unit)
    {
        playerData.AddUnitToParty(unit);
    }

    private void RemoveUnitFromParty(Unit unit)
    {
        playerData.RemoveUnitFromParty(unit);
    }
}
