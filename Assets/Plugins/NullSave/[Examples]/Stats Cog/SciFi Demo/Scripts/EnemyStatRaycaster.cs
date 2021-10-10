using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class EnemyStatRaycaster : MonoBehaviour
    {

        #region Variables

        public Transform caster;
        public EnemyStatDisplay display;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (Physics.Raycast(RectTransformUtility.ScreenPointToRay(Camera.main, caster.position), out RaycastHit hit))
            {
                if (hit.transform.gameObject.CompareTag("Player")) return;

                StatsCog statsCog = hit.transform.gameObject.GetComponentInChildren<StatsCog>();
                if (statsCog == null)
                {
                    display.gameObject.SetActive(false);
                }
                else
                {
                    display.SetStatsCog(statsCog);
                    display.gameObject.SetActive(true);
                }
            }
        }

        #endregion

    }
}