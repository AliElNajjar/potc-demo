using UnityEngine;
using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventoryCog))]
    public class InventoryCogEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            UI = 1,
            Recipies = 2,
            FailedCraft = 4,
            StartingItems = 8,
            Events = 16,
            EquipPoints = 32,
            StorePoints = 64,
            Skills = 128
        }

        #endregion

        #region Variables

        private bool inAddItem;
        private int selItem;
        private InventoryCog myTarget;
        private DisplayFlags displayFlags;
        private Texture2D uiIcon, recipeIcon, itemsIcon, eventsIcon, equipIcon, storeIcon, skillIcon;

        private string[] views = new string[] { "Debug", "Default" };
        private string[] spawnDrop = new string[] { "Automatic", "Manual" };
        private Vector2 itemScroll;

        private int boneIndex, boneIndex2;

        #endregion

        #region Properties

        private Texture2D EquipIcon
        {
            get
            {
                if (equipIcon == null)
                {
                    equipIcon = (Texture2D)Resources.Load("Icons/equip_point", typeof(Texture2D));
                }

                return equipIcon;
            }
        }

        private Texture2D EventsIcon
        {
            get
            {
                if (eventsIcon == null)
                {
                    eventsIcon = (Texture2D)Resources.Load("Icons/tock-event", typeof(Texture2D));
                }

                return eventsIcon;
            }
        }

        private Texture2D ItemsIcon
        {
            get
            {
                if (recipeIcon == null)
                {
                    recipeIcon = (Texture2D)Resources.Load("Icons/loot_item", typeof(Texture2D));
                }

                return recipeIcon;
            }
        }

        private Texture2D RecipeIcon
        {
            get
            {
                if (recipeIcon == null)
                {
                    recipeIcon = (Texture2D)Resources.Load("Icons/tock-book", typeof(Texture2D));
                }

                return recipeIcon;
            }
        }

        private Texture2D SkillIcon
        {
            get
            {
                if (skillIcon == null)
                {
                    skillIcon = (Texture2D)Resources.Load("Icons/skill", typeof(Texture2D));
                }

                return skillIcon;
            }
        }

        private Texture2D StoreIcon
        {
            get
            {
                if (storeIcon == null)
                {
                    storeIcon = (Texture2D)Resources.Load("Icons/store_point", typeof(Texture2D));
                }

                return storeIcon;
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

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            myTarget = (InventoryCog)target;
            FixObsolete();
        }

        public override void OnInspectorGUI()
        {
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBeginSlim();

            if (Application.isPlaying)
            {
                ViewSelect(views);
                if (View == 0)
                {
                    DrawDebug();
                    MainContainerEnd();
                    return;
                }
            }

            DrawBehaviour();
            DrawStarting();
            DrawEquipPoints();
            DrawStorePoints();
            DrawSkills();
            DrawCrafting();

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Events, "Events", EventsIcon))
            {
                SimpleProperty("onItemDropped");
                SimpleProperty("onItemAdded");
                SimpleProperty("onSpawnDropRequested");
                SimpleProperty("onItemEquipped");
                SimpleProperty("onItemStored");
                SimpleProperty("onItemUnequipped");
                SimpleProperty("onCraftingFailed");
                SimpleProperty("onMenuOpen");
                SimpleProperty("onMenuClose");
            }

            MainContainerEnd();
        }

        public override bool RequiresConstantRepaint()
        {
            if (!Application.isPlaying || View > 0) return false;
            return true;
        }

        #endregion

        #region Private Methods

        private void DrawBehaviour()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.UI, "UI", UIIcon))
            {
                SectionHeader("Pickup & Drop");
                int spawnSel = SimpleBool("spawnDropManually") ? 1 : 0;
                SimpleBool("spawnDropManually", EditorGUILayout.Popup("Spawn Drop", spawnSel, spawnDrop) == 1);
                if (!SimpleBool("spawnDropManually")) SimpleProperty("dropOffset");
                SimpleProperty("pickupUI", "Prompt");
                SimpleProperty("pickupPopup", "Prompt Settings");

                SectionHeader("Menu");
                SimpleProperty("menuMode", "Mode");
                switch ((NavigationType)serializedObject.FindProperty("menuMode").intValue)
                {
                    case NavigationType.ByButton:
                        SimpleProperty("menuButton", "Button");
                        break;
                    case NavigationType.ByKey:
                        SimpleProperty("menuKey", "Key");
                        break;
                }
                SimpleProperty("menu", "Client");
                SimpleProperty("openType", "Open Method");
                switch ((MenuOpenType)serializedObject.FindProperty("openType").intValue)
                {
                    case MenuOpenType.SpawnInTransform:
                        SimpleProperty("menuContainer", "Parent Transform");
                        break;
                    case MenuOpenType.SpawnInTag:
                        SimpleProperty("menuSpawnTag", "Target Tag");
                        break;
                }

                SectionHeader("Containers");
                SimpleProperty("containerMenu", "Client");
                SimpleProperty("containerUI", "Prompt");
                SimpleProperty("containerPopup", "Prompt Settings");

                SectionHeader("Merchants");
                SimpleProperty("merchantMenu", "Client");
                SimpleProperty("merchantUI", "Prompt");
                SimpleProperty("merchantPopup", "Prompt Settings");

                SectionHeader("Item Renaming");
                SimpleProperty("renamePrompt");
                SimpleProperty("renameOpenType", "Open Method");
                switch ((MenuOpenType)serializedObject.FindProperty("renameOpenType").intValue)
                {
                    case MenuOpenType.SpawnInTransform:
                        SimpleProperty("renameContainer", "Parent Transform");
                        break;
                    case MenuOpenType.SpawnInTag:
                        SimpleProperty("renameSpawnTag", "Target Tag");
                        break;
                }
            }
        }

        private void DrawCrafting()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Recipies, "Failed Crafting", RecipeIcon))
            {
                SimpleList("failedResult");
            }
        }

        private void DrawDebug()
        {
            SectionHeader("Debug Data");
            if (myTarget.Items == null || myTarget.Items.Count == 0)
            {
                GUILayout.Label("{Inventory Empty}");
            }
            else
            {
                string displayName;
                for (int i = 0; i < myTarget.Items.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    displayName = myTarget.Items[i].DisplayName;
                    if (myTarget.Items[i].useSlotId)
                    {
                        displayName += " [slot " + myTarget.Items[i].slotId + "]";
                    }
                    GUILayout.Label(displayName);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("x" + myTarget.Items[i].CurrentCount);
                    GUILayout.EndHorizontal();
                }
            }
        }

        private void DrawEquipPoints()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.EquipPoints, "Equip Points", EquipIcon))
            {
                EquipPoint[] points = myTarget.GetComponentsInChildren<EquipPoint>();
                foreach (EquipPoint point in points)
                {
                    EditorGUILayout.ObjectField(point, typeof(EquipPoint), true);
                }
                if (points.Length == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    GUILayout.Label("{None}", Skin.GetStyle("SubHeader"));
                    GUILayout.EndHorizontal();
                }

                Animator anim = myTarget.GetComponentInChildren<Animator>();
                if (anim != null && anim.isHuman)
                {
                    SubHeader("Dynamic Creation");
                    boneIndex = EditorGUILayout.Popup("Add to Bone", boneIndex, bones, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Add"))
                    {
                        Transform target = anim.GetBoneTransform((HumanBodyBones)boneIndex);
                        if (target == null)
                        {
                            EditorUtility.DisplayDialog("Inventgory Cog", "The requested bone '" + bones[boneIndex] + "' could not be found on the selected rig.", "OK");
                        }
                        else
                        {
                            GameObject newDD = new GameObject();
                            newDD.name = "EquipPoint_" + bones[boneIndex];
                            newDD.AddComponent<EquipPoint>().pointId = bones[boneIndex];
                            newDD.transform.SetParent(target);
                            newDD.transform.localPosition = Vector3.zero;
                            Selection.activeGameObject = newDD;
                        }
                    }
                }
            }

        }

        private void DrawStorePoints()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.StorePoints, "Store Points", StoreIcon))
            {
                StorePoint[] points = myTarget.GetComponentsInChildren<StorePoint>();
                foreach (StorePoint point in points)
                {
                    EditorGUILayout.ObjectField(point, typeof(StorePoint), true);
                }
                if (points.Length == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(24);
                    GUILayout.Label("{None}", Skin.GetStyle("SubHeader"));
                    GUILayout.EndHorizontal();
                }

                Animator anim = myTarget.GetComponentInChildren<Animator>();
                if (anim != null && anim.isHuman)
                {
                    SubHeader("Dynamic Creation");
                    boneIndex = EditorGUILayout.Popup("Add to Bone", boneIndex, bones, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Add"))
                    {
                        Transform target = anim.GetBoneTransform((HumanBodyBones)boneIndex);
                        if (target == null)
                        {
                            EditorUtility.DisplayDialog("Inventgory Cog", "The requested bone '" + bones[boneIndex] + "' could not be found on the selected rig.", "OK");
                        }
                        else
                        {
                            GameObject newDD = new GameObject();
                            newDD.name = "StorePoint_" + bones[boneIndex];
                            newDD.AddComponent<StorePoint>().pointId = bones[boneIndex] + " Store";
                            newDD.transform.SetParent(target);
                            newDD.transform.localPosition = Vector3.zero;
                            Selection.activeGameObject = newDD;
                        }
                    }
                }
            }

        }

        private void DrawSkills()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Skills, "Skill Slots", SkillIcon))
            {
                SimpleList("skillSlots");
            }
        }

        private void DrawStarting()
        {
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.StartingItems, "Starting Items", ItemsIcon))
            {
                SerializedProperty list = serializedObject.FindProperty("startingItems");
                int startSize = list.arraySize;
                itemScroll = SimpleList("startingItems", itemScroll, 120, 2);
                if (list.arraySize > startSize)
                {
                    SerializedProperty newItem = list.GetArrayElementAtIndex(list.arraySize - 1);
                    newItem.FindPropertyRelative("item").objectReferenceValue = null;
                    newItem.FindPropertyRelative("count").intValue = 1;
                }
            }
        }

        private void FixObsolete()
        {
            if (myTarget.pickupPopup != null && !myTarget.pickupPopup.obsoleteFixed)
            {
                myTarget.pickupPopup.detection = myTarget.pickupDetection;
                myTarget.pickupPopup.raycastCulling = myTarget.raycastCulling;
                myTarget.pickupPopup.raycastOffset = myTarget.raycastOffset;
                myTarget.pickupPopup.maxDistance = myTarget.maxDistance;
                myTarget.pickupPopup.selectButton = myTarget.pickupButton;
                myTarget.pickupPopup.selectKey = myTarget.pickupKey;
                myTarget.pickupPopup.obsoleteFixed = true;
            }

            if (myTarget.containerPopup != null && !myTarget.containerPopup.obsoleteFixed)
            {
                myTarget.containerPopup.detection = myTarget.containerDetection;
                myTarget.containerPopup.maxDistance = myTarget.maxDistance;
                switch (myTarget.containerMode)
                {
                    case NavigationType.ByButton:
                        myTarget.containerPopup.selection = PickupType.ByButton;
                        break;
                    case NavigationType.ByKey:
                        myTarget.containerPopup.selection = PickupType.ByKey;
                        break;
                    case NavigationType.Manual:
                        myTarget.containerPopup.selection = PickupType.Manual;
                        break;
                }
                myTarget.containerPopup.selectButton = myTarget.containerButton;
                myTarget.containerPopup.selectKey = myTarget.containerKey;
                myTarget.containerPopup.openType = myTarget.containerOpenType;
                myTarget.containerPopup.container = myTarget.containerMenuContainer;
                myTarget.containerPopup.spawnTag = myTarget.containerSpawnTag;
                myTarget.containerPopup.obsoleteFixed = true;
            }

            if (myTarget.merchantPopup != null && !myTarget.merchantPopup.obsoleteFixed)
            {
                if (!myTarget.merchantPopup.obsoleteFixed)
                {
                    myTarget.merchantPopup.detection = myTarget.merchantDetection;
                    myTarget.merchantPopup.maxDistance = myTarget.maxDistance;
                    switch (myTarget.merchantMode)
                    {
                        case NavigationType.ByButton:
                            myTarget.merchantPopup.selection = PickupType.ByButton;
                            break;
                        case NavigationType.ByKey:
                            myTarget.merchantPopup.selection = PickupType.ByKey;
                            break;
                        case NavigationType.Manual:
                            myTarget.merchantPopup.selection = PickupType.Manual;
                            break;
                    }
                    myTarget.merchantPopup.selectButton = myTarget.merchantButton;
                    myTarget.merchantPopup.selectKey = myTarget.merchantKey;
                    myTarget.merchantPopup.openType = myTarget.merchantOpenType;
                    myTarget.merchantPopup.container = myTarget.merchantMenuMerchant;
                    myTarget.merchantPopup.spawnTag = myTarget.merchantSpawnTag;
                    myTarget.merchantPopup.obsoleteFixed = true;
                }
            }
        }

        #endregion

    }
}