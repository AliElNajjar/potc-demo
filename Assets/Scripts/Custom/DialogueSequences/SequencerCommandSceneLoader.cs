using Scripts.Managers;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    public class SequencerCommandSceneLoader : SequencerCommand
    {
        public void Awake()
        {
            GameManager.Instance.LoadLevel(GetParameter(0));
            Stop();
        }
    }

}
