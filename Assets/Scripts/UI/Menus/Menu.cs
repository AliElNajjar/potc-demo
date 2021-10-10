using UnityEngine;
using DG.Tweening;
using System.Collections;
using Scripts.Managers;
using Rewired.Integration.UnityUI;

public class Menu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject menuItems;

    protected RectTransform menuRect;
    protected CanvasGroup menuCanvasGroup;
    protected MenuItem currentItem;

    private string cancelActionId;

    //public Action<MenuItem> OnMenuItemClicked;

    protected virtual void Awake()
    {
        menuRect = menu.GetComponent<RectTransform>();
        menuCanvasGroup = menu.GetComponent<CanvasGroup>();

        Events.OnMenuItemClicked += UpdateMenuItem;
    }

    protected virtual void OnEnable()
    {
        currentItem = null;
    }

    private void Start()
    {
        menuCanvasGroup.DOFade(0f, 0.5f).From();
        menuRect.DOAnchorPosY(600f, 0.5f).From().OnComplete(() => ActivateCG(menuCanvasGroup));
        cancelActionId = UIManager.Instance.GetComponent<RewiredStandaloneInputModule>().cancelButton;
    }

    public void ToggleMenu(bool paused)
    {
        if (paused)
        {
            StopCoroutine(FadeAndSlideOut());
            StartCoroutine(FadeAndSlideIn());
        }

        if (!paused && menu.activeInHierarchy)
        {
            StopCoroutine(FadeAndSlideIn());
            StartCoroutine(FadeAndSlideOut());

        }

    }

    private void UpdateMenuItem(MenuItem clickedItem)
    {
        var previousItem = currentItem;
        currentItem = clickedItem;

        if (RewiredInputHandler.Instance.player.GetButtonDown(cancelActionId))
        {
            return;
        }

        if (currentItem == null)
        {
            previousItem?.StartCrawlingOut();
            return;
        }

        if (previousItem == currentItem)
        {
            currentItem.StartCrawlingOut();
            currentItem = null;
            return;
        }

        if (previousItem != null)
        {
            previousItem.StartCrawlingOut();
        }

        currentItem.StartCrawlingIn();
    }

    private void ActivateCG(CanvasGroup canvas)
    {
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    private IEnumerator FadeAndSlideOut()
    {
        MenuSlideOut();
        CleanUp(menuItems);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    private IEnumerator FadeAndSlideIn()
    {
        MenuSlideIn();
        yield return null;
    }

    private void MenuSlideIn()
    {
        menuCanvasGroup.DOFade(1f, 0.5f);
        menuRect.DOAnchorPosY(0f, 0.5f).OnComplete(() => ActivateCG(menuCanvasGroup));
    }

    private void MenuSlideOut()
    {
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
        menuCanvasGroup.DOFade(0f, 0.5f);
        menuRect.DOAnchorPosY(600f, 0.5f);
    }

    public virtual void FadeIn()
    {
        menuCanvasGroup.DOFade(1f, 0.5f);
    }

    protected void FadeOut()
    {
        menuCanvasGroup.DOFade(0f, 0.5f);
    }

    protected virtual void CleanUp(GameObject menuItems)
    {  
        for (int i = 0; i < menuItems.transform.childCount; i++)
        {
            menuItems.transform.GetChild(i).gameObject.SetActive(false);
        }

        Events.OnMenuItemClicked.Invoke(null);
    }

}
