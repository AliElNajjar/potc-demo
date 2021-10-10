using System.Collections.Generic;

namespace NullSave.TOCK.Inventory
{
    public class InventoryCogDefinitions : DefinitionSymbol
    {
        public override List<string> GetSymbols
        {
            get
            {
                return new List<string>() { "INVENTORY_COG" };
            }
        }
    }
}