using PixelCrushers;
using UnityEngine;
using UnityEngine.UI;

public class LoadsMenu : MonoBehaviour
{
    [SerializeField] GameObject[] slots;

    private void Awake()
    {
        UpdateSlots();
    }

    private void OnEnable()
    {
        UpdateSlots();
    }

    public void UpdateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (SaveSystem.HasSavedGameInSlot(i+1))
            {
                slots[i].GetComponent<Button>().interactable = true;
                slots[i].transform.GetChild(1).GetComponent<Text>().text = SaveManager.Instance.SaveSlotInfo[i];
                continue;
            }

            else
            {
                slots[i].GetComponent<Button>().interactable = false;
                
            }
        }
    }
}
