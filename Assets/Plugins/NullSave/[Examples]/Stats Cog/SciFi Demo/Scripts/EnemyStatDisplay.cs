using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Stats
{
    public class EnemyStatDisplay : MonoBehaviour
    {

        #region Variables

        public string infoStat = "EnemyInfo";
        public Image enemySprite;
        public TextMeshProUGUI enemyName;

        public Transform effectsContainer;
        public EffectMonitor effectPrefab;

        // WebGL can cause effects to stack up, this is just to prevent that
        private float lastAdd;

        #endregion

        #region Public Methods

        public void SetStatsCog(StatsCog statsCog)
        {
            ClearEffects();

            StatMonitorTMP[] monitors = GetComponentsInChildren<StatMonitorTMP>();
            for (int i = 0; i < monitors.Length; i++)
            {
                monitors[i].statsCog = statsCog;
            }

            EnemyInfoList[] infoLists = GetComponentsInChildren<EnemyInfoList>();
            for (int i = 0; i < infoLists.Length; i++)
            {
                infoLists[i].statsCog = statsCog;
            }

            foreach (StatEffect effect in statsCog.Effects)
            {
                Instantiate(effectPrefab, effectsContainer).SetEffect(effect);
            }

            StatValue info = statsCog.FindStat(infoStat);
            if (info != null)
            {
                enemySprite.sprite = info.icon;
                enemyName.text = info.displayName;
            }

            // Subscribe to new effects
            statsCog.onEffectAdded.AddListener(AddEffect);
        }

        #endregion

        #region Private Methods

        private void AddEffect(StatEffect effect)
        {
            // We're just preventing WebGL stacking issues here
            // This isn't needed for mobile, desktop or platform games
            if (Time.time - lastAdd > 0.5f)
            {
                Instantiate(effectPrefab, effectsContainer).SetEffect(effect);
                lastAdd = Time.time;
            }
        }

        private void ClearEffects()
        {
            EffectMonitor[] monitors = GetComponentsInChildren<EffectMonitor>();
            foreach (EffectMonitor monitor in monitors)
            {
                Destroy(monitor.gameObject);
            }
        }

        #endregion

    }
}