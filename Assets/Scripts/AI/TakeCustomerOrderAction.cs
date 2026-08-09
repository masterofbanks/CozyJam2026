using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakeCustomerOrder", story: "Employee Takes Customer Order", category: "Action", id: "50a11427a5955811bfc87136075a4dca")]
public partial class TakeCustomerOrderAction : Action
{

    public Transform RegisterStation;
    public List<string> PendingOrders;
    public NavMeshAgent agent;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

