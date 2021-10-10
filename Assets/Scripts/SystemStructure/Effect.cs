using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Unit/Create Effect", order = 1)]
public class Effect : ScriptableObject
{
    [SerializeField] private UnitParameters effectParams;
    public float Apply(UnitParameters parameters)
    {
        for (int i = 0; i < Enum.GetValues((typeof(UnitParameters.Stats))).Length; i++)
        {
            //parameters[(UnitParameters.Stats)i].AffectStats(effectParams[(UnitParameters.Stats)i].Multipliers);
        }

        return 1f;
    }
}