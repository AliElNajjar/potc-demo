using Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleArena : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && GameManager.Instance.CurrentLevelState == GameManager.LevelState.BATTLE)
        {
            GameManager.Instance.ExitBattle();
        }
    }
}
