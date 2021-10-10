using Scripts.Managers;
using System;
using UnityEngine.Events;

public class Events
{
    [Serializable] public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState> { }

    [Serializable] public class EventLevelState : UnityEvent<GameManager.LevelState> { }

    public static Action<bool> PlayerMoving;

    public static Action<MenuItem> OnMenuItemClicked;
}
