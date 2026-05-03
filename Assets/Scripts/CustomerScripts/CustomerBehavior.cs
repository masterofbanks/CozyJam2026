using Algorithms;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerBehavior : MonoBehaviour
{
    [Header("Targeting")]
    public Transform Target;
    public SeatBehavior Seat;
    public PlayerInput.Directions CurrentDirection = PlayerInput.Directions.Down;

    [Header("Ordering")]
    public string Order;//{ get; private set; }
    public int ID = -1;
    public TextMeshProUGUI IDText;

    [Header("Rating System")]
    public int MaxHappinessRating = 80;
    public int MinHappyRating = 60;
    public int MinMediocreRating = 30;
    public float DecreaseScaling = 0.0001f;
    public float CurrentRating; 


    public NavMeshAgent agent { get; private set; }
    private Animator anime;
    private AnimateCat animCatScript;
    public Animator AreaAnimator;
    public CustomerStateMachine stateMachine;

    private bool isMoving;
    public bool InOrderArea { get; private set; }
    public bool FinishedOrdering { get; private set; }
    public bool HasFood { get; private set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anime = GetComponent<Animator>();
        animCatScript = new AnimateCat(anime);
        stateMachine = new CustomerStateMachine(this);
        IDText = GetComponentInChildren<TextMeshProUGUI>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateMachine.Initialize(stateMachine.StartState);
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        CurrentRating = MaxHappinessRating;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        CurrentDirection = animCatScript.UpdateDirectionState(agent.velocity);
        UpdateIsMoving();
        animCatScript.SendAnimationInformation(CurrentDirection, isMoving);
    }

    private void UpdateIsMoving()
    {
        isMoving = agent.velocity.magnitude > 0.1f;
    }

    public void MoveTheCustomerNormally()
    {
        agent.SetDestination(Target.position);
        
    }

    public void AimCustomerAtTransform(Transform t)
    {
        Target = t;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("OrderingArea"))
        {
            Debug.Log($"{gameObject.name} has hit the ordering area!");
            InOrderArea = true;
        }

        else if (collision.gameObject.CompareTag("Seats"))
        {
            SeatBehavior otherSeat = collision.gameObject.GetComponent<SeatBehavior>();
            if(otherSeat == Seat)
            {
                transform.position = otherSeat.transform.position;
                agent.updatePosition = false;
            }
        }

        else if (collision.gameObject.CompareTag("LeavingArea"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("OrderingArea"))
        {
            Debug.Log($"{gameObject.name} has left the ordering area!");
            InOrderArea = false;
        }
    }

    public void TakeOrder()
    {
        FinishedOrdering = true;
    }

    public void GiveOrder(string order, int id)
    {
        Order = order;
        ID = id;
        IDText.text = id.ToString();
    }

    public void InProximityOfSittingCustomer()
    {
        if(!agent.updatePosition)
        {
            AreaAnimator.Play("InArea");
        }
    }

    public void GiveFood(string servedOrder)
    {
        HasFood = true;
        int distanceBetweenServedAndActual = LevenshteinDistance.Calculate(servedOrder, Order);
        Debug.Log(distanceBetweenServedAndActual);
        CurrentRating -= distanceBetweenServedAndActual;
        UIManager.Instance.RemoveOrderSlip(ID);
        GameManager.Instance.GiveFood(CurrentRating);

    }
}
