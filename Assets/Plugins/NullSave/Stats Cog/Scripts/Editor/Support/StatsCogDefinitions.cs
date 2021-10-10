using System.Collections.Generic;

namespace NullSave.TOCK.Stats
{
    public class StatsCogDefinitions : DefinitionSymbol
    {
        public override List<string> GetSymbols
        {
            get
            {
                return new List<string>() { "STATS_COG" };
            }
        }
    }
}