using UnityEngine;

public abstract class Tree : MonoBehaviour
{
    protected Node rootNode;

    protected virtual void Start()
    {
        rootNode = SetUp();
    }

    protected virtual void Update()
    {
        if (rootNode != null)
        {
            rootNode.Evaluate();
        }
    }

    protected abstract Node SetUp();
}
