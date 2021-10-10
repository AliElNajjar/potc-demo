using Scripts.Managers;
using UnityEngine;

public class BaseMenu : Menu
{
    [Header("Sub-Menus")]
    [SerializeField] private ManageUnits manageUnits;
    [SerializeField] private Ledger ledger;
    [SerializeField] private Configuration configuration;
    [SerializeField] private SavesMenu savesMenu;
    [SerializeField] private UnitsList unitsList;
    [SerializeField] private UnitLoadOut unitLoadOut;
    [SerializeField] private ManageUnitsClone manageUnitsClone;


    public void ManageUnitsBtnClicked()
    {
        Events.OnMenuItemClicked?.Invoke(manageUnits);
    }

    public void LedgerBtnClicked()
    {
        Events.OnMenuItemClicked?.Invoke(ledger);
    }

    public void SkipToBattleBtnClicked()
    {
        Debug.Log("Skipped to Battle!");
    }

    public void ConfigurationBtnClicked()
    {
        Events.OnMenuItemClicked?.Invoke(configuration);
    }

    public void SaveBtnClicked()
    {
        Events.OnMenuItemClicked?.Invoke(savesMenu);
    }

    public void QuitToMenuBtnClicked()
    {
        GameManager.Instance.QuitToMenu();
    }

    public void BarracksClicked()
    {
        FadeOut();
        manageUnitsClone.StartCrawlingIn();
        Events.OnMenuItemClicked?.Invoke(unitsList);
        //ManageUnitsClone.clickedItem = unitsList;
    }

    public void StallsClicked()
    {
        //ManageUnitsClone.clickedItem = stalls;
    }

    public void ChaptersClicked()
    {

    }

    public void SynopsisClicked()
    {

    }

    public void UnitClicked() //turns on units' Loadout screen.
    {
        unitLoadOut.FadeIn();
    }

    protected override void CleanUp(GameObject menuItems)
    {
        base.CleanUp(menuItems);
        unitLoadOut.FadeOut();
    }
}
