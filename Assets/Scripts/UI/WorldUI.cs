using UnityEngine;
using DG.Tweening;
using Scripts.Managers;
using System.Collections;
using UnityEngine.UI;

public class WorldUI : MonoBehaviour
{
    [SerializeField] Text location, objective, terrain, roundCount;
    private RectTransform rect;
    private bool isUIChanging;

    private void Awake()
    {
        GameManager.Instance.OnLevelStateChanged.AddListener(HandleLevelStateChanged);
        Events.PlayerMoving += HandlePlayerStateChanged;
        rect = gameObject.GetComponent<RectTransform>();
        DataManager.Instance.OnLevelDataUpdated += UpdateLevelUI;
    }

    private void HandlePlayerStateChanged(bool isPlayerMoving)
    {
        if(isPlayerMoving && !isUIChanging)
        {
            isUIChanging = true;
            rect.DOAnchorPosY(270, 0.25f, true).OnComplete(()=>isUIChanging = false);
        }

        if(!isPlayerMoving && !isUIChanging)
        {
            isUIChanging = true;
            rect.DOAnchorPosY(0, 0.25f, true).OnComplete(() => isUIChanging = false);
        }
  
    }

    private void UpdateLevelUI(LevelData levelData)
    {
        location.text = levelData.Location;
        objective.text = levelData.Objective;
    }

    private void HandleLevelStateChanged(GameManager.LevelState currentLevelState)
    {
        BattleInfo(currentLevelState == GameManager.LevelState.BATTLE);
    }

    private void BattleInfo(bool active)
    {
        //terrain.gameObject.SetActive(active);
        roundCount.gameObject.SetActive(active);
        //objective.gameObject.SetActive(!active);
    }

    public void SetRoundCount(int count)
    {
        roundCount.text = count.ToString();
    }

}
