using UnityEngine;
using Scripts.UI;
using System.Collections;

namespace Scripts.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private GameObject worldUI;

        [Header("Menus")]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject baseMenu;
        [SerializeField] private GameObject battleMenu;
        [SerializeField] private GameObject miscUI;

        //[SerializeField] private UnitsList UnitsList;

        private GameObject menuToDisplay;
        private BaseMenu _baseMenu;
        private BattleMenu _battleMenu;
        private MainMenu _mainMenu;

        protected override void Awake()
        {
            base.Awake();

            _baseMenu = baseMenu.GetComponent<BaseMenu>();
            _battleMenu = battleMenu.GetComponent<BattleMenu>();
            _mainMenu = mainMenu.GetComponent<MainMenu>();

            GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
            //GameManager.Instance.OnLevelStateChanged.AddListener(HandleLeveStateChanged); TO DO: Battle UI
        }

        private void Start()
        {
            mainMenu.SetActive(!GameManager.Instance.testing);
            worldUI.SetActive(true);

            menuToDisplay = baseMenu; //on load game if saved during battle, should switch this to battle in load method
            //unit adding/removing logic then call UpDateUnitList();
        }

        private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
        {
            ToggleMenu(currentState == GameManager.GameState.PAUSED);
            ToggleMainMenu(currentState == GameManager.GameState.PREGAME);
        }

        private void HandleLeveStateChanged(GameManager.LevelState currentLevelState)
        {
            menuToDisplay = currentLevelState == GameManager.LevelState.BASE ? baseMenu : battleMenu;
        }

        public void HandleUnitClicked() //turns on units' Loadout screen.
        {
            _baseMenu.UnitClicked();
        }

        private void ToggleMenu(bool paused)
        {
            if (paused)
            {
                menuToDisplay.SetActive(paused);
            }

            if (menuToDisplay == baseMenu)
            {
                _baseMenu.ToggleMenu(paused);
                return;
            }

            if (menuToDisplay == battleMenu)
            {
                _battleMenu.ToggleMenu(paused);
                return;
            }
        }

        private void ToggleMainMenu(bool pregame)
        {
            if (pregame)
            {
                mainMenu.SetActive(pregame);
            }

            _mainMenu.ToggleMenu(pregame);
        }

        public void LoadGame(int slot)
        {
            SaveManager.Instance.LoadGame(slot);
        }

        public void FlashBattleUI()
        {
            miscUI.SetActive(true);

            StartCoroutine(FlashRoutine(miscUI, 3));
        }

        public void UpDateUnitList() // as UI not data
        {
            
        }

        private IEnumerator FlashRoutine(GameObject UIToFlash, int timesToFlash)
        {
            var timesFlashed = 0;
            while (timesToFlash > timesFlashed)
            {
                UIToFlash.SetActive(false);
                yield return new WaitForSeconds(0.5f);
                UIToFlash.SetActive(true);
                yield return new WaitForSeconds(0.5f);

                timesFlashed++;
            }

            UIToFlash.SetActive(false);
        }
    }
}

