using Scripts.Managers;
using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public void ActivateInteractable()
    {
        Interact();
    }

    protected virtual void Interact() { }
}
