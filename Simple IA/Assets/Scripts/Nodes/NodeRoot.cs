using System.Collections.Generic;
using UnityEngine;

public class NodeRoot : Node
{
    public NodeRoot()
    {
    }

    public NodeRoot(List<Node> childrens) : base(childrens)
    {
    }

    public override NodeState Evaluate()
    {
        foreach (Node node in childrens)
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    state = NodeState.Running;
                    break;
                case NodeState.Success:
                    state = NodeState.Success;
                    break;
                case NodeState.Failure:
                    state = NodeState.Failure;
                    break;
                default:
                    Debug.Log("Entró al caso default. ERROR!");
                    break;
            }
        }

        return state;
    }
}
