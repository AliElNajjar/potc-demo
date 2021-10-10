using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLoadOut : MonoBehaviour
{
    [Header("Unit Loadout Screens")]
    [SerializeField] private GameObject[] screens;

    private CanvasGroup canvasGroup;
    private Dictionary<int, GameObject> _screens = new Dictionary<int, GameObject>();
    private int index = 0;
    
    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        for (int i = 0; i < screens.Length; i++)
        {
            _screens.Add(i, screens[i]);
        }
    }

    private void OnEnable()
    {
        ShowScreen(0);
    }

    private void Start()
    {
        canvasGroup.DOFade(0f, 0.8f).From().OnComplete(ActivateCG);
    }

    public void BackButton()
    {
        FadeOut();
    }

    public void NextScreen()
    {
        HideScreen(index);

        index++;
        if (index > _screens.Count - 1)
        {
            index = 0;
        }

        ShowScreen(index);
    }

    public void PreviousScreen()
    {
        HideScreen(index);
        
        index--;
        if (index < 0)
        {
            index = _screens.Count - 1;
        }

        ShowScreen(index);
    }

    private void ShowScreen(int _index)
    {
        _screens[_index].SetActive(true);
    }

    private void HideScreen(int _index)
    {
        _screens[_index].SetActive(false);
    }

    private void HideAllScreens()
    {
        for (int i = 0; i < screens.Length; i++)
        {
            HideScreen(i);
        }
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        StopCoroutine(FadingOut());
        StartCoroutine(FadingIn());
    }

    public void FadeOut()
    {
        if (gameObject.activeInHierarchy)
        {
            StopCoroutine(FadingIn());
            StartCoroutine(FadingOut());
        }
    }

    private IEnumerator FadingIn()
    {
        canvasGroup.DOFade(1f, 0.8f).OnComplete(ActivateCG);
        yield return null;
    }

    private IEnumerator FadingOut()
    {
        DeactivateCG();
        canvasGroup.DOFade(0f, 0.8f).OnComplete(() => gameObject.SetActive(false));
        yield return null;
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

    private void OnDisable()
    {
        HideAllScreens();
        //_screens.Clear();
        //UIManager.Instance.CleanUpMenuItems(gameObject); 
    }
}







