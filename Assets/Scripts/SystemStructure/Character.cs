using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private UnitParameters parameters;
    [SerializeField] private List<Skill> activatedSkills;

    public UnitParameters Parameters { get => parameters; set => parameters = value; }

    public Unit CharacterUnit => unit;

    public List<Skill> ActivatedSkills { get => activatedSkills; set => activatedSkills = value; }

    public UnitParameters Attack(Unit other)
    {
        var bufferParams = parameters;
        
        var power = 0f;
        var weaponResist = 0f;
        var def = 0f;
        
        //var power = other.BaseBuffs.Select(x => x.Apply(bufferParams)).Sum();
        if (unit.Weapons != null)
        {
            power = unit.Weapons.Select(x => x.Stats.StrengthParam.AttackPower).Sum();

            weaponResist = unit.Weapons.Sum(weapon =>
                (other.UnitClass.Resistances.PhysicResist.Find(x => x.resistanceType == weapon.PhysicalType))
                .resistedValue);
        }


        if (other.Armors != null)
        {
            def = other.Armors.Select(x => x.Parameters.StrengthParam.DefensePower).Sum();
        }

        var damage = bufferParams.StrengthParam.AttackPower + power - ((power * weaponResist) / 100f) -
                     (other.UnitParams.StrengthParam.DefensePower + def);
        other.UnitParams.EnduranceParam.ChangeHealthPoints(-damage);

        return bufferParams;
    }

    //damage =  Atk + Power - ([Power x Resistance] / 100) - Def

    private void OnValidate()
    {
        parameters = CharacterUnit.UnitParams;
    }
}