using PixelCrushers.DialogueSystem;

namespace Scripts.Managers
{
    public class DialogueController : Singleton<DialogueController>
    {
        protected override void Awake()
        {
            base.Awake();
            GameManager.Instance?.OnGameStateChanged.AddListener(HandleGameStateChanged);
        }

        private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
        {
            if(currentState == GameManager.GameState.PAUSED)
            {
                PixelCrushers.DialogueSystem.DialogueManager.StopConversation();
            }
        }

        public bool GetBool(string boolName)
        {
            bool boolValue = DialogueLua.GetVariable(boolName).asBool;
            return boolValue;
        }
    }

}

