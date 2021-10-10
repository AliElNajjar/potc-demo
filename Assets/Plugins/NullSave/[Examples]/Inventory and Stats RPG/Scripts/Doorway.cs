//using Cinemachine;
//using UnityEngine;

//[RequireComponent(typeof(Collider))]
//public class Doorway : MonoBehaviour
//{

//    #region Variables

//    public GameObject blackout;
//    public bool visibleAfterTrigger;
//    public CinemachineVirtualCamera newCamera;

//    #endregion

//    #region Unity Methods

//    private void OnTriggerEnter(Collider other)
//    {
//        blackout.SetActive(visibleAfterTrigger);
//        if (!visibleAfterTrigger)
//        {
//            Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.Priority = 1;
//            newCamera.Priority = 10;
//        }
//    }

//    #endregion

//}
