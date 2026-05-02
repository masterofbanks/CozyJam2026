using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerBehavior : MonoBehaviour
{
    [SerializeField] private Transform Target;
    public PlayerInput.Directions CurrentDirection = PlayerInput.Directions.Down;

    private NavMeshAgent agent;
    private Animator anime;
    private AnimateCat animCatScript;

    private bool isMoving;
    public Vector3 velo;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anime = GetComponent<Animator>();
        animCatScript = new AnimateCat(anime);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        velo = agent.velocity;
        agent.SetDestination(Target.position);
        CurrentDirection = animCatScript.UpdateDirectionState(agent.velocity);
        UpdateIsMoving();
        animCatScript.SendAnimationInformation(CurrentDirection, isMoving);
    }

    private void UpdateIsMoving()
    {
        isMoving = agent.velocity.magnitude > 0.1f;
    }
}
