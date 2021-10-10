using System;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class CustomTagFilter
    {

        #region Variables

        public TagFilterType filterType;
        public string tagName;
        public string tagValue;

        #endregion

        #region Public Methods

        public bool PassMatch(InventoryItem item)
        {
            switch (filterType)
            {
                case TagFilterType.DoesNotExist:
                    foreach (StringValue tag in item.customTags)
                    {
                        if (tag.Name == tagName)
                        {
                            return false;
                        }
                    }
                    return true;
                case TagFilterType.Exists:
                    foreach (StringValue tag in item.customTags)
                    {
                        if (tag.Name == tagName)
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    foreach (StringValue tag in item.customTags)
                    {
                        if (tag.Name == tagName)
                        {
                            return tag.Value == tagValue;
                        }
                    }
                    return false;
            }
        }

        #endregion

    }
}
