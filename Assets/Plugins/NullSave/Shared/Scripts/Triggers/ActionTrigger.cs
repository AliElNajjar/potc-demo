using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NullSave.TOCK
{
    public class ActionTrigger : MonoBehaviour
    {

        #region Variables

        public KeyCode actionKey = KeyCode.F;
        public float fillTime = 1.5f;
        public RectTransform container;
        public bool hideAfterUse = true;

        public Image fillLeft, fillCenter, fillRight;

        public ActionMenu actionMenu;
        public MenuOpenType actionOpen = MenuOpenType.ActiveGameObject;
        public Transform menuContainer;
        public string spawnTag;

        public UnityEvent onActionTriggered;

        private float fillElapsed;
        private bool inTrigger;
        private bool waitForKeyUp;
        private ActionMenu spawnedMenu;

        #endregion

        #region Properties

#if STATS_COG
        public Stats.StatsCog StatsCog { get; internal set; }
#endif

#if INVENTORY_COG
        public Inventory.InventoryCog InventoryCog { get; internal set; }
#endif

        /// <summary>
        /// Get/Set menu open flag
        /// </summary>
        public bool IsMenuOpen { get; set; }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (!inTrigger) return;

            float newValue = 0;
#if GAME_COG
            if (waitForKeyUp)
            {
                waitForKeyUp = GameCog.Input.GetKey(actionKey);
            }
            else
            {
                if (GameCog.Input.GetKey(actionKey))
                {
                    newValue = Mathf.Clamp(fillElapsed + Time.deltaTime, 0, fillTime);
                }
                else
                {
                    newValue = Mathf.Clamp(fillElapsed - Time.deltaTime, 0, fillTime);
                }
            }
#else
            if (waitForKeyUp)
            {
                waitForKeyUp = Input.GetKey(actionKey);
            }
            else
            {
                if (Input.GetKey(actionKey))
                {
                    newValue = Mathf.Clamp(fillElapsed + Time.deltaTime, 0, fillTime);
                }
                else
                {
                    newValue = Mathf.Clamp(fillElapsed - Time.deltaTime, 0, fillTime);
                }
            }
#endif

            if (newValue != fillElapsed)
            {
                fillElapsed = newValue;
                UpdateUI();
            }

            if (fillElapsed == fillTime)
            {
                onActionTriggered?.Invoke();
                fillElapsed = 0;
                UpdateUI();
                if (hideAfterUse)
                {
                    container.gameObject.SetActive(false);
                    inTrigger = false;
                }
                else
                {
                    waitForKeyUp = true;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;

            if (other.CompareTag("Player"))
            {
#if STATS_COG
                StatsCog = other.gameObject.GetComponentInChildren<Stats.StatsCog>();
#endif
#if INVENTORY_COG
                InventoryCog = other.gameObject.GetComponentInChildren<Inventory.InventoryCog>();
#endif
                inTrigger = true;
                container.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled) return;

            if (other.CompareTag("Player"))
            {
                fillElapsed = 0;
                UpdateUI();
                inTrigger = false;
                container.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Public Methods

        public void AddEffect(string effectName)
        {
#if STATS_COG
            StatsCog.AddEffect(effectName);
#endif
        }

        public void CloseMenu()
        {
            if (!IsMenuOpen) return;

            switch (actionOpen)
            {
                case MenuOpenType.ActiveGameObject:
                    actionMenu.gameObject.SetActive(false);
                    break;
                default:
                    Destroy(spawnedMenu.gameObject);
                    break;
            }

            IsMenuOpen = false;
        }

        public void OpenMenu()
        {
            if (actionMenu == null) return;

            switch (actionOpen)
            {
                case MenuOpenType.ActiveGameObject:
                    spawnedMenu = actionMenu;
                    actionMenu.gameObject.SetActive(true);
                    break;
                case MenuOpenType.SpawnInTransform:
                    spawnedMenu = Instantiate(actionMenu, menuContainer);
                    break;
                case MenuOpenType.SpawnOnFirstCanvas:
                    Canvas canvas = FindObjectOfType<Canvas>();
                    spawnedMenu = Instantiate(actionMenu, canvas.transform);
                    break;
                case MenuOpenType.SpawnInTag:
                    spawnedMenu = Instantiate(actionMenu, GameObject.FindGameObjectWithTag(spawnTag).transform);
                    break;
            }

            spawnedMenu.Owner = this;

#if INVENTORY_COG
            spawnedMenu.InventoryCog = InventoryCog;
#endif

#if STATS_COG
            spawnedMenu.StatsCog = StatsCog;
#endif

            spawnedMenu.Initialize();

            IsMenuOpen = true;

        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            float totalPercent = fillElapsed / fillTime;
            float fillPixels = container.rect.width * totalPercent;
            float prevFill = 0;

            float targetWidth = fillLeft.rectTransform.rect.width;
            fillLeft.fillAmount = Mathf.Clamp(fillPixels / targetWidth, 0, 1);

            if (fillLeft.fillAmount == 1)
            {
                prevFill = targetWidth;
                targetWidth = fillCenter.rectTransform.rect.width;
                fillCenter.fillAmount = Mathf.Clamp((fillPixels - prevFill) / targetWidth, 0, 1);

                if (fillCenter.fillAmount == 1)
                {
                    prevFill += targetWidth;
                    targetWidth = fillRight.rectTransform.rect.width;
                    fillRight.fillAmount = Mathf.Clamp((fillPixels - prevFill) / targetWidth, 0, 1);
                }
                else
                {
                    fillRight.fillAmount = 0;
                }
            }
            else
            {
                fillCenter.fillAmount = 0;
                fillRight.fillAmount = 0;
            }
        }

        #endregion

    }
}