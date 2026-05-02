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
            GameManager.Instance.OrderSequence();
        }
    }

    public void Update()
    {
        behaviorScript.MoveTheCustomerNormally();
    }

    public void Exit()
    {

    }



}


