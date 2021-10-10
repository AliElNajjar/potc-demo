using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void OnEnable()
    {
        canvasGroup.DOFade(0f, 0.5f).From();
        rectTransform.DOAnchorPosX(-660f, 0.5f).From();
    }

    public void StartCrawlingIn()
    {
        gameObject.SetActive(true);
        StopCoroutine(CrawlOut());
        StartCoroutine(CrawlIn());
    }

    public virtual void StartCrawlingOut()
    {
        StopCoroutine(CrawlIn());
        if(gameObject.activeInHierarchy) StartCoroutine(CrawlOut());
    }

    protected virtual IEnumerator CrawlIn()
    {
        canvasGroup.DOFade(1f, 0.5f);
        rectTransform.DOAnchorPosX(660f, 0.5f);
        yield return new WaitUntil(() => Mathf.Approximately(rectTransform.anchoredPosition.x, 660f));
        ActivateCanvasGroup();
    }

    protected virtual IEnumerator CrawlOut()
    {
        DeactivateCanvasGroup();
        canvasGroup.DOFade(0f, 0.5f);
        rectTransform.DOAnchorPosX(-660f, 0.5f);
        yield return new WaitUntil(() => Mathf.Approximately(rectTransform.anchoredPosition.x, -660f));
        gameObject.SetActive(false);
    }


    protected void DeactivateCanvasGroup()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
    protected void ActivateCanvasGroup()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
