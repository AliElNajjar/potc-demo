using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class EnemyInfoList : MonoBehaviour
    {

        #region Enumerations

        public enum InfoTarget
        {
            Resistances = 0,
            Weaknesses = 1,
            Immunities = 2
        }

        #endregion

        #region Variables

        public StatsCog statsCog;
        public InfoTarget targetInfo;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (statsCog == null) return;

            string list = string.Empty;

            TextMeshProUGUI tmpro = GetComponent<TextMeshProUGUI>();
            switch (targetInfo)
            {
                case InfoTarget.Immunities:
                    foreach (DamageModifier modifier in statsCog.damageModifiers)
                    {
                        if (modifier.modifierType == DamageModType.Resistance && modifier.CurrentValue == 1)
                        {
                            if (list == string.Empty)
                            {
                                list = modifier.damageType.displayName;
                            }
                            else
                            {
                                list += ", " + modifier.damageType.displayName;
                            }
                        }
                    }
                    break;
                case InfoTarget.Resistances:
                    foreach (DamageModifier modifier in statsCog.damageModifiers)
                    {
                        if (modifier.modifierType == DamageModType.Resistance && modifier.CurrentValue < 1)
                        {
                            if (list == string.Empty)
                            {
                                list = modifier.damageType.displayName;
                            }
                            else
                            {
                                list += ", " + modifier.damageType.displayName;
                            }
                        }
                    }
                    break;
                case InfoTarget.Weaknesses:
                    foreach (DamageModifier modifier in statsCog.damageModifiers)
                    {
                        if (modifier.modifierType == DamageModType.Weakness)
                        {
                            if (list == string.Empty)
                            {
                                list = modifier.damageType.displayName;
                            }
                            else
                            {
                                list += ", " + modifier.damageType.displayName;
                            }
                        }
                    }
                    break;
            }

            if (list == string.Empty) list = "None";
            tmpro.text = list;
        }

        #endregion

    }
}