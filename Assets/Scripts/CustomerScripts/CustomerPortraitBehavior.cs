using UnityEngine;

public class CustomerPortraitBehavior : MonoBehaviour
{
    private Animator _anime;
    private SpriteRenderer _spriteRenderer;
    private UnityEngine.UI.Image _image;

    [SerializeField] private CharacterAnimations _characterAnimations;

    private void Awake()
    {
        _anime = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _image = GetComponent<UnityEngine.UI.Image>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int indexOfAnimation = CustomerManager.Instance.CurrentCustomer.AnimationID;
        Debug.Log($"{CustomerManager.Instance.CurrentCustomer.CustomerName} || Index: {CustomerManager.Instance.CurrentCustomer.AnimationID}");
        _anime.runtimeAnimatorController = _characterAnimations.GetCharacterController(indexOfAnimation);
    }

    // Update is called once per frame
    void Update()
    {
        _image.sprite = _spriteRenderer.sprite;
    }
}
