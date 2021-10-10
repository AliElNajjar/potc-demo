using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class InventoryDBWindow : TOCKEditorWindow
    {

        #region Variables

        public InventoryDB db;
        private InventoryDBEditor dbEditor;

        private bool warnPrefab;
        private GameObject prefabRoot;
        private int selToolbarIndex;

        private static Texture2D dbIcon;
        private static GUIStyle warningLabel;
        private static readonly string[] toolbarOptions = new string[] { "Item Categories", "Items", "Crafting Categories", "Recipes" };

        // Item Categories
        private Category addItemCategory, editItemCategory;
        private CategoryEditor itemCategoryEditor;

        // Items
        private InventoryItem addItem, editItem;
        private InventoryItemEditor itemEditor;
        private GameObject itemBase;
        private bool autoLoot, autoEquip, autoPreview, autoAttachment;
        private Vector2 itemPos;

        // Crafting Categories
        private CraftingCategory addCraftCategory, editCraftCategory;
        private CraftingCategoryEditor craftCategoryEditor;

        // Recipes
        private CraftingRecipe addRecipe, editRecipe;
        private CraftingRecipeEditor recipeEditor;


        #endregion

        #region Properties

        private static Texture2D DBIcon
        {
            get
            {
                if (dbIcon == null)
                {
                    dbIcon = (Texture2D)Resources.Load("Icons/db-window", typeof(Texture2D));
                }

                return dbIcon;
            }
        }

        private static GUIStyle WarningLabel
        {
            get
            {
                if (warningLabel == null)
                {
                    warningLabel = new GUIStyle(EditorStyles.label);
                    warningLabel.normal.textColor = Color.yellow;
                }

                return warningLabel;
            }
        }

        #endregion

        #region Unity Methods

        private void OnGUI()
        {
            ContainerBegin();
            if (HaveDB())
            {
                if (warnPrefab)
                {
                    DrawPrefabWarning();
                }

                if (dbEditor != null)
                {
                    dbEditor.serializedObject.Update();
                }
                DrawButtons();
                switch (selToolbarIndex)
                {
                    case 0:
                        DrawItemCategories();
                        break;
                    case 1:
                        DrawItems();
                        break;
                    case 2:
                        DrawCraftingCategories();
                        break;
                    case 3:
                        DrawRecipes();
                        break;
                }
                if (dbEditor != null)
                {
                    dbEditor.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                GUILayout.Label("Please select an Inventory DB instance in the scene.");
            }
            ContainerEnd();

        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        #endregion

        #region Public Methods

        public static void Open()
        {
            InventoryDBWindow w = GetWindow<InventoryDBWindow>("Inventory DB Editor", typeof(SceneView));
            w.titleContent = new GUIContent("Inventory DB Editor", DBIcon);
            w.wantsMouseMove = true;
        }

        #endregion

        #region Private Methods

        private void ContainerBegin()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
        }

        private void ContainerEnd()
        {
            GUILayout.EndVertical();
            GUILayout.Space(8);
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            GUILayout.EndVertical();
        }

        private void DrawButtons()
        {
            GUILayout.BeginVertical();
            selToolbarIndex = GUILayout.Toolbar(selToolbarIndex, toolbarOptions, GUILayout.Width(600));
            GUILayout.EndVertical();
        }

        private void DrawItemCategories()
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(750));

            int editId = dbEditor.DrawItemCategories();
            if (editId > -1)
            {
                editItemCategory = db.categories[editId];
                itemCategoryEditor = (CategoryEditor)Editor.CreateEditor(editItemCategory);
            }

            GUILayout.BeginVertical(GUILayout.Width(300));
            if (editItemCategory == null)
            {
                if (addItemCategory == null)
                {
                    addItemCategory = (Category)ScriptableObject.CreateInstance(typeof(Category));
                    addItemCategory.name = "New Category";
                    itemCategoryEditor = (CategoryEditor)Editor.CreateEditor(addItemCategory);
                }

                itemCategoryEditor.serializedObject.Update();
                SectionHeader("Create Category");

                itemCategoryEditor.DrawInspector();

                GUILayout.Space(16);
                if (GUILayout.Button("Create and Add"))
                {
                    if (string.IsNullOrWhiteSpace(db.catFolder))
                    {
                        db.catFolder = Application.dataPath;
                    }
                    string path = EditorUtility.SaveFilePanelInProject("Save Category", addItemCategory.displayName, "asset", "Select a location to save the category", db.catFolder);
                    if (path.Length != 0)
                    {
                        db.catFolder = System.IO.Path.GetDirectoryName(path);

                        addItemCategory.name = System.IO.Path.GetFileNameWithoutExtension(path);
                        AssetDatabase.CreateAsset(addItemCategory, path);
                        AssetDatabase.SaveAssets();

                        if (db.categories == null)
                        {
                            db.categories = new System.Collections.Generic.List<Category>();
                        }
                        db.categories.Add(AssetDatabase.LoadAssetAtPath(path, typeof(Category)) as Category);
                        addItemCategory = null;
                    }
                }
                itemCategoryEditor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                itemCategoryEditor.serializedObject.Update();
                SectionHeader("Edit Category");

                itemCategoryEditor.DrawInspector();

                GUILayout.Space(16);
                if (GUILayout.Button("Done"))
                {
                    addItemCategory = null;
                    editItemCategory = null;
                    GUI.FocusControl("Clear");
                    Repaint();
                }
                else
                {
                    itemCategoryEditor.serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawItems()
        {
            SerializedObject serializedObject = new SerializedObject(db);

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(750));

            int editId = dbEditor.DrawItemList();
            if (editId > -1)
            {
                editItem = db.availableItems[editId];
                itemEditor = (InventoryItemEditor)Editor.CreateEditor(editItem);
            }

            GUILayout.BeginVertical(GUILayout.Width(300));
            if (editItem == null)
            {
                if (addItem == null)
                {
                    addItem = (InventoryItem)ScriptableObject.CreateInstance(typeof(InventoryItem));
                    addItem.name = "New Item";
                    itemEditor = (InventoryItemEditor)Editor.CreateEditor(addItem);
                }

                itemEditor.serializedObject.Update();
                SectionHeader("Create Item");

                itemPos = GUILayout.BeginScrollView(itemPos);
                itemEditor.DrawInspector(autoLoot, autoEquip, autoPreview, autoAttachment);
                GUILayout.EndScrollView();

                GUILayout.Space(16);
                if (GUILayout.Button("Create and Add"))
                {
                    if (itemBase == null && (autoEquip || autoLoot || autoPreview || autoAttachment))
                    {
                        EditorUtility.DisplayDialog("Inventory Cog", "You have selected at least one auto-create prefab but have not assigned an object to use.\r\n\r\nPlease provide one or turn off auto-create prefabs.", "OK");
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(db.itemFolder))
                        {
                            db.itemFolder = Application.dataPath;
                        }
                        string path = EditorUtility.SaveFilePanelInProject("Save ", addItem.displayName, "asset", "Select a location to save the item.", db.itemFolder);
                        if (path.Length != 0)
                        {
                            db.itemFolder = System.IO.Path.GetDirectoryName(path) + "/";

                            // Create new item
                            string baseFilename = System.IO.Path.GetFileNameWithoutExtension(path);
                            addItem.name = baseFilename;

                            AssetDatabase.CreateAsset(addItem, path);

                            // Create Loot item
                            if (autoLoot)
                            {
                                GameObject loot = Instantiate(itemBase);
                                loot.name = addItem.name + " (Loot)";
                                LootItem lootItem = loot.AddComponent<LootItem>();
                                lootItem.item = addItem;
                                lootItem.count = 1;
                                BoxCollider bc = loot.AddComponent<BoxCollider>();
                                bc.isTrigger = true;
                                addItem.dropObject = (PrefabUtility.SaveAsPrefabAsset(loot, db.itemFolder + loot.name + ".prefab")).GetComponent<LootItem>();
                                DestroyImmediate(loot);
                            }

                            // Create equip item
                            if (autoEquip)
                            {
                                GameObject equip = new GameObject();
                                equip.name = addItem.name + " (Equip)";
                                Instantiate(itemBase, equip.transform);
                                addItem.equipObject = PrefabUtility.SaveAsPrefabAsset(equip, db.itemFolder + equip.name + ".prefab");
                                DestroyImmediate(equip);
                            }

                            // Create preview item
                            if (autoPreview)
                            {
                                GameObject preview = new GameObject();
                                preview.name = addItem.name + " (Preview)";
                                Instantiate(itemBase, preview.transform);
                                addItem.previewObject = PrefabUtility.SaveAsPrefabAsset(preview, db.itemFolder + preview.name + ".prefab");
                                DestroyImmediate(preview);
                            }

                            // Create attachment item
                            if (autoAttachment)
                            {
                                GameObject attach = new GameObject();
                                attach.name = addItem.name + " (Attachment)";
                                Instantiate(itemBase, attach.transform);
                                addItem.attachObject = PrefabUtility.SaveAsPrefabAsset(attach, db.itemFolder + attach.name + ".prefab");
                                DestroyImmediate(attach);
                            }


                            AssetDatabase.SaveAssets();
                            db.availableItems.Add(AssetDatabase.LoadAssetAtPath(path, typeof(InventoryItem)) as InventoryItem);
                            addItem = null;
                            itemBase = null;
                            autoEquip = autoLoot = autoPreview = autoAttachment = false;
                        }
                    }
                }
                itemEditor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                itemEditor.serializedObject.Update();
                SectionHeader("Edit Item");

                itemPos = GUILayout.BeginScrollView(itemPos);
                itemEditor.DrawInspector(false, false, false, false);
                GUILayout.EndScrollView();

                GUILayout.Space(16);
                if (GUILayout.Button("Done"))
                {
                    addItem = null;
                    editItem = null;
                    GUI.FocusControl("Clear");
                    Repaint();
                }
                else
                {
                    itemEditor.serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();

            if (editItem == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(32);
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical(GUILayout.Width(300));
                SectionHeader("Auto Create Prefabs");

                itemBase = SimpleEditorGameObject("Base Model", itemBase);
                autoLoot = SimpleEditorBool("Loot Item", autoLoot);
                autoEquip = SimpleEditorBool("Equip Item", autoEquip);
                autoPreview = SimpleEditorBool("Item Preview", autoPreview);
                autoAttachment = SimpleEditorBool("Attachment Item", autoAttachment);

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

            }

            GUILayout.EndHorizontal();

        }

        private void DrawCraftingCategories()
        {
            SerializedObject serializedObject = new SerializedObject(db);

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(750));

            int editId = dbEditor.DrawCraftCategories();
            if (editId > -1)
            {
                editCraftCategory = db.craftingCategories[editId];
                craftCategoryEditor = (CraftingCategoryEditor)Editor.CreateEditor(editCraftCategory);
            }

            GUILayout.BeginVertical(GUILayout.Width(300));
            if (editCraftCategory == null)
            {
                if (addCraftCategory == null)
                {
                    addCraftCategory = (CraftingCategory)ScriptableObject.CreateInstance(typeof(CraftingCategory));
                    addCraftCategory.name = "New Crafting Category";
                    craftCategoryEditor = (CraftingCategoryEditor)Editor.CreateEditor(addCraftCategory);
                }

                craftCategoryEditor.serializedObject.Update();
                SectionHeader("Create Crafting Category");

                craftCategoryEditor.DrawInspector();

                GUILayout.Space(16);
                if (GUILayout.Button("Create and Add"))
                {
                    if (string.IsNullOrWhiteSpace(db.craftCatFolder))
                    {
                        db.craftCatFolder = Application.dataPath;
                    }
                    string path = EditorUtility.SaveFilePanelInProject("Save Crafting Category", addCraftCategory.displayName, "asset", "Select a location to save the crafting category", db.craftCatFolder);
                    if (path.Length != 0)
                    {
                        db.craftCatFolder = System.IO.Path.GetDirectoryName(path);

                        addCraftCategory.name = System.IO.Path.GetFileNameWithoutExtension(path);
                        AssetDatabase.CreateAsset(addCraftCategory, path);
                        AssetDatabase.SaveAssets();

                        db.craftingCategories.Add(AssetDatabase.LoadAssetAtPath(path, typeof(CraftingCategory)) as CraftingCategory);
                        addCraftCategory = null;
                    }
                }
                craftCategoryEditor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                craftCategoryEditor.serializedObject.Update();
                SectionHeader("Edit Crafting Category");

                craftCategoryEditor.DrawInspector();

                GUILayout.Space(16);
                if (GUILayout.Button("Done"))
                {
                    addCraftCategory = null;
                    editCraftCategory = null;
                    GUI.FocusControl("Clear");
                    Repaint();
                }
                else
                {
                    craftCategoryEditor.serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawRecipes()
        {
            SerializedObject serializedObject = new SerializedObject(db);

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(750));

            int editId = dbEditor.DrawCraftingRecipes();
            if (editId > -1)
            {
                editRecipe = db.recipes[editId];
                recipeEditor = (CraftingRecipeEditor)Editor.CreateEditor(editRecipe);
            }

            GUILayout.BeginVertical(GUILayout.Width(300));
            if (editRecipe == null)
            {
                if (addRecipe == null)
                {
                    addRecipe = (CraftingRecipe)ScriptableObject.CreateInstance(typeof(CraftingRecipe));
                    addRecipe.name = "New Recipe";
                    recipeEditor = (CraftingRecipeEditor)Editor.CreateEditor(addRecipe);
                }

                craftCategoryEditor.serializedObject.Update();
                SectionHeader("Create Recipe");

                recipeEditor.DrawInspector();

                GUILayout.Space(16);
                if (GUILayout.Button("Create and Add"))
                {
                    if (string.IsNullOrWhiteSpace(db.recipeFolder))
                    {
                        db.recipeFolder = Application.dataPath;
                    }
                    string path = EditorUtility.SaveFilePanelInProject("Save Crafting Category", addRecipe.displayName, "asset", "Select a location to save the crafting category", db.recipeFolder);
                    if (path.Length != 0)
                    {
                        db.recipeFolder = System.IO.Path.GetDirectoryName(path);

                        addRecipe.name = System.IO.Path.GetFileNameWithoutExtension(path);
                        AssetDatabase.CreateAsset(addRecipe, path);
                        AssetDatabase.SaveAssets();

                        db.recipes.Add(AssetDatabase.LoadAssetAtPath(path, typeof(CraftingRecipe)) as CraftingRecipe);
                        addRecipe = null;
                    }
                }
                craftCategoryEditor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                craftCategoryEditor.serializedObject.Update();
                SectionHeader("Edit Recipe");

                recipeEditor.DrawInspector();

                GUILayout.Space(16);
                if (GUILayout.Button("Done"))
                {
                    addRecipe = null;
                    editRecipe = null;
                    GUI.FocusControl("Clear");
                    Repaint();
                }
                else
                {
                    craftCategoryEditor.serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawPrefabWarning()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            //if (GUILayout.Button("Edit Prefab", GUILayout.Width(100)))
            //{
            //    Selection.activeObject = AssetDatabase.LoadAssetAtPath(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot), typeof(Object));
            //    AssetDatabase.OpenAsset(Selection.activeObject);
            //    db = ((GameObject)Selection.activeObject).GetComponent<InventoryDB>();
            //}
            //GUILayout.Label("You are editing a prefab instance, this will not save to directly to the prefab.", WarningLabel);
            GUILayout.Label("This version of the editor does not work well with prefabs. Consider using a single instance or unpacking and replacing the prefab when you're done editing.", WarningLabel);

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private bool HaveDB()
        {
            GameObject activeObj = Selection.activeObject as GameObject;

            if (activeObj == null)
            {
                db = null;
                return false;
            }

            prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(activeObj);

            warnPrefab = prefabRoot != null;

            InventoryDB newDB = activeObj.GetComponent<InventoryDB>();

            if (db != newDB || (db == null && newDB != null))
            {
                db = newDB;
                if (db == null)
                {
                    dbEditor = null;

                    editItemCategory = addItemCategory = null;
                    itemCategoryEditor = null;

                    editItem = addItem = null;
                    itemEditor = null;

                    editCraftCategory = addCraftCategory = null;
                    craftCategoryEditor = null;

                    editRecipe = addRecipe = null;
                    recipeEditor = null;

                    return false;
                }
                else
                {
                    dbEditor = (InventoryDBEditor)Editor.CreateEditor(db, typeof(InventoryDBEditor));
                    return true;
                }
            }

            return db != null;
        }

        #endregion

    }
}