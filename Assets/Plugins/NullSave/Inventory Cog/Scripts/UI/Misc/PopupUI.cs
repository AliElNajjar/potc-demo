using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class PopupUI 
    {

        #region Variables

        public PickupDetection detection;
        public PickupType selection;
        public string selectButton;
        public KeyCode selectKey;
        public MenuOpenType openType;
        public Transform container;
        public string spawnTag;
        public Vector3 raycastOffset = Vector3.zero;
        public LayerMask raycastCulling = 1;
        public float maxDistance = 1.5f;

        public DisplayNameRequest onPromptRequest;

        public bool obsoleteFixed;

        #endregion

        #region Properties

        public GameObject ActiveInteract { get; private set; }

        public GameObject ActivePopup { get; private set; }

        public List<object> ActiveTargets { get; set; }

        public GameObject InteractInterface { get; set; }

        public GameObject TargetPrompt { get; set; }

        #endregion

        #region Public Methods

        public bool CheckInput()
        {
            if (ActiveTargets == null || ActiveTargets.Count == 0 || ActivePopup == null) return false;

#if GAME_COG
            if ((selection == PickupType.ByButton && GameCog.Input.GetButtonDown(selectButton)) ||
                (selection == PickupType.ByKey && GameCog.Input.GetKeyDown(selectKey)))
#else
            if ((selection == PickupType.ByButton && Input.GetButtonDown(selectButton)) ||
                (selection == PickupType.ByKey && Input.GetKeyDown(selectKey)))
#endif
            {
                return true;
            }

            return false;
        }

        public void HideInteract()
        {
            switch (openType)
            {
                case MenuOpenType.ActiveGameObject:
                    ActiveInteract.SetActive(false);
                    break;
                default:
                    GameObject.Destroy(ActiveInteract);
                    break;
            }

            ActiveInteract = null;
        }

        public void HidePopup()
        {
            switch (openType)
            {
                case MenuOpenType.ActiveGameObject:
                    TargetPrompt.SetActive(false);
                    break;
                default:
                    GameObject.Destroy(ActivePopup);
                    break;
            }

            ActivePopup = null;
        }

        public void Revalidate()
        {
            if (ActiveTargets.Count == 0)
            {
                HidePopup();
            }
            else
            {
                onPromptRequest?.Invoke(this, ActiveTargets[ActiveTargets.Count - 1]);
            }
        }

        public void ShowInteract()
        {
            if (InteractInterface == null || ActiveInteract != null) return;

            switch (openType)
            {
                case MenuOpenType.ActiveGameObject:
                    ActiveInteract = InteractInterface;
                    InteractInterface.gameObject.SetActive(true);
                    break;
                case MenuOpenType.SpawnInTransform:
                    ActiveInteract = GameObject.Instantiate(InteractInterface, container);
                    break;
                case MenuOpenType.SpawnOnFirstCanvas:
                    Canvas canvas = GameObject.FindObjectOfType<Canvas>();
                    ActiveInteract = GameObject.Instantiate(InteractInterface, canvas.transform);
                    break;
                case MenuOpenType.SpawnInTag:
                    ActiveInteract = GameObject.Instantiate(InteractInterface, GameObject.FindGameObjectWithTag(spawnTag).transform);
                    break;
            }
        }

        public void ShowPopup()
        {
            if (ActivePopup != null) return;

            switch (openType)
            {
                case MenuOpenType.ActiveGameObject:
                    ActivePopup = TargetPrompt;
                    TargetPrompt.SetActive(true);
                    break;
                case MenuOpenType.SpawnInTransform:
                    ActivePopup = GameObject.Instantiate(TargetPrompt, container);
                    break;
                case MenuOpenType.SpawnOnFirstCanvas:
                    Canvas canvas = GameObject.FindObjectOfType<Canvas>();
                    ActivePopup = GameObject.Instantiate(TargetPrompt, canvas.transform);
                    break;
                case MenuOpenType.SpawnInTag:
                    ActivePopup = GameObject.Instantiate(TargetPrompt, GameObject.FindGameObjectWithTag(spawnTag).transform);
                    break;
            }
        }

        public void TriggerEnter(GameObject other, System.Type findType)
        {
            if (ActiveTargets == null) ActiveTargets = new List<object>();

            var target = other.GetComponent(findType);
            if (target != null)
            {
                ActiveTargets.Add(target);
                onPromptRequest?.Invoke(this, target);
            }
        }

        public void TriggerExit(GameObject other, System.Type findType)
        {
            if (ActiveTargets == null) ActiveTargets = new List<object>();

            if (ActiveTargets.Count > 0)
            {
                var otherTarget = other.GetComponent(findType);
                if (otherTarget == null) return;

                foreach (object target in ActiveTargets)
                {
                    if (target.Equals(otherTarget))
                    {
                        ActiveTargets.Remove(target);
                        break;
                    }
                }

                Revalidate();
            }
        }

        public void UpdateRaycast(System.Type findType, Transform playerTransform)
        {
            if (detection != PickupDetection.MainCamRaycast) return;

            if (ActiveTargets == null) ActiveTargets = new List<object>();

            // Raycast
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, maxDistance + 20, raycastCulling))
            {
                var target = hit.transform.gameObject.GetComponentInChildren(findType);
                if (target == null)
                {
                    if (ActiveTargets.Count != 0)
                    {
                        ActiveTargets.Clear();
                        HidePopup();
                    }
                    return;
                }

                float distance = Vector3.Distance(playerTransform.position, hit.transform.position);
                if (distance > maxDistance)
                {
                    if (ActiveTargets.Count != 0)
                    {
                        ActiveTargets.Clear();
                        HidePopup();
                    }
                    return;
                }

                if (!ActiveTargets.Contains(target))
                {
                    ActiveTargets.Add(target);
                    onPromptRequest?.Invoke(this, target);
                }
            }
            else
            {
                if (ActiveTargets.Count != 0)
                {
                    ActiveTargets.Clear();
                    HidePopup();
                }
            }
        }

        #endregion

    }
}