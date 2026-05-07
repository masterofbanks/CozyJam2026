using UnityEngine;
using System.Collections.Generic;

using System.Linq;
using UnityEngine.Rendering;
public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;
    [Header("Waiting Areas")]
    public Transform WaitingArea;
    public Transform OrderingArea;
    public Transform LeavingArea;
    [SerializeField] private Transform Seats;
    private SeatBehavior[] _seatScripts;
    private int numCustomers = 0;

    [Header("Customer Data")]
    [SerializeField] private GameObject CustomerPrefab;
    [SerializeField] private Transform CustomerSpawnPos;
    [SerializeField] private Queue<CustomerBehavior> CustomersInLine = new();
    [SerializeField] private List<CoffeeOrder> CoffeeOrderTypes = new();
    [SerializeField] private List<Recipe> FoodOrderTypes = new();
    public CustomerBehavior CurrentCustomer = null;

    private System.Random _random = new System.Random();    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }

        _seatScripts = Seats.gameObject.GetComponentsInChildren<SeatBehavior>();
        _random = new System.Random();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.StartedNewLevel += ResetCustomerManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentCustomer(CustomerBehavior currentCustomer)
    {
        CurrentCustomer = currentCustomer;
    }

    public void TakeCustomerOrder()
    {
        if(CurrentCustomer != null)
        {
            CurrentCustomer.TakeOrder();
            UIManager.Instance.AddOrderToUI(CurrentCustomer.Order, CurrentCustomer.ID);
            CurrentCustomer = null;
            CustomersInLine.Dequeue();
            if (CustomersInLine.Count >= 1)
            {
                CustomerBehavior newCustomerAtFront = CustomersInLine.Peek();
                newCustomerAtFront.AimCustomerAtTransform(CustomerManager.Instance.OrderingArea);
            }
            Debug.Log(CustomersInLine.Count);

        }

        else
        {
            Debug.Log("Theres no customer here!!!");
        }
    }

    public void SpawnCustomer()
    {
        GameObject customerClone = Instantiate(CustomerPrefab, CustomerSpawnPos.position, Quaternion.identity);
        numCustomers++;
        customerClone.GetComponent<CustomerBehavior>().GiveOrder(CreateCustomerOrder(), numCustomers);
        CustomersInLine.Enqueue(customerClone.GetComponent<CustomerBehavior>());
        Debug.Log(CustomersInLine.Count);

    }

    public SeatBehavior GetPositionOfFreeSeat()
    {
        for(int i = 0; i < _seatScripts.Length; i++)
        {
            if (!_seatScripts[i].Occupied)
            {
                _seatScripts[i].OccupySeat();
                return _seatScripts[i];
            }
        }

        return null;

    }

    public string CreateCustomerOrder()
    {
        int randIndex = _random.Next(0, CoffeeOrderTypes.Count);
        CoffeeOrder order = (CoffeeOrder)ScriptableObject.CreateInstance("CoffeeOrder");
        order.Type = CoffeeOrderTypes[randIndex].Type;
        order.NumMilk = 0;
        order.NumSugar = 0;
        string drinksOrder = order.ToString();
        randIndex = _random.Next(0, FoodOrderTypes.Count);
        string finalOrder = drinksOrder + $"-{FoodOrderTypes[randIndex].name}";
        return finalOrder;
    }

    public bool TestForCustomersInLine()
    {
        return CustomersInLine.Count > 0;
    }

    public void ResetCustomerManager()
    {
        numCustomers = 0;
        foreach(SeatBehavior seat in _seatScripts)
        {
            seat.DeoccupySeat();
        }
    }

}
