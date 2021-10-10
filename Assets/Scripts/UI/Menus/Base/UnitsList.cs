using System.Collections.Generic;
using UnityEngine;

public class UnitsList : MenuItem
{
    [SerializeField] private UnitUI unitUI; // This is our prefab object that will be exposed in the inspector.
    [SerializeField] private Transform listParent; // This is the parent object under which the units will be created.
    [SerializeField] private Unit unit; // For Testing.


    //public int numberToCreate; // number of objects to create. Exposed in inspector
    
    private void Start()
    {
        DataManager.Instance.OnPartyMemberAdded += (u)=>UpdatePlayerPartyList();
        DataManager.Instance.OnPartyMemberRemoved += (u)=>UpdatePlayerPartyList();
        UpdatePlayerPartyList();
    }

    private void UpdatePlayerPartyList()
    {
        var playerParty = DataManager.Instance.PlayerData.PlayerParty;
        ClearList();

        foreach(var unit in playerParty)
        {
            GameObject newUnit;
            newUnit = Instantiate(unitUI.gameObject, listParent);
            newUnit.name = unit.UnitParams.UnitName;
            unitUI.SetUnitUI(unit.UnitImages.PortraitImage, unit.UnitParams.UnitName);
        }
    }

    private void ClearList()
    {
        for (int i = 0; i < listParent.childCount; i++)
        {
            Destroy(listParent.GetChild(i).gameObject);
        }
    }

    public void AddUnitToPartyTest() // For testing
    {
        DataManager.Instance.OnPartyMemberAdded.Invoke(unit);
    }

    public void RemoveUnitFromPartyTest() // For testing
    {
        DataManager.Instance.OnPartyMemberRemoved.Invoke(unit);
    }
}
