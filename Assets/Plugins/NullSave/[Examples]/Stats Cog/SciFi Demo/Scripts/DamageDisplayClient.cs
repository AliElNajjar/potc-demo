using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(StatsCog))]
    public class DamageDisplayClient : MonoBehaviour
    {

        #region Variables

        public DamageDisplay displayPrefab;
        public Transform displayParent;
        public float secBetweenHits = 1;

        public Color damageColor = Color.red;
        public Color immuneColor = Color.blue;
        public Color resistColor = Color.green;

        float lastDisplay;
        float dmgSinceLast;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            GetComponent<StatsCog>().onDamageTaken.AddListener(DamageTaken);
            GetComponent<StatsCog>().onImmuneToDamage.AddListener(DamageImmune);
            GetComponent<StatsCog>().onEffectResisted.AddListener(EffectResist);
        }

        private void Update()
        {
            if (dmgSinceLast > 0 && Time.time - lastDisplay >= secBetweenHits)
            {
                DisplayDamage();
            }
        }

        #endregion

        #region Private Methods

        private void DamageTaken(float amount, DamageDealer dealer, GameObject damageSource)
        {
            if (amount == 0) return;

            dmgSinceLast += amount;
            if (Time.time - lastDisplay >= secBetweenHits)
            {
                DisplayDamage();
            }
        }

        private void DamageImmune()
        {
            DamageDisplay display = Instantiate(displayPrefab, displayParent);
            display.displayText.color = immuneColor;
            display.displayText.text = "IMMUNE";
            display.transform.position = display.transform.position - new Vector3(0, -0.1f, 0);
        }

        private void DisplayDamage()
        {
            DamageDisplay display = Instantiate(displayPrefab, displayParent);
            display.displayText.color = damageColor;
            display.displayText.text = "-" + Mathf.RoundToInt(dmgSinceLast);
            dmgSinceLast = 0;
            lastDisplay = Time.time;
        }

        private void EffectResist(StatEffect effect)
        {
            DamageDisplay display = Instantiate(displayPrefab, displayParent);
            display.displayText.color = resistColor;
            display.displayText.text = "RESIST";
            display.transform.position = display.transform.position - new Vector3(0, -0.1f, 0);
        }

        #endregion


    }
}
