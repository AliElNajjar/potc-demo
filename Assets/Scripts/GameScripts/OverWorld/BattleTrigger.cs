using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField] private string battleArea;

    public int loadPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Battle Triggered!");
            StartCoroutine(PrepareForBattle());
        }
    }

    public IEnumerator PrepareForBattle()
    {
        yield return new WaitForSeconds(.5f);
        StartBattle();
    }

    public void StartBattle()
    {
        // Assign battle area and environment to battle data.
        //BattleData.Instance.backgroundAreaToLoad = Area;

        BattleData.Instance.InitiateBattle();
    }
}