using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class EffectsList : MonoBehaviour
    {

        #region Variables

        public StatsCog statsCog;
        public GameObject listItem;
        public GameObject target;

        private List<EffectDisplay> tracked;

        #endregion

        #region Unity Methods

        public void Start()
        {
            tracked = new List<EffectDisplay>();
        }

        public void Update()
        {
            // Remnove extra items
            if (tracked.Count > statsCog.Effects.Count)
            {
                int diff = tracked.Count - statsCog.Effects.Count;
                for (int i = 0; i < diff; i++)
                {
                    if (tracked[0] != null) Destroy(tracked[0].gameObject);
                    tracked.RemoveAt(0);
                }
            }

            // Add new items
            while (tracked.Count < statsCog.Effects.Count)
            {
                GameObject newItem = Instantiate(listItem);
                newItem.transform.SetParent(target.transform, false);
                tracked.Add(newItem.GetComponent<EffectDisplay>());
            }

            // Update items
            for (int i = 0; i < tracked.Count; i++)
            {
                tracked[i].targetEffect = statsCog.Effects[i];
            }
        }

        #endregion

    }
}