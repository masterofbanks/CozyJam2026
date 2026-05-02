using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerBehavior : MonoBehaviour
{
    [Header("Targeting")]
    public Transform Target;
    
    public PlayerInput.Directions CurrentDirection = PlayerInput.Directions.Down;

    private NavMeshAgent agent;
    private Animator anime;
    private AnimateCat animCatScript;
    public CustomerStateMachine stateMachine;

    private bool isMoving;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anime = GetComponent<Animator>();
        animCatScript = new AnimateCat(anime);
        stateMachine = new CustomerStateMachine(this);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateMachine.Initialize(stateMachine.StartState);
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    private void UpdateIsMoving()
    {
        isMoving = agent.velocity.magnitude > 0.1f;
    }

    public void MoveTheCustomerNormally()
    {
        agent.SetDestination(Target.position);
        CurrentDirection = animCatScript.UpdateDirectionState(agent.velocity);
        UpdateIsMoving();
        animCatScript.SendAnimationInformation(CurrentDirection, isMoving);
    }

    public void AimCustomerAtTransform(Transform t)
    {
        Target = t;
    }


}
