using PixelCrushers;
using UnityEngine;
using UnityEngine.UI;

public class SavesMenu : MenuItem
{
    [SerializeField] private GameObject confirmationPopup;
    [SerializeField] private GameObject savedGamePopup;
    [SerializeField] private GameObject[] slots;
    [SerializeField] private Color savedSlotColor;
    [SerializeField] private Color emptySlotColor;

    private int forcedSlot;

    public GameObject[] Slots => slots;

    protected override void Awake()
    {
        base.Awake();

        UpdateSlots();
    }

    protected override void OnEnable()
    {
        UpdateSlots();
    }

    public void SaveToSlot(int slotNumber)
    {
        if (!SaveSystem.HasSavedGameInSlot(slotNumber))
        {
            SaveSlot(slotNumber);
            savedGamePopup.SetActive(true);
            return;
        }

        forcedSlot = slotNumber;
        ConfirmOverWrite();
    }

    private void ConfirmOverWrite()
    {
        confirmationPopup.SetActive(true); //or fadeIn.
    }

    //if yes to confirm
    public void ForceSaveToSlot()
    {
        SaveSlot(forcedSlot);
        savedGamePopup.SetActive(true);
        //update saveSlot UI info.
    }

    private void SaveSlot(int slotNumber)
    {
        var slot = slots[slotNumber -1];
        slot.GetComponent<Image>().color = savedSlotColor;
        slot.transform.GetChild(1).GetComponent<Text>().text = SaveManager.Instance.GetSaveTime();
        SaveManager.Instance.SaveGame(slotNumber);
    }

    public void UpdateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (SaveSystem.HasSavedGameInSlot(i + 1))
            {
                slots[i].GetComponent<Image>().color = savedSlotColor;
                slots[i].transform.GetChild(1).GetComponent<Text>().text = SaveManager.Instance.SaveSlotInfo[i];
                continue;
            }
            else
            {
                slots[i].GetComponent<Image>().color = emptySlotColor;
                slots[i].transform.GetChild(1).GetComponent<Text>().text = "Empty Slot";
            }
        }
    }

    private void OnDisable()
    {
        savedGamePopup.SetActive(false);
    }
}
