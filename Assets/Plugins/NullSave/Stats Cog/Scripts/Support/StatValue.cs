using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    [CreateAssetMenu(menuName = "TOCK/Stats Cog/Stat Value", order = 2)]
    public class StatValue : ScriptableObject
    {

        #region Variables

        // UI
        public string category;
        public Sprite icon;
        public Color iconColor = Color.white;
        public string displayName;
        public Color textColor = Color.white;
        public bool displayInList = true;

        // Value range
        public string value = "0";
        public string minValue = "0";
        public string maxValue = "100";
        public bool startWithMaxValue = false;

        // Regeneration
        public bool enableRegen = false;
        public string regenDelay = "1";
        public string regenPerSecond = "1";

        // Incrementation
        public bool enableIncrement = false;
        public string incrementWhen = "1 > 2";
        public string incrementAmount = "0";
        public string[] incrementCommand;

        // Events
        public ValueChanged onBaseMinValueChanged, onBaseValueChanged, onBaseMaxValueChanged;
        public ValueChanged onMinValueChanged, onValueChanged, onMaxValueChanged;
        public UnityEvent onInit;

        // Internal current values
        private float curBaseValue, curBaseMin, curBaseMax, curBaseRegenAmount, curBaseRegenDelay;
        private float curValue, curMin, curMax, curRegenAmount, curRegenDelay;

        private float nextRegen;
        private bool processingIncrement, wantsReprocess;

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
        /// Current minimum value (without modifiers)
        /// </summary>
        public float CurrentBaseMinimum
        {
            get { return curBaseMin; }
            private set
            {
                if (curBaseMin == value) return;

                float oldValue = curBaseMin;
                curBaseMin = value;
                onBaseMinValueChanged?.Invoke(oldValue, value);

                ReCalcCurrentMin();
            }
        }

        public float CurrentBaseRegenAmount
        {
            get { return curBaseRegenAmount; }
            private set
            {
                if (curBaseRegenAmount == value) return;
                curBaseRegenAmount = value;

                // Apply modifiers
                float v = curBaseRegenAmount;
                foreach (StatModifier mod in ActiveModifiers)
                {
                    if (mod.valueTarget == EffectValueTarget.RegenAmount)
                    {
                        v += mod.AppliedValue;
                    }
                }
                CurrentRegenAmount = v;
            }
        }

        public float CurrentBaseRegenDelay
        {
            get { return curBaseRegenDelay; }
            private set
            {
                if (curBaseRegenDelay == value) return;
                curBaseRegenDelay = value;

                // Apply modifiers
                float v = curBaseRegenDelay;
                foreach (StatModifier mod in ActiveModifiers)
                {
                    if (mod.valueTarget == EffectValueTarget.RegenDelay)
                    {
                        v += mod.AppliedValue;
                    }
                }
                CurrentRegenDelay = v;
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
                value = Mathf.Clamp(value, curMin, curMax);
                if (curBaseValue == value) return;

                float oldValue = curBaseValue;
                curBaseValue = value;
                onBaseValueChanged?.Invoke(oldValue, value);

                if (enableRegen && oldValue > curBaseValue)
                {
                    nextRegen = CurrentRegenDelay;
                }

                if (curValue != unclamped)
                {
                    ReCalcCurrentValue(unclamped);
                }

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
        /// Current minimum value (with modifiers)
        /// </summary>
        public float CurrentMinimum
        {
            get { return curMin; }
            private set
            {
                if (curMin == value) return;

                float oldValue = curMin;
                curMin = value;
                onMinValueChanged?.Invoke(oldValue, value);
            }
        }

        public float CurrentRegenAmount { get; private set; }

        public float CurrentRegenDelay { get; private set; }

        /// <summary>
        /// Current value (with modifiers)
        /// </summary>
        public float CurrentValue
        {
            get { return curValue; }
            private set
            {
                value = Mathf.Clamp(value, curMin, curMax);
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
        /// Add an instant modifier x times
        /// </summary>
        /// <param name="mod">modifier</param>
        /// <param name="count">times to apply</param>
        public void AddInstantModifier(StatModifier mod, int count)
        {
            switch (mod.effectType)
            {
                case EffectTypes.Instant:
                    if (!value.IsNumeric())
                    {
                        Debug.Log(name + " only sustained modifiers can be used on stats with non-numeric values");
                        return;
                    }

                    mod.Initialize(this);
                    float change = mod.AppliedValue * count;
                    switch (mod.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            CurrentBaseMaximum += change;
                            break;
                        case EffectValueTarget.MinimumValue:
                            CurrentBaseMinimum += change;
                            break;
                        case EffectValueTarget.RegenAmount:
                            CurrentBaseRegenAmount += change;
                            break;
                        case EffectValueTarget.RegenDelay:
                            CurrentBaseRegenDelay += change;
                            break;
                        case EffectValueTarget.Value:
                            CurrentBaseValue += change;
                            break;
                    }
                    break;
            }
        }

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
                        Debug.Log(name + " only sustained modifiers can be used on stats with non-numeric values");
                        return;
                    }

                    mod.Initialize(this);
                    switch (mod.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            CurrentBaseMaximum += mod.AppliedValue;
                            break;
                        case EffectValueTarget.MinimumValue:
                            CurrentBaseMinimum += mod.AppliedValue;
                            break;
                        case EffectValueTarget.RegenAmount:
                            CurrentBaseRegenAmount += mod.AppliedValue;
                            break;
                        case EffectValueTarget.RegenDelay:
                            CurrentBaseRegenDelay += mod.AppliedValue;
                            break;
                        case EffectValueTarget.Value:
                            CurrentBaseValue += mod.AppliedValue;
                            break;
                    }
                    break;
                case EffectTypes.Recurring:
                    if (!value.IsNumeric())
                    {
                        Debug.Log(name + " only sustained modifiers can be used on stats with non-numeric values");
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
                            ReCalcCurrentMax();
                            break;
                        case EffectValueTarget.MinimumValue:
                            ReCalcCurrentMin();
                            break;
                        case EffectValueTarget.Value:
                            ReCalcCurrentValue(CurrentBaseValue);
                            break;
                        case EffectValueTarget.RegenAmount:
                            ReCalcCurrentRegenAmount();
                            break;
                        case EffectValueTarget.RegenDelay:
                            ReCalcCurrentRegenDelay();
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
            UpdateMinValue(0, 0);
            UpdateMaxValue(0, 0);
            UpdateValue(0, 0);
            UpdateRegenAmount(0, 0);
            UpdateRegenDelay(0, 0);
            nextRegen = -1;

            // Max
            if (startWithMaxValue && value.IsNumeric())
            {
                CurrentBaseValue = CurrentBaseMaximum;
            }

            // Finalize
            Initialized = true;
            if (onInit != null) onInit.Invoke();
        }

        /// <summary>
        /// Calculate the change in value when replacing one modifier with another
        /// </summary>
        /// <param name="original">Original Stat Modifier</param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public float GetModifierChange(StatModifier original, StatModifier replacement)
        {
            float val1 = 0;
            if (original != null)
            {
                val1 = original.AppliedValue;
            }

            float val2 = 0;
            if (replacement != null)
            {
                val2 = replacement.CalculateAppliedValue(this);
            }

            return val2 - val1;
        }

        /// <summary>
        /// Load StatValue data from stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream, float version)
        {
            if (version != 1.4f) return;

            name = stream.ReadStringPacket();
            curBaseValue = stream.ReadFloat();
            curBaseMin = stream.ReadFloat();
            curBaseMax = stream.ReadFloat();
            curBaseRegenAmount = stream.ReadFloat();
            curBaseRegenDelay = stream.ReadFloat();
            curValue = stream.ReadFloat();
            curMin = stream.ReadFloat();
            curMax = stream.ReadFloat();
            curRegenAmount = stream.ReadFloat();
            curRegenDelay = stream.ReadFloat();
            nextRegen = stream.ReadFloat();

            ActiveModifiers.Clear();
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                StatModifier sm = new StatModifier();
                sm.Load(stream);
                ActiveModifiers.Add(sm);
            }

            // Raise events
            if (onInit != null) onInit.Invoke();
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
            stream.WriteStringPacket(name);
            stream.WriteFloat(curBaseValue);
            stream.WriteFloat(curBaseMin);
            stream.WriteFloat(curBaseMax);
            stream.WriteFloat(curBaseRegenAmount);
            stream.WriteFloat(curBaseRegenDelay);
            stream.WriteFloat(curValue);
            stream.WriteFloat(curMin);
            stream.WriteFloat(curMax);
            stream.WriteFloat(curRegenAmount);
            stream.WriteFloat(curRegenDelay);
            stream.WriteFloat(nextRegen);
            stream.WriteInt(ActiveModifiers.Count);

            foreach (StatModifier modifier in ActiveModifiers)
            {
                modifier.Save(stream);
            }
        }

        /// <summary>
        /// Relay command to parent for evaluation
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            Parent.StartCoroutine(DoSendCommand(command));
        }

        /// <summary>
        /// Update StatValue
        /// </summary>
        public void Update()
        {
            // Regenerate
            if (enableRegen)
            {
                if (CurrentValue < CurrentMaximum)
                {
                    if (nextRegen == -1)
                    {
                        nextRegen = CurrentRegenDelay;
                    }
                    if (nextRegen > 0)
                    {
                        nextRegen -= Time.deltaTime;
                    }
                    else
                    {
                        CurrentBaseValue += CurrentRegenAmount * Time.deltaTime;
                    }
                }
                else if (CurrentValue == CurrentMaximum)
                {
                    nextRegen = -1;
                }
            }

            // Recurring modifiers
            foreach (StatModifier modifier in ActiveModifiers)
            {
                if (modifier.effectType  == EffectTypes.Recurring)
                {
                    switch (modifier.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            CurrentBaseMaximum += modifier.AppliedValue * Time.deltaTime;
                            break;
                        case EffectValueTarget.MinimumValue:
                            CurrentBaseMinimum += modifier.AppliedValue * Time.deltaTime;
                            break;
                        case EffectValueTarget.RegenAmount:
                            CurrentBaseRegenAmount += modifier.AppliedValue * Time.deltaTime;
                            break;
                        case EffectValueTarget.RegenDelay:
                            CurrentBaseRegenDelay += modifier.AppliedValue * Time.deltaTime;
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

        private IEnumerator DoSendCommand(string command)
        {
            yield return new WaitForEndOfFrame();
            Parent.SendCommand(command);
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

        private void ReCalcCurrentMin()
        {
            // Apply modifiers
            float v = curBaseMin;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.MinimumValue)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }
            CurrentMinimum = v;
            ReCalcCurrentValue(CurrentBaseValue);
        }

        private void ReCalcCurrentRegenAmount()
        {
            // Apply modifiers
            float v = curBaseRegenAmount;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.RegenAmount)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }

            if (v < 0) v = 0;
            CurrentRegenAmount = v;
        }

        private void ReCalcCurrentRegenDelay()
        {
            // Apply modifiers
            float v = curBaseRegenDelay;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.RegenDelay)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }

            if (v < 0) v = 0;
            CurrentRegenDelay = v;
        }

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
                                CurrentBaseValue = curBaseValue;
                                break;
                            case EffectValueTarget.MinimumValue:
                                ReCalcCurrentMin();
                                CurrentBaseValue = curBaseValue;
                                break;
                            case EffectValueTarget.Value:
                                ReCalcCurrentValue(CurrentValue - mod.AppliedValue);
                                break;
                            case EffectValueTarget.RegenAmount:
                                ReCalcCurrentRegenAmount();
                                break;
                            case EffectValueTarget.RegenDelay:
                                ReCalcCurrentRegenDelay();
                                break;
                        }
                    }
                    return;
                }
            }
        }

        protected internal void SetMaximum(float value)
        {
            CurrentBaseMaximum = value;
        }

        protected internal void SetMinimum(float value)
        {
            CurrentBaseMinimum = value;
        }

        protected internal void SetValue(float value)
        {
            value = Mathf.Clamp(value, CurrentBaseMinimum, CurrentBaseMaximum);
            if (enableRegen && value < CurrentBaseValue)
            {
                nextRegen = CurrentRegenDelay;
            }
            CurrentBaseValue = value;
        }

        private void Subscribe()
        {
            StatValue stat;
            List<StatValue> subs;
            List<string> req;

            // Min Value
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(minValue);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateMinValue);
                    subs.Add(stat);
                }
            }

            // Max Value
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(maxValue);
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
            req = Parent.GetSubscriptionRequirements(value);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateValue);
                    subs.Add(stat);
                }
            }

            // Regen Delay
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(regenDelay);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateRegenDelay);
                    subs.Add(stat);
                }
            }

            // Regen Amount
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(regenPerSecond);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateRegenAmount);
                    subs.Add(stat);
                }
            }

            // Increment
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(incrementWhen);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateIncrement);
                    subs.Add(stat);
                }
            }
        }

        private void UpdateIncrement(float oldValue, float newValue)
        {
            if (processingIncrement)
            {
                wantsReprocess = true;
                return;
            }

            if (Parent.EvaluateCondition(incrementWhen))
            {
                processingIncrement = true;
                CurrentBaseValue += Parent.GetExpressionValue(incrementAmount);
                if (incrementCommand != null)
                {
                    foreach (string command in incrementCommand)
                    {
                        Parent.SendCommand(command);
                    }
                }
                processingIncrement = false;
                if (wantsReprocess)
                {
                    wantsReprocess = false;
                    UpdateIncrement(0, 0);
                }
            }
        }

        private void UpdateMaxValue(float oldValue, float newValue)
        {
            CurrentBaseMaximum = Parent.GetExpressionValue(maxValue);
        }

        private void UpdateMinValue(float oldValue, float newValue)
        {
            CurrentBaseMinimum = Parent.GetExpressionValue(minValue);
        }

        private void UpdateRegenAmount(float oldValue, float newValue)
        {
            CurrentBaseRegenAmount = Parent.GetExpressionValue(regenPerSecond);
        }

        private void UpdateRegenDelay(float oldValue, float newValue)
        {
            CurrentBaseRegenDelay = Parent.GetExpressionValue(regenDelay);
        }

        private void UpdateValue(float oldValue, float newValue)
        {
            CurrentBaseValue = Parent.GetExpressionValue(value);
        }

        #endregion

    }
}