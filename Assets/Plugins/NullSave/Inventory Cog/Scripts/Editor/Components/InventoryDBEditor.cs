using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryDB))]
    public class InventoryDBEditor : TOCKEditorV2
    {

        #region Variables

        private InventoryDB myTarget;
        internal ReorderableList categories, craftingCategories, items, recipes;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is InventoryDB)
            {
                myTarget = (InventoryDB)target;

                categories = new ReorderableList(serializedObject, serializedObject.FindProperty("categories"), true, true, true, true);
                categories.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                categories.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Item Categories"); };
                categories.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = categories.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((Category)element.objectReferenceValue).displayName, null, string.Empty));
                    }
                };

                items = new ReorderableList(serializedObject, serializedObject.FindProperty("availableItems"), true, true, true, true);
                items.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                items.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Items"); };
                items.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = items.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((InventoryItem)element.objectReferenceValue).DisplayName, null, string.Empty));
                    }
                };

                craftingCategories = new ReorderableList(serializedObject, serializedObject.FindProperty("craftingCategories"), true, true, true, true);
                craftingCategories.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                craftingCategories.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Crafting Categories"); };
                craftingCategories.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = craftingCategories.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((CraftingCategory)element.objectReferenceValue).displayName, null, string.Empty));
                    }
                };

                recipes = new ReorderableList(serializedObject, serializedObject.FindProperty("recipes"), true, true, true, true);
                recipes.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                recipes.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Recipes"); };
                recipes.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = recipes.serializedProperty.GetArrayElementAtIndex(index);
                    if (element.objectReferenceValue == null)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((CraftingRecipe)element.objectReferenceValue).displayName, null, string.Empty));
                    }
                };

            }
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Inventory DB", "Icons/category");

            LaunchEditor();

            DrawCategories();
            DrawItems();
            DrawCraftingCategories();
            DrawRecipes();

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void DrawCategories()
        {
            SectionHeader("Item Categories", "categories", typeof(Category));
            categories.DoLayoutList();
        }

        private void DrawItems()
        {
            SectionHeader("Items", "availableItems", typeof(InventoryItem));
            items.DoLayoutList();
        }

        private void DrawCraftingCategories()
        {
            SectionHeader("Crafting Categories", "craftingCategories", typeof(CraftingCategory));
            craftingCategories.DoLayoutList();
        }

        private void DrawRecipes()
        {
            SectionHeader("Recipes", "recipes", typeof(CraftingRecipe));
            recipes.DoLayoutList();
        }

        private void LaunchEditor()
        {
            if (GUILayout.Button("Launch Inventory DB Editor", GUILayout.MinHeight(32)))
            {
                InventoryDBWindow.Open();
            }
            GUILayout.BeginVertical();
            GUILayout.Space(16);
            GUILayout.EndVertical();
        }

        private void ReloadFromProject()
        {
            if (GUILayout.Button("Refresh from Project"))
            {
                // Clear items
                myTarget.availableItems.Clear();
                myTarget.categories.Clear();
                myTarget.recipes.Clear();
                myTarget.craftingCategories.Clear();

                string[] result = AssetDatabase.FindAssets("t:InventoryItem");
                foreach (string item in result)
                {
                    myTarget.availableItems.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(InventoryItem)) as InventoryItem);
                }

                result = AssetDatabase.FindAssets("t:Category");
                foreach (string item in result)
                {
                    myTarget.categories.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(Category)) as Category);
                }

                result = AssetDatabase.FindAssets("t:CraftingCategory");
                foreach (string item in result)
                {
                    myTarget.craftingCategories.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(CraftingCategory)) as CraftingCategory);
                }

                result = AssetDatabase.FindAssets("t:CraftingRecipe");
                foreach (string item in result)
                {
                    myTarget.recipes.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), typeof(CraftingRecipe)) as CraftingRecipe);
                }
            }
        }

        #endregion

        #region Window Methods

        public int DrawItemCategories()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawCategories();

            if (categories.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = categories.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        public int DrawItemList()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawItems();

            if (items.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = items.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        public int DrawCraftCategories()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawCraftingCategories();

            if (craftingCategories.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = craftingCategories.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        public int DrawCraftingRecipes()
        {
            int result = -1;
            GUILayout.BeginVertical(GUILayout.Width(300));
            DrawRecipes();

            if (craftingCategories.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = craftingCategories.index;
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            return result;
        }

        #endregion

    }
}
