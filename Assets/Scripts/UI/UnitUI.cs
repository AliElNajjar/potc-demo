using Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private Image unitAvatar; 
    [SerializeField] private Text unitName; 

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(HandleUnitButtonClicked);
    }

    public void SetUnitUI(Sprite unitPortrait, string name)
    {
        unitAvatar.sprite = unitPortrait;
        unitName.text = name;
    }

    void HandleUnitButtonClicked()
    {
        UIManager.Instance.HandleUnitClicked();
    }
}
