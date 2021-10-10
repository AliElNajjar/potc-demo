using NullSave.TOCK.Stats;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class StatModifierUI : MonoBehaviour
    {

        #region Variables

        public Image icon;
        public TextMeshProUGUI displayText;
        public TextMeshProUGUI oldValue;
        public TextMeshProUGUI newValue;
        public bool applyTextColor = true;
        public TextMeshProUGUI statName;
        public TextMeshProUGUI modifierValue;

        public List<GameObject> hideWhenEquipped;
        public bool reduceSizeOnHide = true;

        private StatsCog _statsCog;
        private InventoryItem _item;
        private StatModifier _modifier;
        private InventoryCog _inventory;

        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            UpdateValues();
            HideShowEquipped();
        }

        #endregion

        #region Public Methods

        public void LoadModifier(InventoryCog inventory, InventoryItem item, StatModifier modifier)
        {
            _item = item;
            _modifier = modifier;
            _inventory = inventory;
            _statsCog = inventory.gameObject.GetComponent<StatsCog>();

            if (icon != null)
            {
                icon.sprite = modifier.icon;
                icon.enabled = modifier.icon != null;
            }

            if (displayText != null)
            {
                displayText.text = modifier.displayText;
                if (applyTextColor)
                {
                    displayText.color = modifier.textColor;
                }
            }

            if(statName != null)
            {
                statName.text = modifier.affectedStat;
            }

            if(modifierValue != null)
            {
                modifierValue.text = _statsCog.GetExpressionValue(modifier.value).ToString();
            }

            UpdateValues();
            HideShowEquipped();
        }

        #endregion

        #region Private Methods

        private void HideShowEquipped()
        {
            RectTransform rt = GetComponent<RectTransform>();
            RectTransform rtTarget;

            foreach (GameObject go in hideWhenEquipped)
            {
                if (_item.EquipState != EquipState.NotEquipped)
                {
                    if (go.activeSelf)
                    {
                        rtTarget = go.GetComponent<RectTransform>();
                        if (rtTarget != null)
                        {
                            rt.sizeDelta = new Vector2(rt.sizeDelta.x - rtTarget.sizeDelta.x, rt.sizeDelta.y);
                        }
                        go.SetActive(false);
                    }
                }
                else
                {
                    if (!go.activeSelf)
                    {
                        go.SetActive(true);
                        rtTarget = go.GetComponent<RectTransform>();
                        if (rtTarget != null)
                        {
                            rt.sizeDelta = new Vector2(rt.sizeDelta.x + rtTarget.sizeDelta.x, rt.sizeDelta.y);
                        }
                    }
                }
            }
        }

        private void UpdateValues()
        {
            InventoryItem orgItem = null;
            if (_item.canEquip && _item.itemType != ItemType.Attachment)
            {
                // Find where we will equip the item
                EquipPoint orgPoint;
                if (_item.EquipState != EquipState.NotEquipped)
                {
                    orgPoint = _item.CurrentEquipPoint;
                }
                else
                {
                    orgPoint = _inventory.GetPointToUse(_item);
                }

                orgItem = orgPoint.EquipedOrStoredItem;
            }

            StatModifier orgModifer = null;
            if (orgItem != null)
            {
                foreach (StatEffect effect in orgItem.statEffects)
                {
                    foreach (StatModifier mod in effect.modifiers)
                    {
                        if (mod.affectedStat == _modifier.affectedStat && mod.valueTarget == _modifier.valueTarget)
                        {
                            orgModifer = mod;
                            break;
                        }
                    }
                }
            }

            if (oldValue != null)
            {
                if (orgModifer != null)
                {
                    oldValue.text = orgModifer.AppliedValue.ToString();
                }
                else
                {
                    oldValue.text = "0";
                }
            }

            if (newValue != null)
            {
                if(_item.EquipState != EquipState.NotEquipped && !_item.IsAttached)
                {
                    newValue.text = "0";
                }
                else
                {
                    newValue.text = _statsCog.GetExpressionValue(_modifier.value).ToString();
                }
            }

        }

        #endregion

    }
}
