using System.Collections.Generic;

public class MinerBT : Tree
{
    protected override void Start ()
    {
        base.Start();
    }
    
    protected override void Update ()
    {
        base.Update();
    }

    protected override Node SetUp ()
    {
        Node root = new NodeRoot(new List<Node>
        {
            new NodeSequence(new List<Node>
            {
                new NodeNot(new List<Node>
                {
                    new NodeSequence(new List<Node>
                    {
                        new LimitMovementNode(new List<Node>(), gameObject),
                        new MovementNode(new List<Node>(), gameObject)
                    }),
                }),
                new RotationNode(new List<Node>(), gameObject)
            })
        });

        return root;
    }
}