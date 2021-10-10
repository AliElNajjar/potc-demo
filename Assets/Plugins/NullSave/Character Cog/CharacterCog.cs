#if INVENTORY_COG
using NullSave.TOCK.Inventory;
#endif
#if STATS_COG
using NullSave.TOCK.Stats;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Character
{
    [HierarchyIcon("retro-character", false)]
    [RequireComponent(typeof(Animator))]
    public class CharacterCog : MonoBehaviour
    {

        #region Variables

        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public float minInputValue = 0.1f;
        public bool useMaxInput = true;
        public bool cameraTranslation = true;
        public List<CharacterCogPlugin> plugins;

        #endregion

        #region Properties

        public List<CharacterCogPlugin> ActivePlugins { get; private set; }

        public Animator Animator { get; private set; }

        public CharacterController CharacterController { get; private set; }

#if INVENTORY_COG
        public InventoryCog Inventory { get; private set; }
#endif

        public bool IsAttacking { get; set; }

        public bool IsBlocking { get; set; }

        public bool InAction { get; set; }

        public bool LockInput { get; set; }

        public Vector3 Movement { get; set; }

        public bool PreventButtonUpdate { get; set; }

        public bool PreventMovement { get; set; }

        public Dictionary<string, string> Properties { get; private set; }

        public Rigidbody Rigidbody { get; private set; }

#if STATS_COG
        public List<DamageDealer> DamageDealers { get; set; }

        public StatsCog Stats { get; private set; }
#endif

        public Transform ViewCamera { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Properties = new Dictionary<string, string>();
            Animator = GetComponent<Animator>();
            CharacterController = GetComponent<CharacterController>();
#if INVENTORY_COG
            Inventory = GetComponent<InventoryCog>();
#endif
            Rigidbody = GetComponent<Rigidbody>();
#if STATS_COG
            Stats = GetComponent<StatsCog>();
#endif
        }

        private void Start()
        {
            ViewCamera = Camera.main.transform;

            ActivePlugins = new List<CharacterCogPlugin>();
            foreach (CharacterCogPlugin plugin in plugins)
            {
                CharacterCogPlugin instance = Instantiate(plugin);
                instance.Character = this;
                instance.Initialize();
                ActivePlugins.Add(instance);
            }

            UpdateDamageDealers();
        }

        private void FixedUpdate()
        {
#if GAME_COG
            if (GameCog.IsModalVisible)             
            {
                Animator.SetFloat("MoveSpeed", 0, 0.1f, Time.deltaTime);
                return;
            }

#endif
#if INVENTORY_COG
            if (Inventory.IsMenuOpen)
            {
                Animator.SetFloat("MoveSpeed", 0, 0.1f, Time.deltaTime);
                return;
            }
#endif

            PreventMovement = false;
            foreach (CharacterCogPlugin plugin in ActivePlugins)
            {
                plugin.PreMovement();
            }

            if (LockInput || IsAttacking || PreventMovement)
            {
                Animator.SetFloat("MoveSpeed", 0, 0.1f, Time.deltaTime);
                //return;
            }

            foreach (CharacterCogPlugin plugin in ActivePlugins)
            {
                plugin.Movement();
            }

            foreach (CharacterCogPlugin plugin in ActivePlugins)
            {
                plugin.PostMovement();
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                if (ActivePlugins == null) return;

                foreach (CharacterCogPlugin plugin in ActivePlugins)
                {
                    plugin.DrawGizmos(transform);
                }
            }
            else
            {
                if (plugins == null) return;

                foreach (CharacterCogPlugin plugin in plugins)
                {
                    plugin.DrawGizmos(transform);
                }
            }
        }

        private void Update()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif

            UpdateInput();

            PreventButtonUpdate = false;
            foreach (CharacterCogPlugin plugin in ActivePlugins)
            {
                plugin.OnUpdate();
            }

            if (PreventButtonUpdate || LockInput) return;
        }

        #endregion

        #region Public Methods

        public float GetAxis(string axisName)
        {
#if GAME_COG
            if (GameCog.Input != null)
            {
                return GameCog.Input.GetAxis(axisName);
            }
#endif

            return Input.GetAxis(axisName);
        }

        public bool GetButton(string buttonName)
        {
#if GAME_COG
            if (GameCog.Input != null)
            {
                return GameCog.Input.GetButton(buttonName);
            }
#endif

            return Input.GetButton(buttonName);
        }

        public bool GetButtonDown(string buttonName)
        {
#if GAME_COG
            if (GameCog.Input != null)
            {
                return GameCog.Input.GetButtonDown(buttonName);
            }
#endif

            return Input.GetButtonDown(buttonName);
        }

        public void SetDamageDealersActive(bool active)
        {
#if STATS_COG
            foreach (DamageDealer dealer in DamageDealers) dealer.SetColliderEnabled(active);
#endif
        }

        public void SetController(string locked)
        {
            CharacterController.enabled = locked.ToLower() == "true";
        }

        public void UpdateDamageDealers()
        {
#if STATS_COG
            if (DamageDealers == null)
            {
                DamageDealers = new List<DamageDealer>();
            }
            else
            {
                DamageDealers.Clear();
            }

            DamageDealer[] dd = GetComponentsInChildren<DamageDealer>();
            foreach (DamageDealer dealer in dd) DamageDealers.Add(dealer);
#endif
        }

        #endregion

        #region Private Methods

        private float ConvertToMax(float input)
        {
            if (input <= -minInputValue) return -1;
            if (input >= minInputValue) return 1;
            return 0;
        }

        private bool InModal()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return true;
#endif
            return false;
        }

        private void UpdateInput()
        {
            float v = GetAxis(inputVertical);
            float h = GetAxis(inputHorizontal);

            if (useMaxInput)
            {
                v = ConvertToMax(v);
                h = ConvertToMax(h);
            }
            else
            {
                if (v > -minInputValue && v < minInputValue) v = 0;
                if (h > -minInputValue && h < minInputValue) h = 0;
            }

            // Normalize input
            if (cameraTranslation && ViewCamera != null)
            {
                Movement = v * Vector3.Scale(ViewCamera.forward, new Vector3(1, 0, 1)).normalized + h * ViewCamera.right;
            }
            else
            {
                Movement = v * Vector3.forward + h * Vector3.right;
            }
        }

        #endregion

    }
}