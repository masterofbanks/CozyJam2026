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
            behaviorScript.AimCustomerAtTransform(CustomerManager.Instance.WaitingArea);
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
        if (behaviorScript.CurrentRating > 0 && !GameManager.Instance.CustomersAreFrozen && GameManager.Instance.FirstCustomerServed)
        {
            behaviorScript.CurrentRating -= behaviorScript.DecreaseScaling * Time.deltaTime;
        }
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
        if(newPosition == null)
        {
            return;
        }
        behaviorScript.AimCustomerAtTransform(newPosition.transform);
        behaviorScript.Seat = newPosition;
        GameManager.Instance.EndOrderSequence();
    }

    public void Update()
    {
        behaviorScript.MoveTheCustomerNormally();
        if(behaviorScript.CurrentRating > 0 && !GameManager.Instance.CustomersAreFrozen && GameManager.Instance.FirstCustomerServed)
        {
            behaviorScript.CurrentRating -= behaviorScript.DecreaseScaling * Time.deltaTime;
        }
        if (behaviorScript.HasFood)
        {
            behaviorScript.stateMachine.TransitionTo(behaviorScript.stateMachine.LeavingState);
        }
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
    }

    public void Update()
    {
        behaviorScript.MoveTheCustomerNormally();
        
    }

    public void Exit()
    {

    }
}



