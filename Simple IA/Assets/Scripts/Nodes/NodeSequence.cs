using System.Collections.Generic;
using UnityEngine;

public class NodeSequence : Node
{
    public NodeSequence() { }

    public NodeSequence(List<Node> childrens) : base(childrens) { }

    public override NodeState Evaluate()
    {
        bool anyRunning = false;
        foreach (Node node in childrens)
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    anyRunning = true;
                    break;
                case NodeState.Success:
                    break;
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
                default:
                    Debug.Log("Entró al caso default. ERROR!");
                    break;
            }
        }

        state = anyRunning ? NodeState.Running : NodeState.Success;
        return state;
    }
}
