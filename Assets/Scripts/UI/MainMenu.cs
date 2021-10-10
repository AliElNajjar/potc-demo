using UnityEngine;
using DG.Tweening;
using Scripts.Managers;

public class MainMenu : MonoBehaviour
{ 
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        canvasGroup.DOFade(0f, 0.8f).From().OnComplete(ActivateCG);
    }

    public void ToggleMenu(bool pregame)
    {   

        if (pregame)
        {
            canvasGroup.DOFade(1f, 0.8f).OnComplete(ActivateCG);
        }

        if (!pregame && gameObject.activeInHierarchy)
        {
            DeactivateCG();
            canvasGroup.DOFade(0f, 0.8f).OnComplete(() => gameObject.SetActive(false));
        }
    }

    void ActivateCG()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void DeactivateCG()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void HandleNewGameButtonClicked()
    {
        GameManager.Instance.StartGame();
    }

    public void HandleExitClicked()
    {
        GameManager.Instance.ExitGame();
    }
}
