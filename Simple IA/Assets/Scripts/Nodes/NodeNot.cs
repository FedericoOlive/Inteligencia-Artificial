using System.Collections.Generic;
using UnityEngine;

public class NodeNot : Node
{
    public NodeNot()
    {
    }

    public NodeNot(List<Node> childrens) : base(childrens)
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
                    state = NodeState.Failure;
                    break;
                case NodeState.Failure:
                    state = NodeState.Success;
                    break;
                default:
                    Debug.Log("Entró al caso default. ERROR!");
                    break;
            }
        }

        return state;
    }
}