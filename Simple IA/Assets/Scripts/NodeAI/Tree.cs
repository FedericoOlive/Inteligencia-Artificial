using UnityEngine;

public abstract class Tree : MonoBehaviour
{
    protected Node rootNode;

    void Start ()
    {
        rootNode = SetUp();
    }

    void Update ()
    {
        if (rootNode != null)
        {
            rootNode.Evaluate();
        }
    }

    protected abstract Node SetUp ();
}