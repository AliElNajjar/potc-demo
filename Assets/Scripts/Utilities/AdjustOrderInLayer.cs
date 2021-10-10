using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AdjustOrderInLayer : MonoBehaviour
{
    [SerializeField] private float _yOffset = 0;

    private GameObject _player;
    private SpriteRenderer _playerSprite;
    private SpriteRenderer _spriteRenderer;
    private int _playerOrderInLayer;

    public IntUnityEvent postOrderSet;
    [SerializeField] int _positiveOrderOffset = 1;
    [SerializeField] int _negativeOrderOffset = 1;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerSprite = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerOrderInLayer = _playerSprite.sortingOrder;
    }

    void Update()
    {
        if (_player.transform.position.y > this.transform.position.y + _yOffset)
        {
            _spriteRenderer.sortingOrder = _playerOrderInLayer + Mathf.Max(1, _positiveOrderOffset);
        }
        else if (_player.transform.position.y < this.transform.position.y + _yOffset)
        {
            _spriteRenderer.sortingOrder = _playerOrderInLayer - Mathf.Max(1, _negativeOrderOffset);
        }
        if (postOrderSet != null)
            postOrderSet.Invoke(_spriteRenderer.sortingOrder);
    }

    [System.Serializable]
    [HideInInspector] public class IntUnityEvent : UnityEngine.Events.UnityEvent<int>
    {

    }

}