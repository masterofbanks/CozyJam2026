using Algorithms;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CustomerBehavior : MonoBehaviour
{
    [Header("Targeting")]
    public Transform Target;
    public SeatBehavior Seat;
    public PlayerInput.Directions CurrentDirection = PlayerInput.Directions.Down;

    [Header("Ordering")]
    public string CustomerName;
    public string Order;//{ get; private set; }
    public int ID = -1;
    public TextMeshProUGUI IDText;
    public TextMeshProUGUI NameTextBox;
    private float _timeAlive = 0f;
    public int CurrentWaitingIndex = -1;

    [Header("Rating System")]
    public int MaxHappinessRating = 80;
    public int MinHappyRating = 60;
    public int MinMediocreRating = 30;
    public float DecreaseScaling = 0.0001f;
    public float CurrentRating;
    public float TimeToStartDecreasing = 140f;

    [Header("Character Animations")]
    [SerializeField] protected CharacterAnimations anims;
    public Animator AreaAnimator;
    public Animator MoodAnimator;
    protected Animator anime;
    protected AnimateCat animCatScript;

    public NavMeshAgent agent;
    public CustomerStateMachine stateMachine;

    private bool isMoving;
    public bool InOrderArea { get; private set; }
    public bool FinishedOrdering { get; private set; }
    public bool HasFood { get; private set; }

    private bool oldUpdatePosition = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anime = GetComponent<Animator>();
        anime.runtimeAnimatorController = anims.GetRandomCharacterController();
        animCatScript = new AnimateCat(anime);
        stateMachine = new CustomerStateMachine(this);
        IDText = GetComponentInChildren<TextMeshProUGUI>();
        GameManager.Instance.FreezeCustomers += FreezeCustomer;
        GameManager.Instance.UnfreezeCustomers += UnfreezeCustomer;
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
        MoodAnimator.SetFloat("Rating", CurrentRating);

    }

    private void UpdateIsMoving()
    {
        isMoving = agent.velocity.magnitude > 0.1f;
    }

    public void MoveTheCustomerNormally()
    {
        if (agent.enabled)
        {
            agent.SetDestination(Target.position);

        }

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
            GameManager.Instance.LoadNextLevel();
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

    public virtual void TakeOrder()
    {
        FinishedOrdering = true;
        CustomerManager.Instance.CompleteCustomerOrder();
    }

    public virtual void MoveCameraOfCustomer(OrderInteractable interactable)
    {
        //Do Nothing since this is a normal customer
    }
    

    public virtual void GiveOrder(CustomerPreset preset, int id)
    {
        Order = preset.GetOrder();
        ID = id;
        IDText.text = id.ToString();
        NameTextBox.text = "";
        /*CustomerName = preset.CustomerName;
        NameTextBox.text = CustomerName;


        anime.runtimeAnimatorController = anims.GetCharacterController(preset.AppearanceIndex);*/
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
        GameManager.Instance.GiveFoodToCustomer(CurrentRating);


    }

    protected void FreezeCustomer()
    {
        oldUpdatePosition = agent.updatePosition;
        agent.updatePosition = false;
        agent.enabled = false;  
    }

    protected void UnfreezeCustomer()
    {
        agent.updatePosition = oldUpdatePosition;
        agent.enabled = true;
    }

    private void OnDestroy()
    {
        GameManager.Instance.FreezeCustomers -= FreezeCustomer;
        GameManager.Instance.UnfreezeCustomers -= UnfreezeCustomer;
    }

    public bool CanDecreaseRating()
    {
        return CurrentRating > 0 && _timeAlive > TimeToStartDecreasing && !GameManager.Instance.CustomersAreFrozen && GameManager.Instance.FirstCustomerServed;
    }

    public void UpdateRatingSystem(float deltaTime)
    {
        _timeAlive += deltaTime;
        if(CanDecreaseRating())
        {
            CurrentRating -= DecreaseScaling * deltaTime;

        }

    }
}
