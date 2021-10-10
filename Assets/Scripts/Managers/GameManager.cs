using PixelCrushers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Scripts.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public Action OnLevelLoaded;

        public bool testing = true;
        
        [HideInInspector] public Events.EventGameState OnGameStateChanged;
        [HideInInspector] public Events.EventLevelState OnLevelStateChanged;

        public static bool startingNewGame, loadingFromEvent;

        #region Private Fields

        private string currentLevelName = string.Empty, previousLevel;

        private GameState _currentGameState = GameState.PREGAME;
        private LevelState _currentLevelState = LevelState.BASE;
        
        private List<GameObject> instancedSystemPrefabs = new List<GameObject>();
        private List<AsyncOperation> loadOperations = new List<AsyncOperation>();

        #endregion

        #region Public Fields

        public GameObject[] SystemPrefabs;

        #endregion

        #region Public Properties

        public string CurrentLevel => currentLevelName;
        public GameState CurrentGameState
        {
            get { return _currentGameState; }
            private set { _currentGameState = value; }
        }

        public LevelState CurrentLevelState
        {
            get { return _currentLevelState; }
            private set { _currentLevelState = value; }
        }

        #endregion

        #region Enums

        public enum GameState
        {
            PREGAME,
            RUNNING,
            PAUSED
        }

        public enum LevelState
        {
            BASE,
            BATTLE
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
            
        }

        private void Start()
        {
            InstantiateSystemPrefabs();
        }

        private void Update()
        {

            if (_currentGameState == GameState.PREGAME)
            {
                return;
            }

            if (RewiredInputHandler.Instance.player.GetButtonDown("Pause"))
            {
                TogglePause();
            }
        }

        private void UpdateState(GameState state)
        {
            var previousGameState = _currentGameState;
            _currentGameState = state;

            switch (_currentGameState)
            {
                case GameState.PREGAME:
                    break;
                case GameState.RUNNING:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.PAUSED:
                    Time.timeScale = 0f;
                    break;
                default:
                    break;
            }

            OnGameStateChanged.Invoke(_currentGameState, previousGameState);
        }

        private void UpdateLevelState(LevelState state)
        {
            _currentLevelState = state;

            switch (_currentLevelState)
            {
                case LevelState.BASE:
                    //logic for Base state
                    break;
                case LevelState.BATTLE:
                    //logic for Battle state
                    break;
                default:
                    break;
            }

            OnLevelStateChanged.Invoke(_currentLevelState);
        }

        //listens to when level loading is completed
        private void OnLoadOperationComplete(AsyncOperation ao)
        {
            if (loadOperations.Contains(ao))
            {
                loadOperations.Remove(ao);

                if (loadOperations.Count == 0)
                {
                    UpdateState(GameState.RUNNING);
                    OnLevelLoaded.Invoke();
                }
            }

            // Debug.Log("load Complete");
        }

        private void OnUnloadOperationComplete(AsyncOperation ao)
        {
            Debug.Log("Unload Complete");
        }

        private void InstantiateSystemPrefabs()
        {
            GameObject prefabInstance;
            for (int i = 0; i < SystemPrefabs.Length; i++)
            {
                prefabInstance = Instantiate(SystemPrefabs[i], transform);
                instancedSystemPrefabs.Add(prefabInstance);
            }
        }

        public void LoadLevel(string levelName)
        {
            previousLevel = currentLevelName;
            currentLevelName = levelName;
            var ao = SceneManager.LoadSceneAsync(levelName);

            if (ao == null)
            {
                Debug.LogError("Unable to load level" + levelName);
                return;
            }

            ao.completed += OnLoadOperationComplete;
            loadOperations.Add(ao);
            if (CurrentLevel == "FinalBattleScene") // todo: check levelData to see if batttle or base
            {
                UpdateLevelState(LevelState.BATTLE);
            }
            //currentLevelName = levelName;
        }

        public void UnLoadLevel(string levelName)
        {
            var ao = SceneManager.UnloadSceneAsync(levelName);

            if (ao == null)
            {
                Debug.LogError("Unable to unload level" + levelName);
                return;
            }

            ao.completed += OnUnloadOperationComplete;
        }

        public void StartGame()
        {
            if (testing) return;
            SaveSystem.ResetGameState();
            startingNewGame = true;
            LoadLevel("FinalBaseScene");
        }

        public void TogglePause()
        {
            UpdateState(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);

        }

        public void QuitToMenu()
        {
            UpdateState(GameState.PREGAME);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void RunGame()
        {
            UpdateState(GameState.RUNNING);
        }

        public void EnterBattle()
        {
            UpdateLevelState(LevelState.BATTLE);
            LoadLevel("Battle");
        }

        public void ExitBattle()
        {
            BattleData.comingFromBattle = true;
            UpdateState(GameState.RUNNING);
            UpdateLevelState(LevelState.BASE);
            LoadLevel(previousLevel);
        }

        //clean up
        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < instancedSystemPrefabs.Count; i++)
            {
                Destroy(instancedSystemPrefabs[i]);
            }
            instancedSystemPrefabs.Clear();
        }
    }
}