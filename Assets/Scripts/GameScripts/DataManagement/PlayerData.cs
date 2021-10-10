using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private List<Unit> playerParty = new List<Unit>();
    //items
    //stats
    //progress

    public List<Unit> PlayerParty => playerParty;

    public void AddUnitToParty(Unit unit)
    {
        if (!playerParty.Contains(unit))
        playerParty.Add(unit);
        //needs to only relate to datamanager
    }
    
    public void RemoveUnitFromParty(Unit unit)
    {
        playerParty.Remove(unit);
        //needs to only relate to datamanager
    }

    public void LoadParty()
    {
        //savemanager save/load this.
    }
}
