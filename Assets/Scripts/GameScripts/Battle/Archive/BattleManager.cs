using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleParty enemyParty;
    [SerializeField] private BattleParty playerParty;
    [SerializeField] private bool a;

    bool o = false;

    private Queue<BattleCharacter> c;

    [Serializable]
    public class BattleCharacter
    {
        public bool isEnemy;
        public Character character;

        public BattleCharacter(bool partyIsEnemy, Character partyUnit)
        {
            isEnemy = partyIsEnemy;
            character = partyUnit;
        }
    }

    [SerializeField] private List<BattleCharacter> unitsInBattle;

    private Queue<BattleCharacter> UnitsQueue()
    {
        unitsInBattle = new List<BattleCharacter>();

        foreach (var partyUnit in playerParty.PartyUnits)
        {
            unitsInBattle.Add(new BattleCharacter(playerParty.IsEnemy, partyUnit));
        }

        foreach (var partyUnit in enemyParty.PartyUnits)
        {
            unitsInBattle.Add(new BattleCharacter(enemyParty.IsEnemy, partyUnit));
        }

        var q = unitsInBattle.OrderBy(x => x.character.Parameters.AgilityParam.InitiativePower).ToList();

        return new Queue<BattleCharacter>(q);
    }

    private void Start()
    {
        c = UnitsQueue();
    }

    private bool end = false;

    public void Turn()
    {
        if (end) return;

        var t = c.Dequeue();

        AttackingCharacter(t);

        if (!(playerParty.PartyUnits.Any(x => x.Parameters.EnduranceParam.HealthPoints > 0)))
        {
            Debug.Log("PlayerEnemy wins");
            end = true;

            return;
        }

        if (!(enemyParty.PartyUnits.Any(x => x.Parameters.EnduranceParam.HealthPoints > 0)))
        {
            Debug.Log("Player wins");
            end = true;

            return;
        }

        if (!(c.Count > 0))
            c = UnitsQueue();
    }

    private void AttackingCharacter(BattleCharacter t)
    {
        var attackedParty = t.isEnemy ? playerParty : enemyParty;

        var attackedCharacters = attackedParty.PartyUnits.Where(x => x.Parameters.EnduranceParam.HealthPoints > 0)
            .ToList();

        var reactions =
            attackedCharacters.Where(x => x.ActivatedSkills.Select(s => s.SkillType() == nameof(Reaction)).ToList().Count > 0)
                .ToList();

        foreach (var skills in reactions.SelectMany(x => x.ActivatedSkills.Where(s => s.SkillType() == nameof(Reaction))))
        {
            skills.ActivateSkill(t.character.Parameters);
        }

        attackedCharacters[Random.Range(0, attackedParty.PartyUnits.Count)].Parameters =
            t.character.Attack(attackedCharacters[Random.Range(0, attackedParty.PartyUnits.Count)].CharacterUnit);
    }
}