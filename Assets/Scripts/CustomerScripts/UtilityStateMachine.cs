using System;
using System.Collections.Generic;
using System.Linq;
// handles
[Serializable]
public class UtilityStateMachine
{
    public I_CustomerState CurrentState { get; private set; }


    // reference to the state objects
    public I_CustomerState EatingState;
    public I_CustomerState BoredState;
    public I_CustomerState BusyState;
    public I_CustomerState LoveState;


    // event to notify other objects of the state change
    public event Action<I_CustomerState> stateChanged;
    public CustomerBehavior behvaviorScript;

    // pass in necessary parameters into constructor 
    public UtilityStateMachine(CustomerBehavior customer)
    {
        // create an instance for each state and pass in PlayerController
        EatingState = new EatingState(customer);
        BoredState = new BoredState(customer);
        BusyState = new BusyState(customer);
        LoveState = new LoveState(customer);
        behvaviorScript = customer;
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

    public I_CustomerState FindHighestUtility()
    {
        // Source - https://stackoverflow.com/a/1332
        // Posted by caryden, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-07-30, License - CC BY-SA 3.0

        Dictionary<string, float> myDict = new Dictionary<string, float>();
        myDict.Add("Bored", behvaviorScript.Boredom);
        myDict.Add("Love", behvaviorScript.Lovesickness);
        myDict.Add("Busy", behvaviorScript.Business);

        var sortedDict = from entry in myDict orderby entry.Value descending select entry;
        string answer = sortedDict.First().Key;
        switch (answer)
        {
            case "Bored":
                return BoredState;
            case "Love":
                return LoveState;
            case "Busy":
                return BusyState;
            default:
                return BoredState;
        }


    }
}
