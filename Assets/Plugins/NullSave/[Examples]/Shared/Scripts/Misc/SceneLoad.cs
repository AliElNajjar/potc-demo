using UnityEngine;
using UnityEngine.SceneManagement;

namespace NullSave.TOCK
{
    public class SceneLoad : MonoBehaviour
    {

        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            SceneManager.LoadScene(sceneName);
        }

    }
}