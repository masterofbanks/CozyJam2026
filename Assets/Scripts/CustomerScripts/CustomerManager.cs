using UnityEngine;
using System.Collections.Generic;

using System.Linq;
using UnityEngine.Rendering;
using System;
using System.IO;
using System.Runtime.CompilerServices;
public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;
    [Header("Waiting Areas")]
    public Transform[] WaitingAreas;
    public Transform OrderingArea;
    public Transform LeavingArea;
    [SerializeField] private Transform Seats;
    private SeatBehavior[] _seatScripts;
    private int numCustomers = 0;

    [Header("Customer Data")]
    [SerializeField] private GameObject CustomerPrefab;
    [SerializeField] private Transform CustomerSpawnPos;
    [SerializeField] private Queue<CustomerBehavior> CustomersInLine = new();
    public CustomerBehavior CurrentCustomer = null;
    private CustomerPreset[] _customerPresets;
    private CustomerPreset _randoCustomer;
    //factory fields
    private CustomerFactory _currentCustomerFactory;
    private NormalCustomerFactory _normalCustomerFactory;
    private NamedCustomerFactory _namedCustomerFactory;

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

        //Find All Customer Presets and Load them Into the _customerPresets array
        _customerPresets = Resources.LoadAll<CustomerPreset>("ExistingCustomers");
        if(_customerPresets != null)
        {
            Debug.Log($"Loaded {_customerPresets.Length} customer presets.");
            if(_customerPresets.Length == 0)
            {
                Debug.LogWarning("No customer presets found in Resources/ExistingCustomers!");
            }

            else
            {
                Debug.Log("Customers Found!");
            }
        }
        _randoCustomer = Resources.Load<CustomerPreset>("Rando");
        if(_randoCustomer == null)
        {
            Debug.LogError("Could not find the rando Customer within the resources file");
        }
        _normalCustomerFactory = new NormalCustomerFactory();
        _namedCustomerFactory = new NamedCustomerFactory();
        _currentCustomerFactory = _normalCustomerFactory;

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
        if(CurrentCustomer != null && GetPositionOfFreeSeat() != null)
        {
            CurrentCustomer.TakeOrder();
            UIManager.Instance.AddOrderToUI(CurrentCustomer.Order, CurrentCustomer.ID);
            CurrentCustomer = null;
            CustomersInLine.Dequeue();
            if (CustomersInLine.Count >= 1)
            {
                CustomerBehavior newCustomerAtFront = CustomersInLine.Peek();
                newCustomerAtFront.AimCustomerAtTransform(CustomerManager.Instance.OrderingArea);
                for(int i = 1; i < CustomersInLine.Count; i++)
                {
                    CustomerBehavior customer = CustomersInLine.ElementAt(i);
                    if (customer != null)
                    {
                        customer.CurrentWaitingIndex = i;
                        customer.AimCustomerAtTransform(WaitingAreas[i-1]);
                    }
                }   
                int indexOfFreeSeat = FirstFreeWaitingArea();
                if(indexOfFreeSeat > 0)
                {
                    WaitingAreas[indexOfFreeSeat - 1].GetComponent<WaitingAreaBehavior>().hasCustomerInArea = false;
                }
                else
                {
                    WaitingAreas[WaitingAreas.Length - 1].GetComponent<WaitingAreaBehavior>().hasCustomerInArea = false;
                }
                
            }
            Debug.Log(CustomersInLine.Count);

        }

        else
        {
            Debug.Log("Theres no customer here!!!");
        }
    }

    public void SpawnCustomer(CustomerPreset preset)
    {
        if (preset == null)
        {
            preset = _randoCustomer;
            _currentCustomerFactory = _normalCustomerFactory;
        }
        else
        {
            _currentCustomerFactory = _namedCustomerFactory;
        }
        GameObject customerClone = _currentCustomerFactory.CreateCustomer(CustomerSpawnPos.position, Quaternion.identity, preset, ++numCustomers); 
        CustomersInLine.Enqueue(customerClone.GetComponent<CustomerBehavior>());

    }

    public SeatBehavior GetPositionOfFreeSeat()
    {
        for(int i = 0; i < _seatScripts.Length; i++)
        {
            if (!_seatScripts[i].Occupied)
            {
                return _seatScripts[i];
            }
        }

        return null;

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

    public int FirstFreeWaitingArea()
    {
        for(int i = 0; i < WaitingAreas.Length; i++)
        {
            if(!WaitingAreas[i].gameObject.GetComponent<WaitingAreaBehavior>().hasCustomerInArea)
            {
                return i;
            }
        }

        Debug.Log("No free waiting areas found!");
        return -1;
    }

    public WaitingAreaBehavior GetWaitingAreaBehavior(int index)
    {
        if(index >= 0 && index < WaitingAreas.Length)
        {
            return WaitingAreas[index].gameObject.GetComponent<WaitingAreaBehavior>();
        }
        Debug.LogWarning($"Index {index} is out of bounds for WaitingAreas.");
        return null;
    }

}
