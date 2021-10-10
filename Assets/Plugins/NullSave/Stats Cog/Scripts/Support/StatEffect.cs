using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    [CreateAssetMenu(menuName = "TOCK/Stats Cog/Stat Effect", order = 0)]
    public class StatEffect : ScriptableObject
    {

        #region Variables

        public string displayName;
        public string description;
        public Sprite sprite;
        public bool displayInList = true;
        public string category = "Default";
        public string startedText;
        public string endedText;
        public string removedText;
        public bool isDetrimental, isBenificial;
        public bool isRemoveable = true;

        public GameObject effectParticles;

        public bool canStack = false;

        public List<StatModifier> modifiers;
        public List<StatEffect> cancelEffects;
        public List<StatEffect> preventEffects;

        public bool hasLifeSpan = false;
        public float lifeInSeconds = 0;
        [Tooltip("Reset lifespan if effect is added again")] public bool resetLifeOnAdd = false;

        public UnityEvent onStart, onEnd;

        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public bool IsDead
        {
            get
            {
                if (hasLifeSpan && RemainingTime <= 0) return true;
                return false;
            }
        }

        public float RemainingTime { get; set; }

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            if (modifiers != null)
            {
                foreach (StatModifier mod in modifiers)
                {
                    mod.Initialized = false;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize effect
        /// </summary>
        public void Initialize()
        {
            if (hasLifeSpan)
            {
                RemainingTime = lifeInSeconds;
            }
        }

        public bool IsEffectCancelled(string effectName)
        {
            foreach (StatEffect effect in cancelEffects)
            {
                if (effect != null && effect.name == effectName)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsEffectPrevented(string effectName)
        {
            foreach (StatEffect effect in preventEffects)
            {
                if (effect != null && effect.name == effectName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Update remaining life of effect
        /// </summary>
        public void Update()
        {
            if (hasLifeSpan)
            {
                RemainingTime -= Time.deltaTime;
            }
        }

        #endregion

    }
}