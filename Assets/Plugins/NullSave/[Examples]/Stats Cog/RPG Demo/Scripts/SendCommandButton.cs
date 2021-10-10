using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Stats
{
    public class SendCommandButton : MonoBehaviour
    {

        #region Variables

        public StatsCog statCog;
        public InputField valueInput;

        #endregion

        #region Public Methods

        public void SendCommand()
        {
            statCog.SendCommand(valueInput.text);
        }

        #endregion

    }
}