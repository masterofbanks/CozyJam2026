using UnityEngine;
using System.Collections.Generic;

using System.Linq;
public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;
    [Header("Waiting Areas")]
    public Transform WaitingArea;
    public Transform OrderingArea;
    [SerializeField] private Transform Seats;
    private SeatBehavior[] _seatScripts;
    private int _currentFreeSeatIndex = 0;

    [Header("Customer Data")]
    [SerializeField] private GameObject CustomerPrefab;
    [SerializeField] private Transform CustomerSpawnPos;
    [SerializeField] private Queue<CustomerBehavior> CustomersInLine = new();
    public CustomerBehavior CurrentCustomer = null;

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
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
