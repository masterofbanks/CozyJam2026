using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public interface I_CustomerState
{
    public void Enter();
    public void Update();
    public void Exit();
}

public class CustomerStartState : I_CustomerState 
{
    private CustomerBehavior behaviorScript;
    public CustomerStartState(CustomerBehavior behaviorScript)
    {
        this.behaviorScript = behaviorScript;
    }

    public void Enter()
    {
        if (GameManager.Instance.CustomerAtOrderArea)
        {
            behaviorScript.CurrentWaitingIndex = CustomerManager.Instance.FirstFreeWaitingArea();
            if(behaviorScript.CurrentWaitingIndex == -1)
            {
                Debug.LogWarning("No free waiting area found for customer.");
                behaviorScript.AimCustomerAtTransform(behaviorScript.transform);
                return;
            }
            WaitingAreaBehavior waitingArea = CustomerManager.Instance.GetWaitingAreaBehavior(behaviorScript.CurrentWaitingIndex);
            behaviorScript.AimCustomerAtTransform(waitingArea.transform);
            waitingArea.hasCustomerInArea = true;
        }

        else
        {
            behaviorScript.AimCustomerAtTransform(CustomerManager.Instance.OrderingArea);
            GameManager.Instance.BeginOrderSequence();
        }
    }

    public void Update()
    {
        behaviorScript.MoveTheCustomerNormally();
        behaviorScript.UpdateRatingSystem(Time.deltaTime);
        if (behaviorScript.InOrderArea && !behaviorScript.FinishedOrdering)
        {
            CustomerManager.Instance.SetCurrentCustomer(behaviorScript);
        }
        else if (behaviorScript.FinishedOrdering)
        {
            behaviorScript.stateMachine.TransitionTo(behaviorScript.stateMachine.WaitingForFoodState);
        }
    }

    public void Exit()
    {

    }

}

public class WaitForFoodState : I_CustomerState
{
    private CustomerBehavior behaviorScript;
    public WaitForFoodState(CustomerBehavior behaviorScript)
    {
        this.behaviorScript = behaviorScript;
    }

    public void Enter()
    {
        SeatBehavior newPosition = CustomerManager.Instance.GetPositionOfFreeSeat();
        newPosition.OccupySeat();
        behaviorScript.AimCustomerAtTransform(newPosition.transform);
        behaviorScript.Seat = newPosition;
        GameManager.Instance.EndOrderSequence();
    }

    public void Update()
    {
        behaviorScript.MoveTheCustomerNormally();
        behaviorScript.UpdateRatingSystem(Time.deltaTime);
        if (behaviorScript.HasFood)
        {
            behaviorScript.stateMachine.TransitionTo(behaviorScript.stateMachine.UtilityState);
        }
    }

    public void Exit()
    {

    }
}

public class UtilityState : I_CustomerState
{
    private CustomerBehavior behaviorScript;
    public UtilityStateMachine utilStateMachine;
    public UtilityState(CustomerBehavior behaviorScript)
    {
        this.behaviorScript = behaviorScript;
        utilStateMachine = new UtilityStateMachine(this.behaviorScript);
    }

    public void Enter()
    {
        utilStateMachine.Initialize(utilStateMachine.EatingState);
        behaviorScript.InitializeUtilityValues();
    }

    public void Update()
    {
        utilStateMachine.Update();
    }

    public void Exit()
    {

    }
}

public class EatingState : I_CustomerState
{
    private CustomerBehavior behaviorScript;
    private float _maxEatingDuration = 5.0f;
    private float t;
    public EatingState(CustomerBehavior behaviorScript)
    {
        this.behaviorScript = behaviorScript;
    }

    public void Enter()
    {
        Debug.Log($"{behaviorScript.name} is eating right now");
        t = _maxEatingDuration;
    }

    public void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
        }

        else
        {
            t = 0;
            //transition to next utility state
            behaviorScript.stateMachine.UtilityState.utilStateMachine.TransitionTo(behaviorScript.stateMachine.UtilityState.utilStateMachine.FindHighestUtility());
        }

    }

    public void Exit()
    {

    }

   
}


public class BoredState : I_CustomerState
{
    private CustomerBehavior behaviorScript;
    
    public BoredState(CustomerBehavior behaviorScript)
    {
        this.behaviorScript = behaviorScript;
    }

    public void Enter()
    {
        Debug.Log($"{behaviorScript.name} is bored right now");
        behaviorScript.stateMachine.TransitionTo(behaviorScript.stateMachine.LeavingState);
    }

    public void Update()
    {
        

    }

    public void Exit()
    {

    }


}

public class BusyState : I_CustomerState
{
    private CustomerBehavior behaviorScript;

    public BusyState(CustomerBehavior behaviorScript)
    {
        this.behaviorScript = behaviorScript;
    }

    public void Enter()
    {
        Debug.Log($"{behaviorScript.name} is busy right now");
        behaviorScript.ResetCustomer();
        //behaviorScript.CompleteTakingTheCustomerOrder();
        UIManager.Instance.AddOrderToUI(behaviorScript.Order, behaviorScript.ID);
        CustomerManager.Instance.numCustomers++;
    }

    public void Update()
    {
        if (behaviorScript.HasFood)
        {
            behaviorScript.stateMachine.TransitionTo(behaviorScript.stateMachine.LeavingState);

        }

    }

    public void Exit()
    {

    }


}

public class LoveState : I_CustomerState
{
    private CustomerBehavior behaviorScript;

    public LoveState(CustomerBehavior behaviorScript)
    {
        this.behaviorScript = behaviorScript;
    }

    public void Enter()
    {
        Debug.Log($"{behaviorScript.CustomerName} is in love right now");
        behaviorScript.stateMachine.TransitionTo(behaviorScript.stateMachine.LeavingState);
    }

    public void Update()
    {


    }

    public void Exit()
    {

    }


}

public class LeavingState : I_CustomerState
{
    private CustomerBehavior behaviorScript;
    public LeavingState(CustomerBehavior behaviorScript)
    {
        this.behaviorScript = behaviorScript;
    }

    public void Enter()
    {
        behaviorScript.agent.updatePosition = true;
        behaviorScript.AimCustomerAtTransform(CustomerManager.Instance.LeavingArea);
        behaviorScript.Seat.DeoccupySeat();
    }

    public void Update()
    {
        behaviorScript.MoveTheCustomerNormally();
        
    }

    public void Exit()
    {

    }
}



