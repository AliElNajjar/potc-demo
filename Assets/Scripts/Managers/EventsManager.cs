using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : Singleton<EventsManager>
{
    private UIManager UIManager;
    
    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
    }

    public void FlashBattleUI()
    {
        UIManager.FlashBattleUI();
    }
}
