using System;
using Unity.VisualScripting;
using UnityEngine;

// handles
[Serializable]
public class CustomerStateMachine
{
    public I_CustomerState CurrentState { get; private set; }


    // reference to the state objects
    public CustomerStartState StartState { get; private set; } 
    public WaitForFoodState WaitingForFoodState { get; private set; }
    public LeavingState LeavingState { get; private set; }
    public UtilityState UtilityState { get; private set; }


    // event to notify other objects of the state change
    public event Action<I_CustomerState> stateChanged;


    // pass in necessary parameters into constructor 
    public CustomerStateMachine(CustomerBehavior customer)
    {
        // create an instance for each state and pass in PlayerController
        this.StartState = new CustomerStartState(customer);
        this.WaitingForFoodState = new WaitForFoodState(customer);
        this.LeavingState = new LeavingState(customer); 
        this.UtilityState = new UtilityState(customer);
    }


    // set the starting state
    public void Initialize(I_CustomerState state)
    {
        CurrentState = state;
        state.Enter();


        // notify other objects that state has changed
        stateChanged?.Invoke(state);
    }


    // exit this state and enter another
    public void TransitionTo(I_CustomerState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();


        // notify other objects that state has changed
        stateChanged?.Invoke(nextState);
    }


    // allow the StateMachine to update this state
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}
