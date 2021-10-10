using System.Collections.Generic;
using UnityEngine;

public class BattleParty : MonoBehaviour
{
    [SerializeField] private List<Character> partyUnits;
    [SerializeField] private bool isEnemy;
    public List<Character> PartyUnits => partyUnits;

    public bool IsEnemy => isEnemy;
}