using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationNode : Node
{
    private GameObject go;
    public RotationNode(List<Node> childrens, GameObject go) : base(childrens)
    {
        this.go = go;
    }

    public override NodeState Evaluate()
    {

        go.transform.Rotate(Vector3.forward, Space.World);
        state = NodeState.Running;
        return state;
    }
}
