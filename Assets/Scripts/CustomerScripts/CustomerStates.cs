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



