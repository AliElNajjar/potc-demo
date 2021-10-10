using Scripts.UI;
using UnityEngine;

public class Openable : Interactable
{
    private SpriteRenderer spriteRenderer;
    private Sprite closedChest;

    bool isOpen;

    [SerializeField] Sprite openChest;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        closedChest = spriteRenderer.sprite;
    }

    protected override void Interact()
    {
        MessageManager.Instance.OnMessageEnd.Invoke();
        OpenCloseChest();
    }

    public void OpenCloseChest()
    {
        if (isOpen)
        {
            spriteRenderer.sprite = closedChest;
            isOpen = false;
            return;
        }
        
        spriteRenderer.sprite = openChest;
        isOpen = true;
    }
}
