using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatUIList : MonoBehaviour
    {

        #region Properties

        public StatsCog statsCog;
        public StatUI statUIPrefab;
        public Transform container;

        private List<StatUI> loadedStats;

        #endregion

        #region Variables

        private void OnEnable()
        {
            Clear();

            foreach(StatValue stat in statsCog.Stats)
            {
                if (stat.displayInList)
                {
                    StatUI instance = Instantiate(statUIPrefab, container);
                    instance.LoadStat(statsCog, stat.name);
                    loadedStats.Add(instance);
                }
            }
        }

        #endregion

        #region Private Methods

        private void Clear()
        {
            if(loadedStats != null)
            {
                foreach(StatUI item in loadedStats)
                {
                    Destroy(item.gameObject);
                }
                loadedStats.Clear();
            }
            else
            {
                loadedStats = new List<StatUI>();
            }
        }

        #endregion

    }
}