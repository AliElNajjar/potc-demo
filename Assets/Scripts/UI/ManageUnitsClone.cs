using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ManageUnitsClone : MenuItem
{
    [SerializeField] private BaseMenu menu;
    [SerializeField] private UnitsList unitsList;

    //public static MenuItem clickedItem;
    [HideInInspector] public MenuItem clickedItem;

    private bool isActive;

    protected override void OnEnable()
    {
        base.OnEnable();
        clickedItem = unitsList;
        isActive = true;
    }
    
    public void CloneBarracksClicked()
    {
        Events.OnMenuItemClicked.Invoke(clickedItem);
        isActive = true ? false : true;
        //clickedItem = unitsList? null : unitsList;
    }

    public void CloneStallsClicked()
    {
        //menu.OnMenuItemClicked.Invoke(stalls);
        //clickedItem = stalls ? null : stalls;
    }

    public void BackToMenuClicked()
    {
        if (isActive)
        {
            Events.OnMenuItemClicked.Invoke(clickedItem);
        }
        else
        {
            Events.OnMenuItemClicked.Invoke(null);
        }
        StartCrawlingOut();
        menu.FadeIn();
    }

    protected override IEnumerator CrawlIn()
    {
        canvasGroup.DOFade(1f, 0.5f);
        rectTransform.DOAnchorPosX(0f, 0.5f);
        yield return new WaitUntil(() => Mathf.Approximately(rectTransform.anchoredPosition.x, 0f));
        ActivateCanvasGroup();
    }

    protected override IEnumerator CrawlOut()
    {
        DeactivateCanvasGroup();
        canvasGroup.DOFade(0f, 0.5f);
        rectTransform.DOAnchorPosX(-660f, 0.5f);
        yield return new WaitUntil(() => Mathf.Approximately(rectTransform.anchoredPosition.x, -660f));
        gameObject.SetActive(false);
    }
}
