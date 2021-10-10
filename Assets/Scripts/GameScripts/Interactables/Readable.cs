using Scripts.Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Readable : Interactable
{
    [TextArea]
    [SerializeField] private string content;

    public string Content => content;

    protected override void Interact() 
    {
        MessageManager.Instance.TogglePanel(true, Content);
    }
}
