using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParty : Singleton<PlayerParty>, ISaveable
{
    public PartyInfo playerParty;

    public void Load()
    {
        playerParty.Load();
    }

    public void Save()
    {
        playerParty.Save();
    }

    public void ResetParty()
    {
        PlayerBattleUnitHolder  terra = Resources.Load<GameObject>("Terra").GetComponent<PlayerBattleUnitHolder>();
        playerParty = new PartyInfo();
        playerParty.AddPartyMember(terra);
    }
}
