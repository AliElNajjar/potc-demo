using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartyInfo
{
    public PlayerBattleUnitHolder[] activePartyMembers;


    public PartyInfo()
    {
        activePartyMembers = new PlayerBattleUnitHolder[0];
    }

    public void AddPartyMember(PlayerBattleUnitHolder newMember)
    {
        if (IsDuplicate(newMember))
            return;

        PlayerBattleUnitHolder[] temp = new PlayerBattleUnitHolder[activePartyMembers.Length + 1];

        for (int i = 0; i < activePartyMembers.Length; i++)
        {
            temp[i] = activePartyMembers[i];
        }

        temp[temp.Length - 1] = newMember;

        activePartyMembers = temp;
    }

    public bool IsDuplicate(PlayerBattleUnitHolder unit)
    {
        for (int i = 0; i < activePartyMembers.Length; i++)
        {
            if (activePartyMembers[i] == unit)
            {
                return true;
            }
        }

        return false;
    }

    public void Save()
    {
        //SerializableDictionary<string, PlayableUnit> unitIDs = new SerializableDictionary<string, PlayableUnit>();

        //// Save unit persistent data of each unit.
        //for (int i = 0; i < activePartyMembers.Length; i++)
        //{
        //    Debug.Log($"<color=yellow> Saving ({activePartyMembers[i].UnitPersistentData.UnitData.unitName}) battle data...</color>");
        //    unitIDs.Add(activePartyMembers[i].UnitPersistentData.name, activePartyMembers[i].UnitPersistentData.UnitData as PlayableUnit);
        //}

        //SaveSystem.SetObject<SerializableDictionary<string, PlayableUnit>>(this.GetType().ToString() + "/PartyMembers", unitIDs);

        ////// Save active party manager data.
        ////if (ActiveManager != null)
        ////{
        ////    Debug.Log($"<color=yellow> Saving manager ({ActiveManager.managerName}) data...</color>");
        ////    SaveSystem.SetObject<PartyManagerUnit>(this.GetType().ToString() + "/Manager", ActiveManager);
        ////}
    }

    public void Load()
    {
        //if (!SaveSystem.HasKey(this.GetType().ToString() + "/PartyMembers"))
        //{
        //    AddPartyMember(UnitDatabase.FindUnitByName("Muchachoman"));
        //    Debug.LogWarning("Party members not found in SaveSystem. Adding muchacho.");
        //    return;
        //}

        //List<string> unitIDs = new List<string>();
        //SerializableDictionary<string, PlayableUnit> loadedUnitIDs = SaveSystem.GetObject<SerializableDictionary<string, PlayableUnit>>(this.GetType().ToString() + "/PartyMembers");

        //if (loadedUnitIDs != null && loadedUnitIDs.Count == 0)
        //{
        //    AddPartyMember(UnitDatabase.FindUnitByName("Muchachoman"));
        //    Debug.LogWarning("Unexpected case. Party members found in SaveSystem but it was empty. Adding muchacho.");
        //    activeManager = new PartyManagerUnit();
        //    return;
        //}

        //// Load battle units
        //// Save loaded data from save system to unit data base.
        //foreach (KeyValuePair<string, PlayableUnit> entry in loadedUnitIDs)
        //{
        //    unitIDs.Add(entry.Key);
        //    UnitDatabase.playableUnitStats[entry.Key] = entry.Value;
        //    Debug.Log($"<color=blue>{entry.Key} battle unit added to active party.</color>");
        //}

        //// Add party member having unit's id as the reference of the unit saved through save system.
        //for (int i = 0; i < unitIDs.Count; i++)
        //{
        //    AddPartyMember(UnitDatabase.playableUnits[unitIDs[i]]);
        //    Debug.Log($"<color=blue>{UnitDatabase.playableUnits[unitIDs[i]]} battle unit added to active party.</color>");
        //}

        ////// Load Manager
        ////activeManager = SaveSystem.GetObject<PartyManagerUnit>(this.GetType().ToString() + "/Manager");

        ////if (activeManager == null || activeManager.managerId == ManagerID.None)
        ////    activeManager = new PartyManagerUnit();
        ////else
        ////    SetPartyManager(UnitDatabase.managerUnits[activeManager.managerName]);
    }
}
