using System.Collections.Generic;

public class AntBT : Tree
{
    void Start ()
    {

    }

    void Update ()
    {

    }

    protected override Node SetUp ()
    {
        Node root = new NodeRoot(new List<Node>
        {
            new NodeSequence(new List<Node>
            {
                new NodeNot(new List<Node>
                {
                    //MyLeaveNode
                }),
                new NodeSequence(new List<Node>
                {
                    new NodeNot(new List<Node>(
                        //MyLeaveNode2
                    ))
                    //MyLeaveNode3
                })
            })
        });
        return root;
    }
}