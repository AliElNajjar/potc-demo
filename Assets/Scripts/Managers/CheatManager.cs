using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField] GameObject TerraPlayer;
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.J))
        {
            Debug.LogWarning("Hit");
            //TerraPlayer.GetComponent<UnitStatsCalculations>().HP -= 2;
        }
    }
}
