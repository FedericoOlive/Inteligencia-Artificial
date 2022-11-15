using System.Collections.Generic;

public enum NodeState
{
    Running,
    Success,
    Failure
}

public abstract class Node
{
    public NodeState state;
    public Node parent;
    public List<Node> childrens = new List<Node>();

    private Dictionary<string, object> data = new Dictionary<string, object>();

    public Node()
    {
        parent = null;
    }

    public Node(List<Node> childrens)
    {
        foreach (Node children in childrens)
        {
            Attach(children);
        }
    }

    public void Attach(Node node)
    {
        node.parent = this;
        childrens.Add(node);
    }

    public virtual NodeState Evaluate() => NodeState.Failure;

    public void SetData(string key, object value)
    {
        if (data.ContainsKey(key))
            data[key] = value;
        else
            data.Add(key, value);
    }

    public object GetData<T>(string key)
    {
        if (data.TryGetValue(key, out var value))
            return value;

        Node parentNode = parent;
        while (parentNode != null)
        {
            value = parentNode.GetData<T>(key);
            if (value != null)
                return value;

            parentNode = parent;
        }

        return default;
    }

    public bool RemoveData(string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);
            return true;
        }

        Node parentNode = parent;
        while (parentNode != null)
        {
            bool cleaned = parentNode.RemoveData(key);
            if (cleaned)
                return true;

            parentNode = parent;
        }

        return false;
    }
}
