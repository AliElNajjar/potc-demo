using NullSave.TOCK.Stats;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventoryItem))]
    public class InventoryItemEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            Behaviour = 1,
            Stacking = 2,
            Equipping = 4,
            Animation = 8,
            UI = 16,
            Breakdown = 32,
            Repair = 64,
            Stas = 128,
            Tags = 256,
            Preview = 512,
            Attachment = 1024
        }

        #endregion

        #region Variables

        private DisplayFlags displayFlags;
        private ReorderableList repair, breakdown, equipPoints, attachPoints, uiTags;
        private Texture2D behaviourIcon, stackingIcon, equipIcon, animIcon, uiIcon, breakdownIcon, repairIcon, statsIcon, tagsIcon, previewIcon, attachmentIcon;

        #endregion

        #region Properties

        private Texture2D AnimateIcon
        {
            get
            {
                if (animIcon == null)
                {
                    animIcon = (Texture2D)Resources.Load("Icons/tock-animate", typeof(Texture2D));
                }

                return animIcon;
            }
        }

        private Texture2D AttachmentIcon
        {
            get
            {
                if (attachmentIcon == null)
                {
                    attachmentIcon = (Texture2D)Resources.Load("Icons/attachment", typeof(Texture2D));
                }

                return attachmentIcon;
            }
        }

        private Texture2D BehaviourIcon
        {
            get
            {
                if (behaviourIcon == null)
                {
                    behaviourIcon = (Texture2D)Resources.Load("Icons/tock-behaviour", typeof(Texture2D));
                }

                return behaviourIcon;
            }
        }

        private Texture2D BreakdownIcon
        {
            get
            {
                if (breakdownIcon == null)
                {
                    breakdownIcon = (Texture2D)Resources.Load("Icons/damage", typeof(Texture2D));
                }

                return breakdownIcon;
            }
        }

        private Texture2D EquipIcon
        {
            get
            {
                if (equipIcon == null)
                {
                    equipIcon = (Texture2D)Resources.Load("Icons/tock-equip", typeof(Texture2D));
                }

                return equipIcon;
            }
        }

        private Texture2D RepairIcon
        {
            get
            {
                if (repairIcon == null)
                {
                    repairIcon = (Texture2D)Resources.Load("Icons/tock-repair", typeof(Texture2D));
                }

                return repairIcon;
            }
        }

        private Texture2D StackingIcon
        {
            get
            {
                if (stackingIcon == null)
                {
                    stackingIcon = (Texture2D)Resources.Load("Icons/tock-stack", typeof(Texture2D));
                }

                return stackingIcon;
            }
        }

        private Texture2D StatsIcon
        {
            get
            {
                if (statsIcon == null)
                {
                    statsIcon = (Texture2D)Resources.Load("Icons/tock-stats", typeof(Texture2D));
                }

                return statsIcon;
            }
        }

        private Texture2D TagsIcon
        {
            get
            {
                if (tagsIcon == null)
                {
                    tagsIcon = (Texture2D)Resources.Load("Icons/tock-tag", typeof(Texture2D));
                }

                return tagsIcon;
            }
        }

        private Texture2D UIIcon
        {
            get
            {
                if (uiIcon == null)
                {
                    uiIcon = (Texture2D)Resources.Load("Icons/tock-ui", typeof(Texture2D));
                }

                return uiIcon;
            }
        }

        private Texture2D PreviewIcon
        {
            get
            {
                if (previewIcon == null)
                {
                    previewIcon = (Texture2D)Resources.Load("Icons/view", typeof(Texture2D));
                }

                return previewIcon;
            }
        }

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            repair = new ReorderableList(serializedObject, serializedObject.FindProperty("incrementComponents"), true, true, true, true);
            repair.elementHeight = (EditorGUIUtility.singleLineHeight * 2) + 4;
            repair.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Increment Components"); };
            SetupComponentsCallback(repair);

            breakdown = new ReorderableList(serializedObject, serializedObject.FindProperty("breakdownResult"), true, true, true, true);
            breakdown.elementHeight = (EditorGUIUtility.singleLineHeight * 2) + 4;
            breakdown.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Breakdown Result"); };
            SetupComponentsCallback(breakdown);

            equipPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("equipPoints"), true, true, true, true);
            equipPoints.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            equipPoints.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Equip Points"); };
            equipPoints.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = equipPoints.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
            };

            attachPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("attachPoints"), true, true, true, true);
            attachPoints.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            attachPoints.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Attach Points"); };
            attachPoints.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = attachPoints.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
            };


            uiTags = new ReorderableList(serializedObject, serializedObject.FindProperty("uiTags"), true, true, true, true);
            uiTags.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            uiTags.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Item Tags"); };
            uiTags.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = uiTags.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
            };
        }

        public override void OnInspectorGUI()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBegin("Inventory Item", "Icons/item_icon", false);

            DrawGeneral();
            DrawUI();
            if ((ItemType)SimpleInt("itemType") != ItemType.Skill)
            {
                if ((ItemType)SimpleInt("itemType") != ItemType.Attachment)
                {
                    DrawAttachments();
                }
                else
                {
                    DrawAttachmentSettings(false);
                }
                DrawRepairBreakdown();
            }
            DrawStats();
            DrawTags();

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void ComponentsAddCallback(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1).FindPropertyRelative("count").intValue = 1;

            var element = list.serializedProperty.GetArrayElementAtIndex(index);
        }

        private void DrawAttachments()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Attachment, "Attachments", AttachmentIcon))
            {
                if (!SimpleBool("canEquip"))
                {
                    GUILayout.Label("Only items that can be equipped can have attachments.", Skin.GetStyle("WrapText"));
                }
                else if (serializedObject.FindProperty("equipObject").objectReferenceValue == null)
                {
                    GUILayout.Label("An equip object needs to be supplied to support attachments.", Skin.GetStyle("WrapText"));
                }
                else
                {
                    SimpleProperty("attachRequirement");
                    switch ((AttachRequirement)SimpleInt("attachRequirement"))
                    {
                        case AttachRequirement.InCategory:
                            SubHeader("Categories", "attachCatsFilter", typeof(Category));
                            SimpleList("attachCatsFilter");
                            DrawSlots();
                            break;
                        case AttachRequirement.InItemList:
                            SubHeader("Items", "attachItemsFilter", typeof(InventoryItem));
                            SimpleList("attachItemsFilter");
                            DrawSlots();
                            break;
                    }
                }
            }
        }

        private void DrawAttachmentSettings(bool autoAttach)
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Attachment, "Attachment Settings", AttachmentIcon))
            {
                if (autoAttach)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField("Attach Object", "{Auto Creating}");
                    GUI.enabled = true;
                }
                else
                {
                    SimpleProperty("attachObject");
                }
                SimpleProperty("modifyName", "Modify Parent Name");
                if (SimpleBool("modifyName"))
                {
                    SimpleProperty("nameModifier");
                    SimpleProperty("modifierOrder");
                }
                attachPoints.DoLayoutList();
            }
        }

        private void DrawGeneral(bool autoLoot = false, bool autoEquip = false)
        {
            bool isSkill = (ItemType)SimpleInt("itemType") == ItemType.Skill;

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Behaviour, "Behaviour", BehaviourIcon))
            {

                SimpleProperty("category");
                SimpleProperty("itemType");
                if ((ItemType)serializedObject.FindProperty("itemType").intValue == ItemType.Weapon)
                {
                    SimpleProperty("usesAmmo");
                    if (serializedObject.FindProperty("usesAmmo").boolValue)
                    {
                        SimpleProperty("ammoType");
                        SimpleProperty("ammoPerUse");
                    }
                }
                else if ((ItemType)SimpleInt("itemType") == ItemType.Ammo)
                {
                    SimpleProperty("ammoType");
                }

                SimpleProperty("canDrop");
                if (serializedObject.FindProperty("canDrop").boolValue)
                {
                    if (autoLoot)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.TextField("Drop Object", "{Auto Creating}");
                        GUI.enabled = true;
                    }
                    else
                    {
                        SimpleProperty("dropObject");
                    }
                }
                SimpleProperty("displayInInventory", "Display in List");
                SimpleProperty("useSlotId");
                if (serializedObject.FindProperty("useSlotId").boolValue)
                {
                    SimpleProperty("slotId");
                }
                if (!isSkill)
                {
                    SimpleProperty("allowCustomName");
                }
                SimpleProperty("canSell");

            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Stacking, "Stacking", StackingIcon))
            {
                if ((ItemType)serializedObject.FindProperty("itemType").intValue == ItemType.Container)
                {
                    GUILayout.Label("Containers cannot be stacked", Skin.GetStyle("WrapText"));
                    serializedObject.FindProperty("canStack").boolValue = false;

                    SectionHeader("Storage");
                    SimpleProperty("mustEmptyToHold");
                    SimpleProperty("hasMaxStoreSlots", "Has Max Slots");
                    if (serializedObject.FindProperty("hasMaxStoreSlots").boolValue)
                    {
                        SimpleProperty("maxStoreSlots", "Max Slots");
                    }
                    SimpleProperty("hasMaxStoreWeight", "Has Max Weight");
                    if (serializedObject.FindProperty("hasMaxStoreWeight").boolValue)
                    {
                        SimpleProperty("maxStoreWeight", "Max Weight");
                    }
                }
                else
                {
                    SimpleProperty("canStack", "Stackable");
                    if (serializedObject.FindProperty("canStack").boolValue)
                    {
                        SimpleProperty("countPerStack");
                        SimpleProperty("hasMaxStacks");
                        if (serializedObject.FindProperty("hasMaxStacks").boolValue)
                        {
                            SimpleProperty("maxStacks");
                        }
                    }
                }
            }

            if (!isSkill)
            {
                if ((ItemType)SimpleInt("itemType") != ItemType.Attachment)
                {
                    if (SectionToggle((int)displayFlags, (int)DisplayFlags.Equipping, "Equipping", EquipIcon))
                    {
                        StatsBoolToggle("canEquip", "Can Equip", "equipSource", "equipExpression");
                        if (serializedObject.FindProperty("canEquip").boolValue)
                        {
                            SimpleProperty("canStore");
                            SimpleProperty("freeSlotWhenEquipped");
                            if (autoEquip)
                            {
                                GUI.enabled = false;
                                EditorGUILayout.TextField("Equip Object", "{Auto Creating}");
                                GUI.enabled = true;
                            }
                            else
                            {
                                SimpleProperty("equipObject");
                            }
                            SimpleProperty("autoEquip");
                            SimpleProperty("equipLocation");
                            GUILayout.Space(4);
                            equipPoints.DoLayoutList();

                            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Animation, "Animator Mods", AnimateIcon))
                            {
                                SectionHeader("On Equip");
                                SimpleList("equipAnimatorMods");

                                SectionHeader("On Un-equip");
                                SimpleList("unequipAnimatorMods");
                            }
                        }
                    }
                }
            }
            else
            {
                if (SectionToggle((int)displayFlags, (int)DisplayFlags.Animation, "Animator Mods", AnimateIcon))
                {
                    SectionHeader("On Skill Equip");
                    SimpleList("equipAnimatorMods");

                    SectionHeader("On Skill Un-equip");
                    SimpleList("unequipAnimatorMods");
                }
            }
        }

        private void DrawRepairBreakdown()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Repair, "Repair", RepairIcon))
            {
                SimpleProperty("condition");
                StatsBoolToggle("canRepair", "Can Repair", "repairSource", "repairExpression");
                if (serializedObject.FindProperty("canRepair").boolValue)
                {
                    SimpleProperty("repairIncrement");
                    SimpleProperty("incrementCost");
                    repair.DoLayoutList();
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Breakdown, "Breakdown", BreakdownIcon))
            {
                StatsBoolToggle("canBreakdown", "Can Breakdown", "breakdownSource", "breakdownExpression");
                if (serializedObject.FindProperty("canBreakdown").boolValue)
                {
                    SimpleProperty("breakdownCategory", "Category");
                    breakdown.DoLayoutList();
                }
            }
        }

        private void DrawSlots()
        {
            InventoryItem myTarget = null;
            AttachPoint[] points = null;

            if (target is InventoryItem) myTarget = (InventoryItem)target;
            if (myTarget != null && myTarget.equipObject != null) points = myTarget.equipObject.GetComponentsInChildren<AttachPoint>();

            SubHeader("Slots");
            //GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            if (points == null || points.Length == 0)
            {
                GUILayout.Label("None", Skin.GetStyle("WrapText"), GUILayout.ExpandWidth(true));
            }
            else
            {
                EditorGUILayout.BeginVertical();
                foreach (AttachPoint point in points)
                {
                    GUILayout.Label(point.pointId, GUILayout.ExpandWidth(true));

                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            //GUI.enabled = true;
            //SimpleList("attachSlots");
            //GUILayout.Space(2);
            //if (GUILayout.Button("Populate from Object"))
            //{
            //    InventoryItem item = (InventoryItem)target;
            //    item.attachSlots.Clear();
            //    AttachPoint[] points = item.equipObject.GetComponentsInChildren<AttachPoint>();
            //    foreach (AttachPoint point in points)
            //    {
            //        item.attachSlots.Add(new AttachmentSlot() { useAttachPoint = point.pointId });
            //    }
            //}
            GUILayout.Space(6);
        }

        private void DrawStats()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Stas, "Stats", StatsIcon))
            {
                SimpleProperty("rarity");
                SimpleProperty("weight");
                SimpleProperty("value");
                SectionHeader("Effects", "statEffects", typeof(StatEffect));
                SimpleList("statEffects");
            }
        }

        private void DrawTags()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Tags, "Custom Tags", TagsIcon))
            {
                SimpleList("customTags");
            }
        }

        private void DrawUI(bool autoPreview = false)
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.UI, "UI", UIIcon))
            {
                SimpleProperty("icon");
                SimpleProperty("displayName");
                SimpleProperty("subtext");
                SimpleProperty("description");
                SimpleProperty("displayRecipe");

                GUILayout.Space(12);
                DragBox(serializedObject.FindProperty("uiTags"), typeof(InventoryItemUITag), "Drag & Drop UI Tags Here");
                uiTags.DoLayoutList();
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Preview, "Preview", PreviewIcon))
            {
                if (autoPreview)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField("Object", "{Auto Creating}");
                    GUI.enabled = true;
                }
                else
                {
                    SimpleProperty("previewObject", "Object");
                }
                SimpleProperty("previewScale", "Scale");
            }
        }

        private void SetupComponentsCallback(ReorderableList list)
        {
            list.onAddCallback += ComponentsAddCallback;

            // Elements
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("item"), new GUIContent("Item", null, string.Empty));
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("count"), new GUIContent("Count", null, string.Empty));
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            };
        }

        private void StatsBoolToggle(string baseValue, string displayName, string sourceValue, string expressionValue)
        {
            if ((BooleanSource)serializedObject.FindProperty(sourceValue).intValue == BooleanSource.Static)
            {
                EditorGUILayout.BeginHorizontal();
                SimpleProperty(sourceValue, displayName);
                serializedObject.FindProperty(baseValue).boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty(baseValue).boolValue);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginVertical();
                SimpleProperty(sourceValue, displayName);
                SimpleProperty(expressionValue, " ");
                EditorGUILayout.EndVertical();
            }
        }

        #endregion

        #region Window Methods

        internal void DrawInspector(bool autoLoot, bool autoEquip, bool autoPreview, bool autoAttach)
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            DrawGeneral(autoLoot, autoEquip);
            DrawUI(autoPreview);
            if ((ItemType)SimpleInt("itemType") != ItemType.Skill)
            {
                if ((ItemType)SimpleInt("itemType") != ItemType.Attachment)
                {
                    DrawAttachments();
                }
                else
                {
                    DrawAttachmentSettings(autoAttach);
                }
                DrawRepairBreakdown();
            }
            DrawStats();
            DrawTags();

        }

        #endregion

    }
}