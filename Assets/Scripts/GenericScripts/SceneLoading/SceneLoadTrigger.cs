using Scripts.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum TransitionCinematic
{
    //None, //for later when we want random events to load scenes use the "else if" below
    Walking = 0
}

public class SceneLoadTrigger : MonoBehaviour
{
    //public TransitionCinematic playerAnimation;
    [SerializeField] private string _sceneName;
    [SerializeField] private Looking enteringDirection = Looking.Up;
    public int loadPosition;

    private bool istriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isTriggerDirection(collision)) return;
            
            BattleData.comingFromBattle = false;
            
            UnitOverWorldMovement.loadPosition = loadPosition;
            Camera.main.GetComponent<CameraFollowPlayer>().target = null;
            StartCoroutine(FadeAndLoad());

            //else if (active && playerAnimation == TransitionCinematic.None) { } //for "None"
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !istriggered)
        {
            if (!isTriggerDirection(other)) return;

            BattleData.comingFromBattle = false;

            UnitOverWorldMovement.loadPosition = loadPosition;
            Camera.main.GetComponent<CameraFollowPlayer>().target = null;
            StartCoroutine(FadeAndLoad());
        }
    }

    private IEnumerator FadeAndLoad()
    {
        istriggered = true;
        
        //string previousLevel = SceneManager.GetActiveScene().name;
        
        Camera.main.GetComponent<FadeCamera>().FadeOut(0.5f);

        yield return new WaitForSeconds(0.5f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel(_sceneName);
        }
        else
        {
            SceneManager.LoadScene(_sceneName);
        }
        
        //GameManager.Instance.unLoadLevel(previousLevel);
    }

    public void LoadScene(string sceneName)
    {
        GameManager.loadingFromEvent = true;
        _sceneName = sceneName;
        StartCoroutine(FadeAndLoad());
        Debug.Log("NAME:" + _sceneName);
    }

    private bool isTriggerDirection(Collider2D collider)
    {
        return collider.GetComponent<UnitOverWorldMovement>().LookingDir == enteringDirection;
    }
}
