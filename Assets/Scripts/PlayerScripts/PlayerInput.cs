using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public enum Directions 
    { 
        Up, 
        Right, 
        Down,
        Left
    }

    public Directions CurrentDirection { get; private set; }

    [Header("Physics Values")]
    [SerializeField] private float moveSpeed = 5f;

    private bool _isMoving = false;



    //input fields
    public InputSystem_Actions ISAs;
    private InputAction _moveInput;
    private Vector2 _directionalInput;

    //components
    private Rigidbody2D rb2D;
    private AnimateCat _animCat;
    private Animator _animator;


    private void Awake()
    {
        ISAs = new InputSystem_Actions();
        rb2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _animCat = new AnimateCat(_animator);
    }
    void Start()
    {
        _directionalInput = Vector2.zero;
        CurrentDirection = Directions.Down;
        _isMoving = false;
    }

    void Update()
    {
        MovePlayer();
    }


    private void MovePlayer()
    {
        _directionalInput = _moveInput.ReadValue<Vector2>().normalized;
        UpdateDirectionState(_directionalInput);
        UpdateIsMoving();
        rb2D.linearVelocity = _directionalInput * moveSpeed;
        _animCat.SendAnimationInformation(CurrentDirection, _isMoving);
    }

    private void UpdateDirectionState(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x < 0)
            {
                CurrentDirection = Directions.Left;
            }
            else
            {
                CurrentDirection = Directions.Right;
            }
        }
        else
        {
            if (dir.y > 0)
            {
                CurrentDirection = Directions.Up;
            }
            else
            {
                CurrentDirection = Directions.Down;
            }
        }
    }

    private void UpdateIsMoving()
    {
        _isMoving = rb2D.linearVelocity.sqrMagnitude > 0.01f;
    }

    private void OnEnable()
    {
        _moveInput = ISAs.Player.Move;
        _moveInput.Enable();
    }

    private void OnDisable()
    {
        _moveInput.Disable();
    }
}
