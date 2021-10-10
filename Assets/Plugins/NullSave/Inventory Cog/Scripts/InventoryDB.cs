using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [DefaultExecutionOrder(-101)]
    [HierarchyIcon("category")]
    public class InventoryDB : MonoBehaviour
    {

        #region Variables

        public bool loadDatabaseAutomatically;

        public List<Category> categories;
        public List<CraftingCategory> craftingCategories;
        public List<InventoryItem> availableItems;
        public List<CraftingRecipe> recipes;

        public string catFolder, craftCatFolder, itemFolder, recipeFolder;

        #endregion

        #region Properties

        public static List<InventoryItem> AvailableItems { get { CreateInstance(); return Instance.availableItems; } }

        public static List<Category> Categories { get { CreateInstance(); return Instance.categories; } }

        public static List<CraftingCategory> CraftingCategories { get { CreateInstance(); return Instance.craftingCategories; } }

        public static InventoryDB Instance { get; private set; }

        public static List<CraftingRecipe> Recipes { get { CreateInstance(); return Instance.recipes; } }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (loadDatabaseAutomatically)
                {
                    RefreshDatabase();
                }
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve an item from the list of all inventory items by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static InventoryItem GetItemByName(string name)
        {
            foreach (InventoryItem item in AvailableItems)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a list of all items in a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static List<InventoryItem> GetItems(string categoryName)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in AvailableItems)
            {
                if (item.category.name == categoryName)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all items in a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static List<InventoryItem> GetItems(Category category)
        {
            return GetItems(category.name);
        }

        /// <summary>
        /// Get a list of all items in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static List<InventoryItem> GetItems(List<string> categories)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in AvailableItems)
            {
                if (categories.Contains(item.category.name))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all items in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static List<InventoryItem> GetItems(List<Category> categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (Category category in categories)
            {
                categoryNames.Add(category.name);
            }

            return GetItems(categoryNames);
        }

        public static void RefreshDatabase()
        {
            CreateInstance();
            Instance.UpdateDatabase();
        }

        #endregion

        #region Private Methods

        private static void CreateInstance()
        {
            if (Instance != null) return;
            InventoryDB goDB = FindObjectOfType<InventoryDB>();
            if (goDB == null)
            {
                GameObject go = new GameObject("Inventory DB");
                go.AddComponent<InventoryDB>().loadDatabaseAutomatically = true;
            }
        }

        private void UpdateDatabase()
        {
            // Get Categories
            categories = new List<Category>();
            Category[] foundCats = Resources.FindObjectsOfTypeAll(typeof(Category)) as Category[];
            foreach (Category cat in foundCats)
            {
                if (!categories.Contains(cat))
                {
                    categories.Add(cat);
                }
            }

            // Get Crafting Categories
            craftingCategories = new List<CraftingCategory>();
            CraftingCategory[] foundCCs = Resources.FindObjectsOfTypeAll(typeof(CraftingCategory)) as CraftingCategory[];
            foreach (CraftingCategory cat in foundCCs)
            {
                if (!craftingCategories.Contains(cat))
                {
                    craftingCategories.Add(cat);
                }
            }

            // Get Items
            availableItems = new List<InventoryItem>();
            InventoryItem[] foundItems = Resources.FindObjectsOfTypeAll(typeof(InventoryItem)) as InventoryItem[];
            foreach (InventoryItem item in foundItems)
            {
                if (!availableItems.Contains(item))
                {
                    availableItems.Add(item);
                }
            }

            // Get Recipes
            recipes = new List<CraftingRecipe>();
            CraftingRecipe[] foundRecipes = Resources.FindObjectsOfTypeAll(typeof(CraftingRecipe)) as CraftingRecipe[];
            foreach (CraftingRecipe recipe in foundRecipes)
            {
                if (!recipes.Contains(recipe))
                {
                    recipes.Add(recipe);
                }
            }
        }

        #endregion

    }
}