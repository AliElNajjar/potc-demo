using NullSave.TOCK.Stats;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.TOCK
{
    [CreateAssetMenu(menuName = "TOCK/Combat/Damage Modifier", order = 0)]
    public class DamageModifier : ScriptableObject
    {

        #region Variables

        public DamageType damageType;
        public DamageModType modifierType = DamageModType.Resistance;

        public string value = "0";
        public string maximum = "4";

        // Events
        public ValueChanged onBaseValueChanged, onValueChanged;
        public ValueChanged onBaseMaxValueChanged, onMaxValueChanged;

        private float curBaseValue, curValue;
        private float curBaseMax, curMax;

        #endregion

        #region Properties

        public List<StatModifier> ActiveModifiers { get; private set; }

        /// <summary>
        /// Current maximum value (without modifiers)
        /// </summary>
        public float CurrentBaseMaximum
        {
            get { return curBaseMax; }
            private set
            {
                if (curBaseMax == value) return;

                float oldValue = curBaseMax;
                curBaseMax = value;
                onBaseMaxValueChanged?.Invoke(oldValue, value);

                ReCalcCurrentMax();
            }
        }

        /// <summary>
        /// Current value (without modifiers)
        /// </summary>
        public float CurrentBaseValue
        {
            get { return curBaseValue; }
            private set
            {
                float unclamped = value;
                if (modifierType == DamageModType.Resistance)
                {
                    value = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    value = Mathf.Clamp(value, 0, curMax);
                }
                if (curBaseValue == value) return;

                if (curValue != unclamped)
                {
                    ReCalcCurrentValue(unclamped);
                }

                float oldValue = curBaseValue;
                curBaseValue = value;
                onBaseValueChanged?.Invoke(oldValue, value);
            }
        }

        /// <summary>
        /// Current maximum value (with modifiers)
        /// </summary>
        public float CurrentMaximum
        {
            get { return curMax; }
            private set
            {
                if (curMax == value) return;

                float oldValue = curMax;
                curMax = value;
                onMaxValueChanged?.Invoke(oldValue, value);
            }
        }

        /// <summary>
        /// Current value (with modifiers)
        /// </summary>
        public float CurrentValue
        {
            get { return curValue; }
            private set
            {
                if (modifierType == DamageModType.Resistance)
                {
                    value = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    value = Mathf.Clamp(value, 0, CurrentMaximum);
                }
                if (curValue == value) return;

                float oldValue = curValue;
                curValue = value;
                onValueChanged?.Invoke(oldValue, value);
            }
        }

        public bool Initialized { get; private set; }

        public StatsCog Parent { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a new modifer 
        /// </summary>
        /// <param name="mod"></param>
        public void AddModifier(StatModifier mod)
        {
            switch (mod.effectType)
            {
                case EffectTypes.Instant:
                    if (!value.IsNumeric())
                    {
                        Debug.Log(damageType + " only sustained modifiers can be used on stats with non-numeric values");
                        return;
                    }

                    mod.Initialize(this);
                    switch (mod.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            if (modifierType == DamageModType.Weakness)
                            {
                                CurrentBaseMaximum += mod.AppliedValue;
                            }
                            break;
                        case EffectValueTarget.Value:
                            CurrentBaseValue += mod.AppliedValue;
                            break;
                    }
                    break;
                case EffectTypes.Recurring:
                    if (!value.IsNumeric())
                    {
                        Debug.Log(damageType + " only sustained modifiers can be used on stats with non-numeric values");
                        return;
                    }
                    mod.Initialize(this);
                    ActiveModifiers.Add(mod);
                    break;
                case EffectTypes.Sustained:
                    mod.Initialized = false;
                    ActiveModifiers.Add(mod);
                    switch (mod.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            if (modifierType == DamageModType.Weakness)
                            {
                                ReCalcCurrentMax();
                            }
                            break;
                        case EffectValueTarget.Value:
                            ReCalcCurrentValue(CurrentBaseValue);
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Initialize Stat Value
        /// </summary>
        /// <param name="owner"></param>
        public void Initialize(StatsCog owner)
        {
            ActiveModifiers = new List<StatModifier>();

            // Set parent stats cog
            Parent = owner;

            // Subscribe
            Subscribe();

            // Set Base values
            if (modifierType == DamageModType.Weakness)
            {
                UpdateMaxValue(0, 0);
            }
            UpdateValue(0, 0);

            // Finalize
            Initialized = true;
        }

        /// <summary>
        /// Load StatValue data from stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream, float version)
        {
            if (version != 1.4f) return;

            damageType = DamageType.Load(stream);
            value = stream.ReadStringPacket();
            maximum = stream.ReadStringPacket();
            curBaseValue = stream.ReadFloat();
            curBaseMax = stream.ReadFloat();
            curValue = stream.ReadFloat();
            curMax = stream.ReadFloat();

            ActiveModifiers = new List<StatModifier>();
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                StatModifier sm = new StatModifier();
                sm.Load(stream);
                ActiveModifiers.Add(sm);
            }
        }

        /// <summary>
        /// Removes modifier from StatValue
        /// </summary>
        /// <param name="mod"></param>
        public void RemoveModifier(StatModifier mod)
        {
            if (mod.effectType != EffectTypes.Instant)
            {
                RemoveMod(mod);
            }
        }

        /// <summary>
        /// Save StatValue data to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            damageType.Save(stream);
            stream.WriteStringPacket(value);
            stream.WriteStringPacket(maximum);
            stream.WriteFloat(curBaseValue);
            stream.WriteFloat(curBaseMax);
            stream.WriteFloat(curValue);
            stream.WriteFloat(curMax);
            stream.WriteInt(ActiveModifiers.Count);

            foreach (StatModifier modifier in ActiveModifiers)
            {
                modifier.Save(stream);
            }
        }

        /// <summary>
        /// Update StatValue
        /// </summary>
        public void Update()
        {
            // Recurring modifiers
            foreach (StatModifier modifier in ActiveModifiers)
            {
                if (modifier.effectType == EffectTypes.Recurring)
                {
                    switch (modifier.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            CurrentBaseMaximum += modifier.AppliedValue * Time.deltaTime;
                            break;
                        case EffectValueTarget.Value:
                            CurrentBaseValue += modifier.AppliedValue * Time.deltaTime;
                            break;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void ReCalcCurrentValue(float value)
        {
            // Apply modifiers
            float v = value;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.Value)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }
            CurrentValue = v;
        }

        private void ReCalcCurrentMax()
        {
            // Apply modifiers
            float v = curBaseMax;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.MaximumValue)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }
            CurrentMaximum = v;
            ReCalcCurrentValue(CurrentBaseValue);
        }

        private void RemoveMod(StatModifier modifier)
        {
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.id == modifier.id)
                {
                    ActiveModifiers.Remove(mod);
                    if (mod.effectType == EffectTypes.Sustained)
                    {
                        switch (mod.valueTarget)
                        {
                            case EffectValueTarget.MaximumValue:
                                ReCalcCurrentMax();
                                break;
                            case EffectValueTarget.Value:
                                ReCalcCurrentValue(CurrentValue - mod.AppliedValue);
                                break;
                        }
                    }
                    return;
                }
            }
        }

        private void Subscribe()
        {
            StatValue stat;
            List<StatValue> subs;
            List<string> req;

            // Max Value
            subs = new List<StatValue>();
            req = SubscriptionRequirements(maximum);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateMaxValue);
                    subs.Add(stat);
                }
            }

            // Value
            subs = new List<StatValue>();
            req = SubscriptionRequirements(value);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateValue);
                    subs.Add(stat);
                }
            }
        }

        private List<string> SubscriptionRequirements(string equation)
        {
            List<string> res = new List<string>();
            if (!equation.IsNumeric())
            {
                equation = equation.Replace("(", "( ");
                equation = equation.Replace(")", " )");

                string[] parts = equation.Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i].Trim();

                    if (!parts[i].IsNumeric() && !LogicExtensions.BASIC_EXPRESSIONS.Contains(parts[i]))
                    {
                        res.Add(parts[i]);
                    }
                }
            }
            return res;
        }

        private void UpdateMaxValue(float oldValue, float newValue)
        {
            CurrentBaseMaximum = Parent.GetExpressionValue(maximum);
        }

        private void UpdateValue(float oldValue, float newValue)
        {
            CurrentBaseValue = Parent.GetExpressionValue(value);
        }

        #endregion

    }
}