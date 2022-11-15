using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNode : Node
{
    private GameObject go;
    public MovementNode(List<Node> childrens, GameObject go) : base(childrens)
    {
        this.go = go;
    }

    public override NodeState Evaluate()
    {
        go.transform.position += Vector3.right * Time.deltaTime;
        state = NodeState.Running;
        return state;
    }
}
