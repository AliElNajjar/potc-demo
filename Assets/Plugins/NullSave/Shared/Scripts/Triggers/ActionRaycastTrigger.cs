using TMPro;
using UnityEngine;

namespace NullSave.TOCK
{
    public class ActionRaycastTrigger : ActionTrigger
    {

        #region Variables

        public GameObject actionUI;
        public bool setUIText;
        public TextMeshProUGUI uiText;
        public string textFormat = "{0}";

        public Vector3 raycastOffset = Vector3.zero;
        public LayerMask raycastCulling = 1;
        public float maxDistance = 1.5f;

        public NavigationType actionType = NavigationType.ByButton;
        public string actionButton = "Submit";

        private ActionRaycastTarget target;

        #endregion

        #region Unity Methods

        private void Awake()
        {
#if STATS_COG
            StatsCog = GetComponentInChildren<Stats.StatsCog>();
#endif
#if INVENTORY_COG
            InventoryCog = GetComponentInChildren<Inventory.InventoryCog>();
#endif

        }

        private void Update()
        {
            if (IsMenuOpen)
            {
                actionUI.SetActive(false);
                return;
            }

            UpdateRaycast();
            if (target != null)
            {
                if ((actionType == NavigationType.ByButton && Input.GetButton(actionButton)) ||
                        (actionType == NavigationType.ByKey && Input.GetKey(actionKey)))
                {
                    target.ActivateTrigger();
                }
            }
        }

        #endregion

        #region Private Methods

        private void UpdateRaycast()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, maxDistance + 20, raycastCulling))
            {
                target = hit.transform.gameObject.GetComponentInChildren<ActionRaycastTarget>();
                if (target == null || !target.interactable)
                {
                    actionUI.SetActive(false);
                    return;
                }

                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance > maxDistance)
                {
                    actionUI.SetActive(false);
                    return;
                }

                target.Caster = this;

                if (setUIText && uiText != null)
                {
                    uiText.text = textFormat.Replace("{0}", target.displayText);
                }

                if (actionUI != null)
                {
                    actionUI.SetActive(true);
                }
            }
            else
            {
                actionUI.SetActive(false);
            }
        }

        #endregion

    }
}