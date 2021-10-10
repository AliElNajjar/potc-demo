using UnityEngine.SceneManagement;
using UnityEngine;
using Scripts.Managers;

public class BattleData : Singleton<BattleData>
{
    public Area backgroundAreaToLoad;
    public static bool comingFromBattle;

    public void InitiateBattle()
    {
        //battle state
        GameManager.Instance.EnterBattle();
    }
}

public enum Area
{
    Demo1 = 0,
    Demo2 = 1
};